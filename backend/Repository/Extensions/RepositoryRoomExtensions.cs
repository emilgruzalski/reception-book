using System.Linq.Dynamic.Core;
using Entities.Models;
using Repository.Extensions.Utility;
using Shared.RequestFeatures;

namespace Repository.Extensions;

public static class RepositoryRoomExtensions
{
    public static IQueryable<Room> FilterRooms(this IQueryable<Room> rooms, RoomParameters parameters) =>
        rooms.Where(r =>
            (!parameters.Type.HasValue || r.Type == parameters.Type) &&
            (!parameters.Status.HasValue || r.Status == parameters.Status) &&
            (!parameters.MinPrice.HasValue || r.PricePerNight >= parameters.MinPrice) &&
            (!parameters.MaxPrice.HasValue || r.PricePerNight <= parameters.MaxPrice) &&
            (!parameters.MinCapacity.HasValue || r.Capacity >= parameters.MinCapacity));

    public static IQueryable<Room> Search(this IQueryable<Room> rooms, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return rooms;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return rooms.Where(r =>
            r.Number!.ToLower().Contains(lowerCaseTerm) ||
            (r.Description != null && r.Description.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Room> Sort(this IQueryable<Room> rooms, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return rooms.OrderBy(r => r.Number);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Room>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return rooms.OrderBy(r => r.Number);

        return rooms.OrderBy(orderQuery);
    }
}
