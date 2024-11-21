using System.Globalization;
using System.Security.Cryptography;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Users;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services.UserDiffs;

public class UserDiffService(
    IRepository<UserDiff> userDiffRepository,
    IUsersRepository userRepository,
    IValidator<UserDiff> validator) : BaseService<UserDiff>(userDiffRepository, validator), IUserDiffsService
{
    private readonly IRepository<UserDiff> _userDiffRepository = userDiffRepository;
    
    public async Task<UserDiff> Add(Dictionary<string, double> diff, Guid userId, Guid tableId, CancellationToken ct)
    {
        var user = await userRepository.GetById(userId, ct);
        var convertedDiff = diff.ToDictionary(key => key.Key, val => val.Value.ToString(CultureInfo.InvariantCulture));
        return await base.Add(new UserDiff(Guid.NewGuid(), tableId, userId, convertedDiff), ct);
    }

    public async Task<List<UserDiff>> GetDiffForUser(Guid userId,
        CancellationToken ct)
    {
        var user = await userRepository.GetById(userId, ct);
        var x = await _userDiffRepository.GetAll(ct);
        return x.Where(d => d.User.Id == userId)
            .OrderByDescending(d => d.UpdateTime)
            .ToList();
    }
}