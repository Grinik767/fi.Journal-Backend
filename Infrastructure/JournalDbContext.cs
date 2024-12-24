using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class JournalDbContext(DbContextOptions<JournalDbContext> options) : DbContext(options)
{
    public DbSet<Group> Groups { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<Table> Tables { get; init; }
    public DbSet<UserDiff> UserDiffs { get; init; }
    
    public DbSet<SuperAdmin> SuperAdmins { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupsConfiguration());
        modelBuilder.ApplyConfiguration(new TablesConfiguration());
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        modelBuilder.ApplyConfiguration(new UserDiffConfiguration());
        modelBuilder.ApplyConfiguration(new SuperAdminConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}