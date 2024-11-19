using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TablesConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Tables");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Url)
            .IsRequired();
        
        builder.Property(t => t.HeaderRow)
            .IsRequired();

        builder.Property(t => t.StudentColumn)
            .IsRequired();

        builder.Property(t => t.AdditionalData);

        builder.HasOne(t => t.Group)
            .WithMany(g => g.Tables)
            .HasForeignKey(t => t.GroupId)
            .IsRequired();
    }
}