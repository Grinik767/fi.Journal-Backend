using Application.Services.Groups;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;
using Moq;
using NUnit.Framework;

namespace Tests.Application.Services;

[TestFixture]
public class TestsGroupsService
{
    private Mock<IGroupsRepository> _groupsRepositoryMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<IUserDiffsRepository> _userDiffRepositoryMock;
    private Mock<IValidator<Group>> _validatorMock;
    private GroupsService _groupsService;

    [SetUp]
    public void SetUp()
    {
        _groupsRepositoryMock = new Mock<IGroupsRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _userDiffRepositoryMock = new Mock<IUserDiffsRepository>();
        _validatorMock = new Mock<IValidator<Group>>();

        _groupsService = new GroupsService(
            _groupsRepositoryMock.Object,
            _usersRepositoryMock.Object,
            _userDiffRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Test]
    public void AddUser_ShouldThrowIfUserIsAdmin()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group");

        _groupsRepositoryMock.Setup(x => x.GetById(groupId, It.IsAny<CancellationToken>())).ReturnsAsync(group);

        Assert.ThrowsAsync<ArgumentException>(() => _groupsService.AddUser(groupId, userId, CancellationToken.None));
    }

    [Test]
    public void DeleteUser_ShouldThrowIfUserNotInGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User(userId, "Test User", "123@gmail.com", "hash", "ФТ-102-2");
        var group = new Group(groupId, "Test Group");

        _groupsRepositoryMock.Setup(x => x.GetById(groupId, It.IsAny<CancellationToken>())).ReturnsAsync(group);
        _usersRepositoryMock.Setup(x => x.GetById(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        Assert.ThrowsAsync<ArgumentException>(() => _groupsService.DeleteUser(groupId, userId, CancellationToken.None));
    }

    [Test]
    public async Task Add_ShouldCreateNewGroup()
    {
        var adminId = Guid.NewGuid();
        var admin = new User(adminId, "Test Admin", "123@gmail.com", "hash", "ФТ-102-2");
        const string groupName = "Test Group";

        _usersRepositoryMock.Setup(x => x.GetById(adminId, It.IsAny<CancellationToken>())).ReturnsAsync(admin);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        await _groupsService.Add(groupName, CancellationToken.None);

        _usersRepositoryMock.Verify(x => x.GetById(adminId, It.IsAny<CancellationToken>()), Times.Once);
        _groupsRepositoryMock.Verify(
            x => x.Add(It.Is<Group>(g => g.Name == groupName), It.IsAny<CancellationToken>()),
            Times.Once);
        _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Test]
    public async Task AddUser_ShouldAddUserToGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var user = new User(adminId, "Test User", "123@gmail.com", "hash", "ФТ-102-2");
        var group = new Group(groupId, "Test Group");

        _groupsRepositoryMock.Setup(x => x.GetById(groupId, It.IsAny<CancellationToken>())).ReturnsAsync(group);
        _usersRepositoryMock.Setup(x => x.GetById(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await _groupsService.AddUser(groupId, userId, CancellationToken.None);

        _groupsRepositoryMock.Verify(x => x.GetById(groupId, It.IsAny<CancellationToken>()), Times.Once);
        _usersRepositoryMock.Verify(x => x.GetById(userId, It.IsAny<CancellationToken>()), Times.Once);
        _groupsRepositoryMock.Verify(
            x => x.Update(It.Is<Group>(g => g.Users.Contains(user)), It.IsAny<CancellationToken>()), Times.Once);
    }
}