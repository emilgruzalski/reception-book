using Contracts;
using Service.Contracts;

namespace Service;

internal sealed class GuestService : IGuestService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public GuestService(IRepositoryManager repository, ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
