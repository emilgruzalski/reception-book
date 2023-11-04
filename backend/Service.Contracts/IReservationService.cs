using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool trackChanges);
    Task<ReservationDto> GetReservationAsync(Guid reservationId, bool trackChanges);
    Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto reservationForCreation);
    Task UpdateReservationAsync(Guid reservationId, ReservationForUpdateDto reservationForUpdate, bool trackChanges);
    Task DeleteReservationAsync(Guid reservationId, bool trackChanges);
}
