using Entities.Models;

namespace Contracts;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges);
    Task<Reservation?> GetReservationAsync(Guid reservationId, bool trackChanges);
    void CreateReservation(Reservation reservation);
    void DeleteReservation(Reservation reservation);
}
