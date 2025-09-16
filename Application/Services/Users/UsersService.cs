using System.Security.Authentication;
using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.PasswordHasher;
using Infrastructure.Repositories.Users;
using Microsoft.Extensions.Options;

namespace Application.Services.Users;

public class UsersService(
    IUsersRepository repository,
    IValidator<User> validator,
    IPasswordHasher passwordHasher,
    IOptions<AuthOptions> authOptions) : BaseService<User>(repository, validator), IUsersService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<User> Register(string name, string email, string password, string studyGroup, CancellationToken ct)
    {
        ValidatePassword(password);
        return await Add(new User(Guid.NewGuid(), name.Trim(), email.Trim(), passwordHasher.Generate(password), studyGroup), ct);
    }

    public async Task<string> Login(string email, string password, bool remember, CancellationToken ct)
    {
        const string errorOutput = "Failed to login. Check credentials.";

        var user = await repository.GetByEmail(email, ct);
        if (user is null)
            throw new InvalidCredentialException(errorOutput);

        var result = passwordHasher.Verify(password, user.PasswordHash);
        if (!result)
            throw new InvalidCredentialException(errorOutput);

        return JwtProvider.GenerateToken(user.GenerateClaims(), _authOptions.JwtSecretKey,
            remember ? _authOptions.ExpireHoursRemember : _authOptions.ExpireHours);
    }

    public async Task<User> Update(Guid id, string? email, string? password, string? studyGroup, CancellationToken ct)
    {
        var user = await repository.GetById(id, ct);

        if (email is not null && email.Trim() != user.Email)
        {
            user.Email = email.Trim();
            user.IsEmailConfirmed = false;
        }

        if (studyGroup is not null)
            user.StudyGroup = studyGroup;

        if (password is null)
            return await base.Update(user, ct);

        ValidatePassword(password);
        user.PasswordHash = passwordHasher.Generate(password);

        return await base.Update(user, ct);
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length < 6)
            throw new ArgumentException("Password must be longer than 6 chars");
    }
}