using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class JournalDbContext: DbContext
{
    public DbSet<Group> Groups { get; set; }

    public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}