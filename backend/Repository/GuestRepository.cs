using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class GuestRepository : RepositoryBase<Guest>, IGuestRepository
{
    public GuestRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Guest>> GetAllGuestsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
            .OrderBy(g => g.LastName)
            .ThenBy(g => g.FirstName)
            .ToListAsync();

    public async Task<Guest?> GetGuestAsync(Guid guestId, bool trackChanges) =>
        await FindByCondition(g => g.Id.Equals(guestId), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateGuest(Guest guest) => Create(guest);

    public void DeleteGuest(Guest guest) => Delete(guest);
}
