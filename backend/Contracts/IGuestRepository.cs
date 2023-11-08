using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IGuestRepository
{
    Task<PagedList<Guest>> GetAllGuestsAsync(GuestParameters guestParameters, bool trackChanges);
    Task<Guest?> GetGuestAsync(Guid guestId, bool trackChanges);
    void CreateGuest(Guest guest);
    void DeleteGuest(Guest guest);
}
