using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EmailConfirmationsConfiguration : IEntityTypeConfiguration<EmailConfirmation>
{
    public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.ToTable("EmailConfirmations");
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.User)
            .WithOne();
    }
}