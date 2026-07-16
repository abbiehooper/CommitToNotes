using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DictionaryApp.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<DictionaryEntry>()
            .HasIndex(e => new { e.TeamId, e.Key }).IsUnique();
    }
}
