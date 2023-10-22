using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync(bool trackChanges);
    Task<RoomDto> GetRoomAsync(Guid roomId, bool trackChanges);
}
