using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class JournalDbContext(DbContextOptions<JournalDbContext> options) : DbContext(options)
{
    public DbSet<Group> Groups { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<Table> Tables { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupsConfiguration());
        modelBuilder.ApplyConfiguration(new TablesConfiguration());
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}