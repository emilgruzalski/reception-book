using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IReservationRepository
{
    Task<PagedList<Reservation>> GetAllReservationsAsync(ReservationParameters reservationParameters, bool trackChanges);
    Task<Reservation?> GetReservationAsync(Guid reservationId, bool trackChanges);
    Task<bool> HasOverlappingReservationAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate,
        Guid? excludedReservationId = null);
    Task<IEnumerable<Reservation>> GetArrivalsAsync(DateTime date);
    Task<IEnumerable<Reservation>> GetDeparturesAsync(DateTime date);
    Task<int> GetGuestsInHouseCountAsync();
    Task<bool> AnyForRoomAsync(Guid roomId);
    Task<bool> AnyForGuestAsync(Guid guestId);
    void CreateReservation(Reservation reservation);
    void DeleteReservation(Reservation reservation);
}
