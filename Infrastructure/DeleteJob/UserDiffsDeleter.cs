using Quartz;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;

namespace Infrastructure.DeleteJob;

public class UserDiffsDeleter(IUsersRepository usersRepository, IUserDiffsRepository userDiffsRepository)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var ct = new CancellationToken();
        var users = await usersRepository.GetAll(ct);
        foreach (var user in users)
        {
            var oldDiffs = await userDiffsRepository.GetOldDiffsByUser(user.Id, ct);
            foreach (var oldDiff in oldDiffs)
                await userDiffsRepository.Delete(oldDiff.Id, ct);
        }
    }
}