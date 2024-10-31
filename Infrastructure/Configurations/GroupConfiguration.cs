using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");
        builder.HasKey(g => g.Id);
        
        builder.HasIndex(g => g.Name).IsUnique();
        builder.Property(g => g.Name)
            .IsRequired();
        
        builder.HasOne(g => g.Admin)
            .WithMany(u => u.GroupsAsAdmin)
            .HasForeignKey(g => g.AdminId)
            .IsRequired();

        builder.HasMany(g => g.Users)
            .WithMany(u => u.Groups);
    }
}