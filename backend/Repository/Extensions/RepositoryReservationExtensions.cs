using System.Linq.Dynamic.Core;
using Entities.Models;
using Repository.Extensions.Utility;
using Shared.RequestFeatures;

namespace Repository.Extensions;

public static class RepositoryReservationExtensions
{
    public static IQueryable<Reservation> FilterReservations(this IQueryable<Reservation> reservations,
        ReservationParameters parameters) =>
        reservations.Where(r =>
            (!parameters.Status.HasValue || r.Status == parameters.Status) &&
            (!parameters.RoomId.HasValue || r.RoomId == parameters.RoomId) &&
            (!parameters.GuestId.HasValue || r.GuestId == parameters.GuestId) &&
            (!parameters.FromDate.HasValue || r.CheckOutDate >= parameters.FromDate.Value.Date) &&
            (!parameters.ToDate.HasValue || r.CheckInDate <= parameters.ToDate.Value.Date));

    public static IQueryable<Reservation> Search(this IQueryable<Reservation> reservations, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return reservations;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return reservations.Where(r =>
            r.Room!.Number!.ToLower().Contains(lowerCaseTerm) ||
            r.Guest!.LastName!.ToLower().Contains(lowerCaseTerm) ||
            r.Guest.FirstName!.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Reservation> Sort(this IQueryable<Reservation> reservations, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return reservations.OrderByDescending(r => r.CheckInDate);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Reservation>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return reservations.OrderByDescending(r => r.CheckInDate);

        return reservations.OrderBy(orderQuery);
    }
}
