using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations;

public class UserNotificationConfiguration: IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.HasOne(n => n.User)
            .WithMany(u => u.UserNotifications)
            .HasForeignKey(n => n.UserId)
            .IsRequired();

        builder.HasOne(n => n.UserDiff)
            .WithOne(d => d.UserNotification)
            .HasForeignKey<UserNotification>(n => n.UserDiffId)
            .IsRequired();
    }
}