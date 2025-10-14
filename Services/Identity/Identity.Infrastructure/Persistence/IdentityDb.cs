using Identity.Domain.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Identity.Infrastructure.Persistence;

public class IdentityDb : IdentityDbContext<IdentityUser>
{
    public IdentityDb(DbContextOptions<IdentityDb> options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.ApplyConfigurationsFromAssembly(typeof(IdentityDb).Assembly);
    }
}
