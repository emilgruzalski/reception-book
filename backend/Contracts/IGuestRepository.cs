using Entities.Models;

namespace Contracts;

public interface IGuestRepository
{
    Task<IEnumerable<Guest>> GetAllGuestsAsync(bool trackChanges);
    Task<Guest?> GetGuestAsync(Guid guestId, bool trackChanges);
    void CreateGuest(Guest guest);
    void DeleteGuest(Guest guest);
}
