using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IRoomService> _roomService;
    private readonly Lazy<IGuestService> _guestService;
    private readonly Lazy<IReservationService> _reservationService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _roomService = new Lazy<IRoomService>(() => new RoomService(repositoryManager, logger, mapper));
        _guestService = new Lazy<IGuestService>(() => new GuestService(repositoryManager, logger, mapper));
        _reservationService = new Lazy<IReservationService>(() => new ReservationService(repositoryManager, logger, mapper));
    }

    public IRoomService RoomService => _roomService.Value;
    public IGuestService GuestService => _guestService.Value;
    public IReservationService ReservationService => _reservationService.Value;
}
