using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class RoomService : IRoomService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public RoomService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync(bool trackChanges)
    {
        var rooms = await _repository.Room.GetAllRoomsAsync(trackChanges);

        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomDto?> GetRoomAsync(Guid roomId, bool trackChanges)
    {
        var room = await _repository.Room.GetRoomAsync(roomId, trackChanges);

        return room is null ? null : _mapper.Map<RoomDto>(room);
    }
}
