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

    public async Task<RoomDto> CreateRoomAsync(RoomForCreationDto roomForCreation)
    {
        await CheckIfRoomNumberIsAvailable(roomForCreation.Number!);

        var room = _mapper.Map<Room>(roomForCreation);
        room.Status = RoomStatus.Available;

        _repository.Room.CreateRoom(room);
        await _repository.SaveAsync();

        return _mapper.Map<RoomDto>(room);
    }

    public async Task UpdateRoomAsync(Guid roomId, RoomForUpdateDto roomForUpdate, bool trackChanges)
    {
        var room = await GetRoomAndCheckIfItExists(roomId, trackChanges);
        await CheckIfRoomNumberIsAvailable(roomForUpdate.Number!, roomId);

        _mapper.Map(roomForUpdate, room);
        await _repository.SaveAsync();
    }

    public async Task DeleteRoomAsync(Guid roomId, bool trackChanges)
    {
        var room = await GetRoomAndCheckIfItExists(roomId, trackChanges);

        if (await _repository.Reservation.AnyForRoomAsync(roomId))
            throw new RoomHasReservationsBadRequestException(roomId);

        _repository.Room.DeleteRoom(room);
        await _repository.SaveAsync();
    }

    private async Task<Room> GetRoomAndCheckIfItExists(Guid roomId, bool trackChanges)
    {
        var room = await _repository.Room.GetRoomAsync(roomId, trackChanges);
        if (room is null)
            throw new RoomNotFoundException(roomId);

        return room;
    }

    private async Task CheckIfRoomNumberIsAvailable(string number, Guid? roomId = null)
    {
        if (await _repository.Room.RoomNumberExistsAsync(number, roomId))
            throw new RoomNumberAlreadyExistsBadRequestException(number);
    }
}
