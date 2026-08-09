using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommitToNotes.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<DictionaryEntry>()
            .HasIndex(e => new { e.UserId, e.Key }).IsUnique();
    }
}
