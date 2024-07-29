using Moq;
using NUnit.Framework;
using POS.Models.Entities;
using POS.Repositories.UserRepository;
using POS.Services.UserServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Test]
    public async Task RegisterUserAsync_ShouldReturnTrue_WhenUserIsNew()
    {
        // Arrange
        var newUser = new User { name = "NewUser", email = "newuser@example.com" };
        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<User>());
        _userRepositoryMock.Setup(repo => repo.AddAsync(newUser))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterUserAsync(newUser);

        // Assert
        Assert.IsTrue(result);
        _userRepositoryMock.Verify(repo => repo.AddAsync(newUser), Times.Once);
    }

    [Test]
    public async Task RegisterUserAsync_ShouldReturnFalse_WhenUserAlreadyExists()
    {
        // Arrange
        var existingUser = new User { name = "ExistingUser", email = "existinguser@example.com" };
        var users = new List<User> { existingUser };
        var newUser = new User { name = "ExistingUser", email = "newemail@example.com" };

        _userRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.RegisterUserAsync(newUser);

        // Assert
        Assert.IsFalse(result);
        _userRepositoryMock.Verify(repo => repo.AddAsync(newUser), Times.Never);
    }

    [Test]
    public async Task Login_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User { name = "ValidUser", email = "validuser@example.com" };
        _userRepositoryMock.Setup(repo => repo.LogInAsync(user.name, "password"))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.Login(user.name, "password");

        // Assert
        Assert.AreEqual(user, result);
    }

    [Test]
    public async Task Login_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.LogInAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _userService.Login("InvalidUser", "wrongpassword");

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task UpdateUserRole_ShouldReturnTrue_WhenRoleIsUpdated()
    {
        // Arrange
        var username = "User";
        var role = UserRole.Admin;
        _userRepositoryMock.Setup(repo => repo.UpdateUserRoleAsync(username, role))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserRole(username, role);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, name = "User", email = "user@example.com" };
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.AreEqual(user, result);
    }

   
}


