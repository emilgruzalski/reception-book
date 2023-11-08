using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source,
        RequestParameters parameters)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, parameters.PageNumber, parameters.PageSize);
    }
}
