using Entities.Models;

namespace Repository.Extensions;

public static class RepositoryGuestExtensions
{
    public static IQueryable<Guest> Search(this IQueryable<Guest> guests, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return guests;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return guests.Where(g =>
            g.FirstName!.ToLower().Contains(lowerCaseTerm) ||
            g.LastName!.ToLower().Contains(lowerCaseTerm) ||
            g.DocumentNumber!.ToLower().Contains(lowerCaseTerm) ||
            (g.Email != null && g.Email.ToLower().Contains(lowerCaseTerm)) ||
            (g.PhoneNumber != null && g.PhoneNumber.Contains(lowerCaseTerm)));
    }
}
