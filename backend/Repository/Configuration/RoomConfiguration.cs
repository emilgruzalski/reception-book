using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasIndex(r => r.Number).IsUnique();

        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasData(
            CreateRoom("3d490a70-94ce-4d15-9494-5248280c2ce3", "101", 1, RoomType.Single, 1, 180m, "Single room with a city view."),
            CreateRoom("8c2b6f3a-1b7e-4f6d-9a43-2f4d9a1c7e01", "102", 1, RoomType.Double, 2, 260m, "Double room with a queen-size bed."),
            CreateRoom("c9d4c053-49b6-410c-bc78-2d54a9991870", "103", 1, RoomType.Twin, 2, 250m, "Twin room with two single beds."),
            CreateRoom("5f1d2e7b-6c3a-4b8e-9f0a-7d2c1b3e4a56", "104", 1, RoomType.Single, 1, 170m, "Single room, accessible for disabled guests."),
            CreateRoom("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d", "201", 2, RoomType.Double, 2, 280m, "Double room with a balcony."),
            CreateRoom("0f8e7d6c-5b4a-4392-8170-6e5d4c3b2a19", "202", 2, RoomType.Triple, 3, 340m, "Triple room for families."),
            CreateRoom("7e6d5c4b-3a29-4180-9f7e-6d5c4b3a2918", "203", 2, RoomType.Twin, 2, 250m, "Twin room with a garden view."),
            CreateRoom("2b3c4d5e-6f70-4812-9a3b-4c5d6e7f8091", "301", 3, RoomType.Suite, 2, 520m, "Suite with a living room and a bathtub."),
            CreateRoom("9a8b7c6d-5e4f-4321-8a9b-0c1d2e3f4a5b", "302", 3, RoomType.Apartment, 4, 640m, "Apartment with a kitchenette and two bedrooms."));
    }

    private static Room CreateRoom(string id, string number, int floor, RoomType type, int capacity,
        decimal pricePerNight, string description) =>
        new()
        {
            Id = new Guid(id),
            Number = number,
            Floor = floor,
            Type = type,
            Capacity = capacity,
            PricePerNight = pricePerNight,
            Status = RoomStatus.Available,
            Description = description
        };
}
