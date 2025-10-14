using Identity.Domain.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Token).IsRequired().HasMaxLength(256);
        e.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        e.HasIndex(x => x.Token).IsUnique();
        e.HasIndex(x => new { x.UserId, x.RevokedAt });
    }
}
