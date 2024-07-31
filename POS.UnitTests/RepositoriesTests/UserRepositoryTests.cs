using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using POS.Data;
using POS.Models.Entities;
using POS.Repositories.UserRepository;
using POS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class UserRepositoryTests
{
    private POSDbContext _context;
    private UserRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Create a new instance of the in-memory database context
        var options = new DbContextOptionsBuilder<POSDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new POSDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { id = "1", name = "admin", email = "admin@example.com", password = Password.EncodePasswordToBase64("adminpass"), role = UserRole.Admin },
            new User { id = "2", name = "cashier", email = "cashier@example.com", password = Password.EncodePasswordToBase64("cashierpass"), role = UserRole.Cashier }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [Test]
    public async Task AddAsync_ShouldAddUser_WhenUserIsNew()
    {
        // Arrange
        var user = new User { name = "newuser", email = "newuser@example.com", password = Password.EncodePasswordToBase64("newpass"), role = UserRole.Cashier };

        // Act
        await _repository.AddAsync(user);

        // Assert
        var addedUser = await _repository.GetUserByIdAsync(Convert.ToInt32(user.Id));
        Assert.IsNotNull(addedUser);
        Assert.AreEqual("newuser", addedUser.name);
    }

    [Test]
    public async Task LogInAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User { name = "admin", email = "admin@example.com", password = Password.EncodePasswordToBase64("adminpass"), role = UserRole.Admin };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.LogInAsync("admin", "adminpass");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("admin", result.name);
    }

    [Test]
    public async Task UpdateUserRoleAsync_ShouldUpdateRole_WhenUserExists()
    {
        // Arrange
        var user = new User { name = "user", email = "user@example.com", password = Password.EncodePasswordToBase64("userpass"), role = UserRole.Cashier };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UpdateUserRoleAsync("user", UserRole.Admin);

        // Assert
        Assert.IsTrue(result);
        var updatedUser = await _repository.GetUserByIdAsync(Convert.ToInt32(user.Id));
        Assert.AreEqual(UserRole.Admin, updatedUser.role);
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { id = "1", name = "user", email = "user@example.com", password = Password.EncodePasswordToBase64("userpass"), role = UserRole.Cashier };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("user", result.name);
    }

    [Test]
    public async Task SeedUsersAsync_ShouldAddDefaultUsers_WhenNoUsersExist()
    {
        // Act
        await _repository.SeedUsersAsync();

        // Assert
        var users = await _repository.GetAllAsync();
        Assert.AreEqual(3, users.Count); // 3 default users should be seeded
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
