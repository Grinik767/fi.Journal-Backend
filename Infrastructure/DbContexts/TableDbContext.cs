using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DbContexts;

public class TableDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Table> Tables => Set<Table>();
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
            .HasOne(tg => tg.Table)
            .WithMany(t => t.TableGroups)
            .HasForeignKey(tg => tg.TableId);
    }
}