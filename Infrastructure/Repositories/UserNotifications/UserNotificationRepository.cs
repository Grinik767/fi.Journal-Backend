using Domain.Entities;

namespace Infrastructure.Repositories.UserNotifications;

public class UserNotificationRepository(JournalDbContext dbContext) : IUserNotificationRepository
{
    public  async Task Add(UserNotification userNotification, CancellationToken ct)
    {
        await dbContext.UserNotifications.AddAsync(userNotification, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}