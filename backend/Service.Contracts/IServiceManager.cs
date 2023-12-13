namespace Service.Contracts;

public interface IServiceManager
{
    IRoomService RoomService { get; }
    IGuestService GuestService { get; }
    IReservationService ReservationService { get; }
    IAuthenticationService AuthenticationService { get; }
    IUserService UserService { get; }
}
