using Entities.Models;

namespace Contracts;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllRoomsAsync(bool trackChanges);
    Task<Room?> GetRoomAsync(Guid roomId, bool trackChanges);
}
