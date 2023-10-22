using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
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

    public async Task<RoomDto> GetRoomAsync(Guid roomId, bool trackChanges)
    {
        var room = await GetRoomAndCheckIfItExists(roomId, trackChanges);

        return _mapper.Map<RoomDto>(room);
    }

    private async Task<Room> GetRoomAndCheckIfItExists(Guid roomId, bool trackChanges)
    {
        var room = await _repository.Room.GetRoomAsync(roomId, trackChanges);
        if (room is null)
            throw new RoomNotFoundException(roomId);

        return room;
    }
}
