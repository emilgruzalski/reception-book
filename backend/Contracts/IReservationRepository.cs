using Entities.Models;

namespace Contracts;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges);
    Task<Reservation?> GetReservationAsync(Guid reservationId, bool trackChanges);
    Task<bool> HasOverlappingReservationAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate);
    void CreateReservation(Reservation reservation);
    void DeleteReservation(Reservation reservation);
}
