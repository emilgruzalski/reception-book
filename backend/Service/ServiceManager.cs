using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IRoomService> _roomService;
    private readonly Lazy<IGuestService> _guestService;
    private readonly Lazy<IReservationService> _reservationService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper,
        UserManager<User> userManager, IOptions<JwtConfiguration> configuration)
    {
        _roomService = new Lazy<IRoomService>(() => new RoomService(repositoryManager, logger, mapper));
        _guestService = new Lazy<IGuestService>(() => new GuestService(repositoryManager, logger, mapper));
        _reservationService = new Lazy<IReservationService>(() => new ReservationService(repositoryManager, logger, mapper));
        _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(logger, mapper, userManager, configuration));
    }

    public IRoomService RoomService => _roomService.Value;
    public IGuestService GuestService => _guestService.Value;
    public IReservationService ReservationService => _reservationService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
