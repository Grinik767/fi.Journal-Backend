using System.Globalization;
using System.Security.Authentication;
using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.PasswordHasher;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Users;
using Microsoft.Extensions.Options;
using GoogleSheetParser.Parser;
using GoogleSheetParser.GoogleSheet;

namespace Application.Services.Users;

public class UsersService(
    IUsersRepository repository,
    IValidator<User> validator,
    IPasswordHasher passwordHasher,
    IOptions<AuthOptions> authOptions,
    IRepository<UserDiff> userDiffRepository,
    ExcelParser excelParser,
    GoogleSheetManager googleSheetManager,
    IRepository<Table> tableRepository) : BaseService<User>(repository, validator), IUsersService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<User> Register(string name, string email, string password, CancellationToken ct) =>
        await Add(new User(Guid.NewGuid(), name, email, passwordHasher.Generate(password)), ct);

    public async Task<string> Login(string email, string password, CancellationToken ct)
    {
        var user = await repository.GetByEmail(email, ct);
        if (user is null)
            throw new InvalidCredentialException("Failed to login. Check credentials.");

        var result = passwordHasher.Verify(password, user.PasswordHash);
        if (!result)
            throw new InvalidCredentialException("Failed to login. Check credentials.");

        return JwtProvider.GenerateToken(user.GenerateClaims(), _authOptions.JwtSecretKey, _authOptions.ExpireHours);
    }

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await repository.GetById(id, ct);

        user.Email = email ?? user.Email;
        if (password is not null)
            user.PasswordHash = passwordHasher.Generate(password);

        return await base.Update(user, ct);
    }

    public async Task<List<Group>> GetGroups(Guid id, CancellationToken ct)
    {
        var user = await repository.GetById(id, ct);
        return user.Groups.ToList();
    }

    public async Task<List<Group>> GetGroupsAsAdmin(Guid id, CancellationToken ct)
    {
        var user = await repository.GetById(id, ct);
        return user.GroupsAsAdmin.ToList();
    }
}