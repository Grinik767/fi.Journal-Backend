using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder
            .HasOne(g => g.Admin)
            .WithMany(a => a.GroupsAsAdmin)
            .HasForeignKey(g => g.AdminId);

        builder
            .HasMany(g => g.Users)
            .WithMany(u => u.Groups);

        builder
            .HasMany(g => g.Tables)
            .WithMany(t => t.Groups);
    }
}