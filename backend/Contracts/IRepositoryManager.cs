namespace Contracts;

public interface IRepositoryManager
{
    IRoomRepository Room { get; }
    IGuestRepository Guest { get; }
    IReservationRepository Reservation { get; }
    Task SaveAsync();
}
