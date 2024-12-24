using System.Security.Authentication;
using Application;
using Application.Services.Users;
using Domain.Entities;
using FluentValidation;
using Infrastructure.PasswordHasher;
using Infrastructure.Repositories.Users;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Tests.Application.Services;

[TestFixture]
public class TestsUsersService
{
    private Mock<IUsersRepository> _repositoryMock;
    private Mock<IValidator<User>> _validatorMock;
    private Mock<IPasswordHasher> _passwordHasherMock;
    private Mock<IOptions<AuthOptions>> _authOptionsMock;
    private UsersService _usersService;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _validatorMock = new Mock<IValidator<User>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _authOptionsMock = new Mock<IOptions<AuthOptions>>();

        _authOptionsMock.Setup(o => o.Value).Returns(new AuthOptions
        {
            JwtSecretKey = "TestSecretKey",
            ExpireHours = 24
        });

        _usersService = new UsersService(
            _repositoryMock.Object,
            _validatorMock.Object,
            _passwordHasherMock.Object,
            _authOptionsMock.Object);
    }

    [Test]
    public void Register_ShouldThrowArgumentException_WhenPasswordIsInvalid()
    {
        const string name = "TestUser";
        const string email = "test@example.com";
        const string password = "123";

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _usersService.Register(name, email, password, CancellationToken.None));
    }

    [Test]
    public void Login_ShouldThrowInvalidCredentialException_WhenUserNotFound()
    {
        const string email = "test@example.com";
        const string password = "password123";

        _repositoryMock.Setup(r => r.GetByEmail(email, CancellationToken.None)).ReturnsAsync((User)null);

        Assert.ThrowsAsync<InvalidCredentialException>(async () =>
            await _usersService.Login(email, password, false, CancellationToken.None));
    }

    [Test]
    public void Login_ShouldThrowInvalidCredentialException_WhenPasswordIsInvalid()
    {
        const string email = "test@example.com";
        const string password = "password123";
        const string hashedPassword = "hashedPassword";
        var user = new User(Guid.NewGuid(), "TestUser", email, hashedPassword);

        _repositoryMock.Setup(r => r.GetByEmail(email, CancellationToken.None)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify(password, hashedPassword)).Returns(false);

        Assert.ThrowsAsync<InvalidCredentialException>(async () =>
            await _usersService.Login(email, password, false, CancellationToken.None));
    }
}