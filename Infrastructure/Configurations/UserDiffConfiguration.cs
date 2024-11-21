using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserDiffConfiguration : IEntityTypeConfiguration<UserDiff>
{
    public void Configure(EntityTypeBuilder<UserDiff> builder)
    {
        builder.ToTable("UserDiff");
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.Diff);

        builder.HasOne(d => d.Table)
            .WithMany(t => t.UserDiffs)
            .HasForeignKey(d => d.TableId)
            .IsRequired();

        builder.HasOne(d => d.User)
            .WithMany(u => u.UserDiffs)
            .HasForeignKey(d => d.UserId)
            .IsRequired();
    }
}