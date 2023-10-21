using Contracts;
using Entities.Models;

namespace Repository;

public class GuestRepository : RepositoryBase<Guest>, IGuestRepository
{
    public GuestRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }
}
