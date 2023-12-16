using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

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

    public async Task<(IEnumerable<ReservationDto> reservations, MetaData metaData)> GetAllReservationsAsync(
        ReservationParameters reservationParameters, bool trackChanges)
    {
        if (!reservationParameters.ValidDateRange)
            throw new DateRangeBadRequestException();

        var reservationsWithMetaData = await _repository.Reservation.GetAllReservationsAsync(reservationParameters, trackChanges);

        var reservationsDto = _mapper.Map<IEnumerable<ReservationDto>>(reservationsWithMetaData);

        return (reservations: reservationsDto, metaData: reservationsWithMetaData.MetaData);
    }

    public async Task<ReservationDto> GetReservationAsync(Guid reservationId, bool trackChanges)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges);

        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto reservationForCreation)
    {
        var room = await GetRoomAndCheckIfItExists(reservationForCreation.RoomId!.Value);
        await CheckIfGuestExists(reservationForCreation.GuestId!.Value);
        await ValidateReservation(room, reservationForCreation);

        var reservation = _mapper.Map<Reservation>(reservationForCreation);
        reservation.CheckInDate = reservation.CheckInDate.Date;
        reservation.CheckOutDate = reservation.CheckOutDate.Date;
        reservation.TotalPrice = CalculateTotalPrice(room, reservation.CheckInDate, reservation.CheckOutDate);
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
        var room = await GetRoomAndCheckIfItExists(reservationForUpdate.RoomId!.Value);
        await CheckIfGuestExists(reservationForUpdate.GuestId!.Value);
        await ValidateReservation(room, reservationForUpdate);

        _mapper.Map(reservationForUpdate, reservation);
        reservation.CheckInDate = reservation.CheckInDate.Date;
        reservation.CheckOutDate = reservation.CheckOutDate.Date;
        reservation.TotalPrice = CalculateTotalPrice(room, reservation.CheckInDate, reservation.CheckOutDate);
        await _repository.SaveAsync();
    }

    public async Task CheckInAsync(Guid reservationId)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges: true);
        var room = reservation.Room!;

        if (reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidReservationStatusBadRequestException(reservation.Status, "check in");

        var today = DateTime.Today;
        if (today < reservation.CheckInDate || today >= reservation.CheckOutDate)
            throw new CheckInDateBadRequestException(reservation.CheckInDate);

        if (room.Status != RoomStatus.Available)
            throw new RoomNotReadyBadRequestException(room.Number!, room.Status);

        reservation.Status = ReservationStatus.CheckedIn;
        room.Status = RoomStatus.Occupied;

        await _repository.SaveAsync();
    }

    public async Task CheckOutAsync(Guid reservationId)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges: true);

        if (reservation.Status != ReservationStatus.CheckedIn)
            throw new InvalidReservationStatusBadRequestException(reservation.Status, "check out");

        reservation.Status = ReservationStatus.CheckedOut;
        reservation.Room!.Status = RoomStatus.Cleaning;

        await _repository.SaveAsync();
    }

    public async Task CancelAsync(Guid reservationId)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges: true);

        if (reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidReservationStatusBadRequestException(reservation.Status, "cancel");

        reservation.Status = ReservationStatus.Cancelled;

        await _repository.SaveAsync();
    }

    public async Task DeleteReservationAsync(Guid reservationId, bool trackChanges)
    {
        var reservation = await GetReservationAndCheckIfItExists(reservationId, trackChanges);

        _repository.Reservation.DeleteReservation(reservation);
        await _repository.SaveAsync();
    }

    private async Task ValidateReservation(Room room, ReservationForManipulationDto reservation)
    {
        var checkInDate = reservation.CheckInDate!.Value.Date;
        var checkOutDate = reservation.CheckOutDate!.Value.Date;

        if (checkOutDate <= checkInDate)
            throw new ReservationDatesBadRequestException();

        if (reservation.NumberOfGuests > room.Capacity)
            throw new RoomCapacityExceededBadRequestException(room.Number!, room.Capacity);

        if (await _repository.Reservation.HasOverlappingReservationAsync(room.Id, checkInDate, checkOutDate))
            throw new RoomNotAvailableBadRequestException(room.Number!);
    }

    private static decimal CalculateTotalPrice(Room room, DateTime checkInDate, DateTime checkOutDate)
    {
        var nights = (checkOutDate.Date - checkInDate.Date).Days;

        return nights * room.PricePerNight;
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
