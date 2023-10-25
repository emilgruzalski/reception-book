using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync(bool trackChanges);
    Task<RoomDto> GetRoomAsync(Guid roomId, bool trackChanges);
    Task<RoomDto> CreateRoomAsync(RoomForCreationDto roomForCreation);
    Task UpdateRoomAsync(Guid roomId, RoomForUpdateDto roomForUpdate, bool trackChanges);
    Task DeleteRoomAsync(Guid roomId, bool trackChanges);
}
