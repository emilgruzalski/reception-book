using Contracts;
using Entities.Exceptions;
using Entities.Models;
using FluentAssertions;
using Moq;
using ReceptionBook.Tests.Helpers;
using Service;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace ReceptionBook.Tests.Services;

public class RoomServiceTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();
    private readonly RoomService _sut;

    public RoomServiceTests()
    {
        _repositoryMock.Setup(r => r.Room).Returns(_roomRepositoryMock.Object);
        _repositoryMock.Setup(r => r.Reservation).Returns(_reservationRepositoryMock.Object);

        _sut = new RoomService(_repositoryMock.Object, Mock.Of<ILoggerManager>(), TestMapper.Create());
    }

    [Fact]
    public async Task GetRoomAsync_WhenRoomDoesNotExist_ThrowsRoomNotFoundException()
    {
        var roomId = Guid.NewGuid();
        _roomRepositoryMock.Setup(r => r.GetRoomAsync(roomId, false)).ReturnsAsync((Room?)null);

        var act = () => _sut.GetRoomAsync(roomId, trackChanges: false);

        await act.Should().ThrowAsync<RoomNotFoundException>();
    }

    [Fact]
    public async Task CreateRoomAsync_WhenRoomNumberIsTaken_ThrowsBadRequestException()
    {
        _roomRepositoryMock.Setup(r => r.RoomNumberExistsAsync("101", null)).ReturnsAsync(true);
        var room = new RoomForCreationDto { Number = "101", Capacity = 1, PricePerNight = 180m };

        var act = () => _sut.CreateRoomAsync(room);

        await act.Should().ThrowAsync<RoomNumberAlreadyExistsBadRequestException>();
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRoomAsync_WithValidData_CreatesAvailableRoom()
    {
        Room? createdRoom = null;
        _roomRepositoryMock.Setup(r => r.RoomNumberExistsAsync("105", null)).ReturnsAsync(false);
        _roomRepositoryMock.Setup(r => r.CreateRoom(It.IsAny<Room>())).Callback<Room>(room => createdRoom = room);
        var room = new RoomForCreationDto { Number = "105", Type = RoomType.Double, Capacity = 2, PricePerNight = 250m };

        var result = await _sut.CreateRoomAsync(room);

        createdRoom.Should().NotBeNull();
        createdRoom!.Status.Should().Be(RoomStatus.Available);
        result.Number.Should().Be("105");
        result.Type.Should().Be(RoomType.Double);
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomAsync_WhenRoomHasReservations_ThrowsBadRequestException()
    {
        var room = new Room { Id = Guid.NewGuid(), Number = "101" };
        _roomRepositoryMock.Setup(r => r.GetRoomAsync(room.Id, false)).ReturnsAsync(room);
        _reservationRepositoryMock.Setup(r => r.AnyForRoomAsync(room.Id)).ReturnsAsync(true);

        var act = () => _sut.DeleteRoomAsync(room.Id, trackChanges: false);

        await act.Should().ThrowAsync<RoomHasReservationsBadRequestException>();
        _roomRepositoryMock.Verify(r => r.DeleteRoom(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task GetAllRoomsAsync_WhenMaxPriceIsLowerThanMinPrice_ThrowsBadRequestException()
    {
        var parameters = new RoomParameters { MinPrice = 300m, MaxPrice = 100m };

        var act = () => _sut.GetAllRoomsAsync(parameters, trackChanges: false);

        await act.Should().ThrowAsync<MaxPriceRangeBadRequestException>();
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_WhenCheckOutIsNotAfterCheckIn_ThrowsBadRequestException()
    {
        var parameters = new RoomAvailabilityParameters
        {
            CheckInDate = DateTime.Today.AddDays(2),
            CheckOutDate = DateTime.Today.AddDays(2)
        };

        var act = () => _sut.GetAvailableRoomsAsync(parameters, trackChanges: false);

        await act.Should().ThrowAsync<ReservationDatesBadRequestException>();
    }
}
