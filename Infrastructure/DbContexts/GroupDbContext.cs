using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DbContexts;

public class GroupDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<TableGroup> TableGroups => Set<TableGroup>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TableGroup>()
            .HasKey(tg => new { tg.TableId, tg.GroupId });

        modelBuilder.Entity<TableGroup>()
            .HasOne(tg => tg.Group)
            .WithMany(g => g.TableGroups)
            .HasForeignKey(tg => tg.GroupId);
    }
}