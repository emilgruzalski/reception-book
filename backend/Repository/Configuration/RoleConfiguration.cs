using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "c7b013f0-5201-4317-abd8-c211f91b7330",
                Name = UserRoles.Administrator,
                NormalizedName = UserRoles.Administrator.ToUpperInvariant(),
                ConcurrencyStamp = "1f3a3c5e-8b8f-4d2a-9a43-6c1e2f0d7b11"
            },
            new IdentityRole
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Name = UserRoles.Receptionist,
                NormalizedName = UserRoles.Receptionist.ToUpperInvariant(),
                ConcurrencyStamp = "4b2d9e7a-6c3f-41a8-b5e0-9d8c7f6a5e42"
            });
    }
}
