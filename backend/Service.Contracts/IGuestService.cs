using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IGuestService
{
    Task<IEnumerable<GuestDto>> GetAllGuestsAsync(bool trackChanges);
    Task<GuestDto> GetGuestAsync(Guid guestId, bool trackChanges);
    Task<GuestDto> CreateGuestAsync(GuestForCreationDto guestForCreation);
    Task UpdateGuestAsync(Guid guestId, GuestForUpdateDto guestForUpdate, bool trackChanges);
    Task DeleteGuestAsync(Guid guestId, bool trackChanges);
}
