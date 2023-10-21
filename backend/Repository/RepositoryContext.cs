using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Room>? Rooms { get; set; }
    public DbSet<Guest>? Guests { get; set; }
    public DbSet<Reservation>? Reservations { get; set; }
}
