using Contracts;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IRoomService> _roomService;
    private readonly Lazy<IGuestService> _guestService;
    private readonly Lazy<IReservationService> _reservationService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger)
    {
        _roomService = new Lazy<IRoomService>(() => new RoomService(repositoryManager, logger));
        _guestService = new Lazy<IGuestService>(() => new GuestService(repositoryManager, logger));
        _reservationService = new Lazy<IReservationService>(() => new ReservationService(repositoryManager, logger));
    }

    public IRoomService RoomService => _roomService.Value;
    public IGuestService GuestService => _guestService.Value;
    public IReservationService ReservationService => _reservationService.Value;
}
