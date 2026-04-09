using Microsoft.EntityFrameworkCore;
using FCOldBoysTeamSelector.Models;

namespace FCOldBoysTeamSelector.Data;

public class AppDbContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<MatchRecord> MatchRecords => Set<MatchRecord>();
    public DbSet<MatchPlayerEntry> MatchPlayerEntries => Set<MatchPlayerEntry>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Speed).IsRequired();
            entity.Property(p => p.Stamina).IsRequired();
            entity.Property(p => p.Defense).IsRequired();
            entity.Property(p => p.Attack).IsRequired();
            entity.Property(p => p.Strength).IsRequired();
        });

        modelBuilder.Entity<MatchRecord>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasMany(m => m.PlayerEntries)
                  .WithOne(e => e.MatchRecord)
                  .HasForeignKey(e => e.MatchRecordId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchPlayerEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
