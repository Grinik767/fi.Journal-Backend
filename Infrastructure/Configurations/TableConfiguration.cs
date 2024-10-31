using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Tables");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Url)
            .IsRequired();

        builder.HasOne(t => t.Admin)
            .WithMany(u => u.TablesAsAdmin)
            .HasForeignKey(t => t.AdminId)
            .IsRequired();
        
        builder.HasMany(t => t.Groups)
            .WithMany(g => g.Tables);
    }
}