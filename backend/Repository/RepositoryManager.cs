using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IRoomRepository> _roomRepository;
    private readonly Lazy<IGuestRepository> _guestRepository;
    private readonly Lazy<IReservationRepository> _reservationRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _roomRepository = new Lazy<IRoomRepository>(() => new RoomRepository(repositoryContext));
        _guestRepository = new Lazy<IGuestRepository>(() => new GuestRepository(repositoryContext));
        _reservationRepository = new Lazy<IReservationRepository>(() => new ReservationRepository(repositoryContext));
    }

    public IRoomRepository Room => _roomRepository.Value;
    public IGuestRepository Guest => _guestRepository.Value;
    public IReservationRepository Reservation => _reservationRepository.Value;

    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}
