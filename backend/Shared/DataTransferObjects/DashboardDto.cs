namespace Shared.DataTransferObjects;

public record DashboardDto
{
    public DateTime Date { get; init; }
    public int TotalRooms { get; init; }
    public int AvailableRooms { get; init; }
    public int OccupiedRooms { get; init; }
    public int RoomsToClean { get; init; }
    public int OutOfServiceRooms { get; init; }
    public decimal OccupancyRate { get; init; }
    public int GuestsInHouse { get; init; }
    public IEnumerable<ReservationDto> ArrivalsToday { get; init; } = Enumerable.Empty<ReservationDto>();
    public IEnumerable<ReservationDto> DeparturesToday { get; init; } = Enumerable.Empty<ReservationDto>();
}
