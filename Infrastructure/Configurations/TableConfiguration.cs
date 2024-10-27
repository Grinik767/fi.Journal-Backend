using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(g => g.Id);

        builder
            .HasOne(t => t.Admin)
            .WithMany(a => a.TablesAsAdmin)
            .HasForeignKey(t => t.AdminId);

        builder
            .HasMany(t => t.Groups)
            .WithMany(g => g.Tables);
    }
}