using Contracts;
using Entities.Exceptions;
using Entities.Models;
using FluentAssertions;
using Moq;
using ReceptionBook.Tests.Helpers;
using Service;
using Shared.DataTransferObjects;

namespace ReceptionBook.Tests.Services;

public class ReservationServiceTests
{
    private const decimal PricePerNight = 200m;

    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IGuestRepository> _guestRepositoryMock = new();
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();
    private readonly ReservationService _sut;

    private readonly Room _room = new()
    {
        Id = Guid.NewGuid(),
        Number = "201",
        Capacity = 2,
        PricePerNight = PricePerNight,
        Status = RoomStatus.Available
    };

    private readonly Guest _guest = new() { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Kowalski" };

    public ReservationServiceTests()
    {
        _repositoryMock.Setup(r => r.Room).Returns(_roomRepositoryMock.Object);
        _repositoryMock.Setup(r => r.Guest).Returns(_guestRepositoryMock.Object);
        _repositoryMock.Setup(r => r.Reservation).Returns(_reservationRepositoryMock.Object);

        _roomRepositoryMock.Setup(r => r.GetRoomAsync(_room.Id, false)).ReturnsAsync(_room);
        _guestRepositoryMock.Setup(r => r.GetGuestAsync(_guest.Id, false)).ReturnsAsync(_guest);

        _sut = new ReservationService(_repositoryMock.Object, Mock.Of<ILoggerManager>(), TestMapper.Create());
    }

    [Fact]
    public async Task CreateReservationAsync_WhenCheckOutIsNotAfterCheckIn_ThrowsBadRequestException()
    {
        var reservation = CreateReservationDto(nights: 0);

        var act = () => _sut.CreateReservationAsync(reservation);

        await act.Should().ThrowAsync<ReservationDatesBadRequestException>();
    }

    [Fact]
    public async Task CreateReservationAsync_WhenNumberOfGuestsExceedsCapacity_ThrowsBadRequestException()
    {
        var reservation = CreateReservationDto(numberOfGuests: _room.Capacity + 1);

        var act = () => _sut.CreateReservationAsync(reservation);

        await act.Should().ThrowAsync<RoomCapacityExceededBadRequestException>();
    }

    [Fact]
    public async Task CreateReservationAsync_WhenRoomIsAlreadyReserved_ThrowsBadRequestException()
    {
        SetupOverlap(hasOverlap: true);

        var act = () => _sut.CreateReservationAsync(CreateReservationDto());

        await act.Should().ThrowAsync<RoomNotAvailableBadRequestException>();
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateReservationAsync_WithValidData_CalculatesTotalPriceAndConfirms()
    {
        const int nights = 3;
        Reservation? createdReservation = null;
        SetupOverlap(hasOverlap: false);
        _reservationRepositoryMock
            .Setup(r => r.CreateReservation(It.IsAny<Reservation>()))
            .Callback<Reservation>(r => createdReservation = r);
        _reservationRepositoryMock
            .Setup(r => r.GetReservationAsync(It.IsAny<Guid>(), false))
            .ReturnsAsync(() => createdReservation);

        var result = await _sut.CreateReservationAsync(CreateReservationDto(nights: nights));

        createdReservation!.TotalPrice.Should().Be(nights * PricePerNight);
        createdReservation.Status.Should().Be(ReservationStatus.Confirmed);
        result.TotalPrice.Should().Be(nights * PricePerNight);
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateReservationAsync_DoesNotTreatTheReservationAsConflictWithItself()
    {
        var reservation = CreateReservation(ReservationStatus.Confirmed);
        _reservationRepositoryMock.Setup(r => r.GetReservationAsync(reservation.Id, true)).ReturnsAsync(reservation);
        SetupOverlap(hasOverlap: false);

        await _sut.UpdateReservationAsync(reservation.Id, CreateReservationUpdateDto(), trackChanges: true);

        _reservationRepositoryMock.Verify(r => r.HasOverlappingReservationAsync(_room.Id,
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), reservation.Id), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CheckInAsync_WhenReservationIsConfirmed_ChecksInAndOccupiesRoom()
    {
        var reservation = CreateReservation(ReservationStatus.Confirmed);
        _reservationRepositoryMock.Setup(r => r.GetReservationAsync(reservation.Id, true)).ReturnsAsync(reservation);

        await _sut.CheckInAsync(reservation.Id);

        reservation.Status.Should().Be(ReservationStatus.CheckedIn);
        _room.Status.Should().Be(RoomStatus.Occupied);
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CheckInAsync_WhenRoomIsBeingCleaned_ThrowsBadRequestException()
    {
        _room.Status = RoomStatus.Cleaning;
        var reservation = CreateReservation(ReservationStatus.Confirmed);
        _reservationRepositoryMock.Setup(r => r.GetReservationAsync(reservation.Id, true)).ReturnsAsync(reservation);

        var act = () => _sut.CheckInAsync(reservation.Id);

        await act.Should().ThrowAsync<RoomNotReadyBadRequestException>();
    }

    [Fact]
    public async Task CheckOutAsync_WhenGuestIsCheckedIn_MarksRoomForCleaning()
    {
        _room.Status = RoomStatus.Occupied;
        var reservation = CreateReservation(ReservationStatus.CheckedIn);
        _reservationRepositoryMock.Setup(r => r.GetReservationAsync(reservation.Id, true)).ReturnsAsync(reservation);

        await _sut.CheckOutAsync(reservation.Id);

        reservation.Status.Should().Be(ReservationStatus.CheckedOut);
        _room.Status.Should().Be(RoomStatus.Cleaning);
    }

    [Theory]
    [InlineData(ReservationStatus.CheckedIn)]
    [InlineData(ReservationStatus.CheckedOut)]
    [InlineData(ReservationStatus.Cancelled)]
    public async Task CancelAsync_WhenReservationIsNotConfirmed_ThrowsBadRequestException(ReservationStatus status)
    {
        var reservation = CreateReservation(status);
        _reservationRepositoryMock.Setup(r => r.GetReservationAsync(reservation.Id, true)).ReturnsAsync(reservation);

        var act = () => _sut.CancelAsync(reservation.Id);

        await act.Should().ThrowAsync<InvalidReservationStatusBadRequestException>();
    }

    private void SetupOverlap(bool hasOverlap) =>
        _reservationRepositoryMock
            .Setup(r => r.HasOverlappingReservationAsync(_room.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<Guid?>()))
            .ReturnsAsync(hasOverlap);

    private ReservationForCreationDto CreateReservationDto(int nights = 2, int numberOfGuests = 1) => new()
    {
        RoomId = _room.Id,
        GuestId = _guest.Id,
        CheckInDate = DateTime.Today.AddDays(1),
        CheckOutDate = DateTime.Today.AddDays(1 + nights),
        NumberOfGuests = numberOfGuests
    };

    private ReservationForUpdateDto CreateReservationUpdateDto() => new()
    {
        RoomId = _room.Id,
        GuestId = _guest.Id,
        CheckInDate = DateTime.Today,
        CheckOutDate = DateTime.Today.AddDays(2),
        NumberOfGuests = 2
    };

    private Reservation CreateReservation(ReservationStatus status) => new()
    {
        Id = Guid.NewGuid(),
        RoomId = _room.Id,
        Room = _room,
        GuestId = _guest.Id,
        Guest = _guest,
        CheckInDate = DateTime.Today,
        CheckOutDate = DateTime.Today.AddDays(2),
        NumberOfGuests = 1,
        Status = status
    };
}
