using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RoomRepository : RepositoryBase<Room>, IRoomRepository
{
    public RoomRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
            .OrderBy(r => r.Number)
            .ToListAsync();

    public async Task<Room?> GetRoomAsync(Guid roomId, bool trackChanges) =>
        await FindByCondition(r => r.Id.Equals(roomId), trackChanges)
            .SingleOrDefaultAsync();
}
