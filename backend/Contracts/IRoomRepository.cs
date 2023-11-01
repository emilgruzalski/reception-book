using Entities.Models;

namespace Contracts;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetAllRoomsAsync(bool trackChanges);
    Task<Room?> GetRoomAsync(Guid roomId, bool trackChanges);
    Task<bool> RoomNumberExistsAsync(string number, Guid? excludedRoomId = null);
    void CreateRoom(Room room);
    void DeleteRoom(Room room);
}
