using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository;

public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
{
    public ReservationRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Reservation>> GetAllReservationsAsync(ReservationParameters reservationParameters, bool trackChanges) =>
        await FindAll(trackChanges)
            .FilterReservations(reservationParameters)
            .Search(reservationParameters.SearchTerm)
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .Sort(reservationParameters.OrderBy)
            .ToPagedListAsync(reservationParameters);

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

    public async Task<IEnumerable<Reservation>> GetArrivalsAsync(DateTime date) =>
        await FindByCondition(r =>
                r.CheckInDate == date.Date &&
                (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn),
                trackChanges: false)
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .OrderBy(r => r.Room!.Number)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetDeparturesAsync(DateTime date) =>
        await FindByCondition(r =>
                r.CheckOutDate == date.Date &&
                (r.Status == ReservationStatus.CheckedIn || r.Status == ReservationStatus.CheckedOut),
                trackChanges: false)
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .OrderBy(r => r.Room!.Number)
            .ToListAsync();

    public async Task<int> GetGuestsInHouseCountAsync() =>
        await FindByCondition(r => r.Status == ReservationStatus.CheckedIn, trackChanges: false)
            .SumAsync(r => r.NumberOfGuests);

    public async Task<bool> AnyForRoomAsync(Guid roomId) =>
        await FindByCondition(r => r.RoomId.Equals(roomId), trackChanges: false).AnyAsync();

    public async Task<bool> AnyForGuestAsync(Guid guestId) =>
        await FindByCondition(r => r.GuestId.Equals(guestId), trackChanges: false).AnyAsync();

    public void CreateReservation(Reservation reservation) => Create(reservation);

    public void DeleteReservation(Reservation reservation) => Delete(reservation);
}
