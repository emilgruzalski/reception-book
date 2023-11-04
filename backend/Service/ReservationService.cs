using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class ReservationService : IReservationService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public ReservationService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool trackChanges)
    {
        var reservations = await _repository.Reservation.GetAllReservationsAsync(trackChanges);

        return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
    }

    public async Task<ReservationDto> GetReservationAsync(Guid reservationId, bool trackChanges)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges);

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto reservationForCreation)
    {
        await GetRoomAndCheckIfItExists(reservationForCreation.RoomId!.Value);
        await CheckIfGuestExists(reservationForCreation.GuestId!.Value);

        var reservation = _mapper.Map<Reservation>(reservationForCreation);
        reservation.Status = ReservationStatus.Confirmed;
        reservation.CreatedAt = DateTime.UtcNow;

        _repository.Reservation.CreateReservation(reservation);
        await _repository.SaveAsync();

        return await GetReservationAsync(reservation.Id, trackChanges: false);
    }

    public async Task UpdateReservationAsync(Guid reservationId, ReservationForUpdateDto reservationForUpdate,
        bool trackChanges)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges);
        await GetRoomAndCheckIfItExists(reservationForUpdate.RoomId!.Value);
        await CheckIfGuestExists(reservationForUpdate.GuestId!.Value);

        _mapper.Map(reservationForUpdate, reservation);
        await _repository.SaveAsync();
    }

    public async Task DeleteReservationAsync(Guid reservationId, bool trackChanges)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges);

        _repository.Reservation.DeleteReservation(reservation);
        await _repository.SaveAsync();
    }

    private async Task<Reservation> GetReservationAndCheckIfItExists(Guid reservationId, bool trackChanges)
    {
        var reservation = await _repository.Reservation.GetReservationAsync(reservationId, trackChanges);
        if (reservation is null)
            throw new ReservationNotFoundException(reservationId);

        return reservation;
    }

    private async Task<Room> GetRoomAndCheckIfItExists(Guid roomId)
    {
        var room = await _repository.Room.GetRoomAsync(roomId, trackChanges: false);
        if (room is null)
            throw new RoomNotFoundException(roomId);

        return room;
    }

    private async Task CheckIfGuestExists(Guid guestId)
    {
        var guest = await _repository.Guest.GetGuestAsync(guestId, trackChanges: false);
        if (guest is null)
            throw new GuestNotFoundException(guestId);
    }
}
