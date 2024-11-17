using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired();
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.OwnsMany(u => u.UserDiffs, b =>
        {
            b.Property(x => x.TableId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.UpdateTime).HasColumnType("timestamp").IsRequired();
            b.Property(x => x.Diff)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, double>>(v, new JsonSerializerOptions())!);
        });
    }
}