using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;

namespace Repository;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new ReservationConfiguration());
    }

    public DbSet<Room>? Rooms { get; set; }
    public DbSet<Guest>? Guests { get; set; }
    public DbSet<Reservation>? Reservations { get; set; }
}
