using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface IRoomService
{
    Task<(IEnumerable<RoomDto> rooms, MetaData metaData)> GetAllRoomsAsync(RoomParameters roomParameters,
        bool trackChanges);
    Task<RoomDto> GetRoomAsync(Guid roomId, bool trackChanges);
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(RoomAvailabilityParameters parameters, bool trackChanges);
    Task<RoomDto> CreateRoomAsync(RoomForCreationDto roomForCreation);
    Task UpdateRoomAsync(Guid roomId, RoomForUpdateDto roomForUpdate, bool trackChanges);
    Task DeleteRoomAsync(Guid roomId, bool trackChanges);
}
