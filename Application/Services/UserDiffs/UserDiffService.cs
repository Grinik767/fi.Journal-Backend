using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Users;

namespace Application.Services.UserDiffs;

public class UserDiffService(
    IRepository<UserDiff> userDiffRepository,
    IUsersRepository userRepository,
    IValidator<UserDiff> validator) : BaseService<UserDiff>(userDiffRepository, validator), IUserDiffsService
{
    private readonly IRepository<UserDiff> _userDiffRepository = userDiffRepository;

    public async Task<List<UserDiff>> GetDiffForUser(Guid userId,
        CancellationToken ct)
    {
        var user = await userRepository.GetById(userId, ct);
        var usersDiff = await _userDiffRepository.GetAll(ct);
        return usersDiff.Where(d => d.User.Id == user.Id)
            .OrderByDescending(d => d.UpdateTime)
            .ToList();
    }
}