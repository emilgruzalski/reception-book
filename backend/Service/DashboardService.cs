using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class DashboardService : IDashboardService
{
    private const decimal PercentageFactor = 100m;
    private const int OccupancyRateDecimals = 1;

    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public DashboardService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.Today;

        var roomStatusCounts = await _repository.Room.GetRoomStatusCountsAsync();
        var arrivals = await _repository.Reservation.GetArrivalsAsync(today);
        var departures = await _repository.Reservation.GetDeparturesAsync(today);
        var guestsInHouse = await _repository.Reservation.GetGuestsInHouseCountAsync();

        var totalRooms = roomStatusCounts.Values.Sum();
        var occupiedRooms = roomStatusCounts.GetValueOrDefault(RoomStatus.Occupied);

        return new DashboardDto
        {
            Date = today,
            TotalRooms = totalRooms,
            AvailableRooms = roomStatusCounts.GetValueOrDefault(RoomStatus.Available),
            OccupiedRooms = occupiedRooms,
            RoomsToClean = roomStatusCounts.GetValueOrDefault(RoomStatus.Cleaning),
            OutOfServiceRooms = roomStatusCounts.GetValueOrDefault(RoomStatus.OutOfService),
            OccupancyRate = totalRooms == 0
                ? 0
                : Math.Round(occupiedRooms * PercentageFactor / totalRooms, OccupancyRateDecimals),
            GuestsInHouse = guestsInHouse,
            ArrivalsToday = _mapper.Map<IEnumerable<ReservationDto>>(arrivals),
            DeparturesToday = _mapper.Map<IEnumerable<ReservationDto>>(departures)
        };
    }
}
