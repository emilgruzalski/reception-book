using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IRoomRepository
{
    Task<PagedList<Room>> GetAllRoomsAsync(RoomParameters roomParameters, bool trackChanges);
    Task<Room?> GetRoomAsync(Guid roomId, bool trackChanges);
    Task<bool> RoomNumberExistsAsync(string number, Guid? excludedRoomId = null);
    void CreateRoom(Room room);
    void DeleteRoom(Room room);
}
