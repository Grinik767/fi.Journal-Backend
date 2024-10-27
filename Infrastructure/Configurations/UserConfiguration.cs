using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(g => g.Id);
        
        builder
            .HasIndex(u => u.Email)
            .IsUnique();
        
        builder
            .HasMany(u => u.Groups)
            .WithMany(g => g.Users);

        builder
            .HasMany(u => u.GroupsAsAdmin)
            .WithOne(g => g.Admin)
            .HasForeignKey(g => g.AdminId);

        builder
            .HasMany(u => u.TablesAsAdmin)
            .WithOne(t => t.Admin)
            .HasForeignKey(t => t.AdminId);
    }
}