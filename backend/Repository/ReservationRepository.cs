using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
{
    public ReservationRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .OrderByDescending(r => r.CheckInDate)
            .ToListAsync();

    public async Task<Reservation?> GetReservationAsync(Guid reservationId, bool trackChanges) =>
        await FindByCondition(r => r.Id.Equals(reservationId), trackChanges)
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .SingleOrDefaultAsync();

    public async Task<bool> HasOverlappingReservationAsync(Guid roomId, DateTime checkInDate,
        DateTime checkOutDate) =>
        await FindByCondition(r =>
                r.RoomId.Equals(roomId) &&
                r.Status != ReservationStatus.Cancelled &&
                r.CheckInDate < checkOutDate &&
                r.CheckOutDate > checkInDate,
                trackChanges: false)
            .AnyAsync();

    public void CreateReservation(Reservation reservation) => Create(reservation);

    public void DeleteReservation(Reservation reservation) => Delete(reservation);
}
