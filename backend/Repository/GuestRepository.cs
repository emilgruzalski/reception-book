using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository;

public class GuestRepository : RepositoryBase<Guest>, IGuestRepository
{
    public GuestRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Guest>> GetAllGuestsAsync(GuestParameters guestParameters, bool trackChanges) =>
        await FindAll(trackChanges)
            .Search(guestParameters.SearchTerm)
            .Sort(guestParameters.OrderBy)
            .ToPagedListAsync(guestParameters);

    public async Task<Guest?> GetGuestAsync(Guid guestId, bool trackChanges) =>
        await FindByCondition(g => g.Id.Equals(guestId), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateGuest(Guest guest) => Create(guest);

    public void DeleteGuest(Guest guest) => Delete(guest);
}
