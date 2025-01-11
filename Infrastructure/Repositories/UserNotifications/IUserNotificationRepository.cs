using Domain.Entities;

namespace Infrastructure.Repositories.UserNotifications;

public interface IUserNotificationRepository
{
    Task Add(UserNotification entity, CancellationToken ct);
}