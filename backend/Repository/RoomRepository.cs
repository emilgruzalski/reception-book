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

    public async Task<bool> RoomNumberExistsAsync(string number, Guid? excludedRoomId = null) =>
        await FindByCondition(r => r.Number == number && r.Id != excludedRoomId, trackChanges: false)
            .AnyAsync();

    public void CreateRoom(Room room) => Create(room);

    public void DeleteRoom(Room room) => Delete(room);
}
