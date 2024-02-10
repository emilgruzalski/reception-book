using Contracts;
using Entities.Exceptions;
using Entities.Models;
using FluentAssertions;
using Moq;
using ReceptionBook.Tests.Helpers;
using Service;
using Shared.DataTransferObjects;

namespace ReceptionBook.Tests.Services;

public class GuestServiceTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IGuestRepository> _guestRepositoryMock = new();
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();
    private readonly GuestService _sut;

    public GuestServiceTests()
    {
        _repositoryMock.Setup(r => r.Guest).Returns(_guestRepositoryMock.Object);
        _repositoryMock.Setup(r => r.Reservation).Returns(_reservationRepositoryMock.Object);

        _sut = new GuestService(_repositoryMock.Object, Mock.Of<ILoggerManager>(), TestMapper.Create());
    }

    [Fact]
    public async Task GetGuestAsync_WhenGuestDoesNotExist_ThrowsGuestNotFoundException()
    {
        var guestId = Guid.NewGuid();
        _guestRepositoryMock.Setup(r => r.GetGuestAsync(guestId, false)).ReturnsAsync((Guest?)null);

        var act = () => _sut.GetGuestAsync(guestId, trackChanges: false);

        await act.Should().ThrowAsync<GuestNotFoundException>();
    }

    [Fact]
    public async Task CreateGuestAsync_WithValidData_SetsCreationDate()
    {
        Guest? createdGuest = null;
        _guestRepositoryMock.Setup(r => r.CreateGuest(It.IsAny<Guest>())).Callback<Guest>(g => createdGuest = g);
        var guest = new GuestForCreationDto
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            DocumentNumber = "ABC123456"
        };

        var result = await _sut.CreateGuestAsync(guest);

        createdGuest.Should().NotBeNull();
        createdGuest!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        result.LastName.Should().Be("Kowalski");
        _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteGuestAsync_WhenGuestHasReservations_ThrowsBadRequestException()
    {
        var guest = new Guest { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Nowak" };
        _guestRepositoryMock.Setup(r => r.GetGuestAsync(guest.Id, false)).ReturnsAsync(guest);
        _reservationRepositoryMock.Setup(r => r.AnyForGuestAsync(guest.Id)).ReturnsAsync(true);

        var act = () => _sut.DeleteGuestAsync(guest.Id, trackChanges: false);

        await act.Should().ThrowAsync<GuestHasReservationsBadRequestException>();
        _guestRepositoryMock.Verify(r => r.DeleteGuest(It.IsAny<Guest>()), Times.Never);
    }
}
