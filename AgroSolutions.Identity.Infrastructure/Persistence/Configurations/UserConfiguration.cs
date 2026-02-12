using AgroSolutions.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroSolutions.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).HasMaxLength(60);
        builder.Property(u => u.Name).HasMaxLength(60);
        builder.Property(u => u.Password).HasMaxLength(60);

        // Login details of a demo user
        builder.HasData([
            new(1, "Demo User", "demo@gmail.com", "$2a$12$2Dj1BaOnV8X0ej7U0KIOjOneac1OOcv9L8rhoIbOgSiafuPPnwQIi"), // Password: password1234$$
        ]);
    }
}
