using Application.Services.UserDiffs;
using Domain.Entities;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

public class UserDiffsServiceTests
{
    private Mock<IUserDiffsRepository> _userDiffsRepositoryMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private UserDiffsService _userDiffsService;

    [SetUp]
    public void SetUp()
    {
        _userDiffsRepositoryMock = new Mock<IUserDiffsRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _userDiffsService = new UserDiffsService(
            _userDiffsRepositoryMock.Object,
            _usersRepositoryMock.Object
        );
    }

    [Test]
    public async Task GetDiffsForUser_ShouldReturnUserDiffs()
    {
        var userId = Guid.NewGuid();
        var userDiffs = new List<UserDiff>
        {
            new (Guid.NewGuid(), Guid.NewGuid(), userId, new Dictionary<string, string>())
        };

        _usersRepositoryMock
            .Setup(repo => repo.GetById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(userId, "Test User", "123@gamil.com", "hash"));

        _userDiffsRepositoryMock
            .Setup(repo => repo.GetAllByUser(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDiffs);
            
        var result = await _userDiffsService.GetDiffsForUser(userId, CancellationToken.None);
            
        Assert.That(userDiffs, Is.EqualTo(result));
        _usersRepositoryMock.Verify(repo => repo.GetById(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userDiffsRepositoryMock.Verify(repo => repo.GetAllByUser(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}