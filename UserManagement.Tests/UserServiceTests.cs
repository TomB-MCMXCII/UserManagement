using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.Api.Contracts.Requests;
using UserManagement.Api.Data;
using UserManagement.Api.Domain.Entities;
using UserManagement.Api.Services;

namespace UserManagement.Api.Tests.Services;

[TestClass]
public class UserServiceTests
{
    [TestMethod]
    public async Task CreateAsync_ShouldCreateUserAndProfile_AndPublishMessage()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var publisherMock = new Mock<IMessagePublisher>();
        var loggerMock = new Mock<ILogger<UserService>>();

        var service = new UserService(dbContext, publisherMock.Object, loggerMock.Object);

        var request = new CreateUserRequest
        {
            Username = "jdoe",
            Email = "jdoe@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Id);
        Assert.AreEqual("jdoe", result.Username);
        Assert.AreEqual("jdoe@example.com", result.Email);
        Assert.IsNotNull(result.Profile);
        Assert.AreEqual("John", result.Profile.FirstName);
        Assert.AreEqual("Doe", result.Profile.LastName);
        Assert.AreEqual(new DateTime(1990, 5, 15), result.Profile.DateOfBirth);

        var userInDb = await dbContext.Users
            .Include(x => x.Profile)
            .SingleAsync();

        Assert.AreEqual("jdoe", userInDb.Username);
        Assert.AreEqual("jdoe@example.com", userInDb.Email);
        Assert.IsNotNull(userInDb.Profile);
        Assert.AreEqual("John", userInDb.Profile.FirstName);
        Assert.AreEqual("Doe", userInDb.Profile.LastName);

        publisherMock.Verify(
            x => x.PublishUserCreatedAsync(
                It.Is<User>(u =>
                    u.Id == result.Id &&
                    u.Username == "jdoe" &&
                    u.Email == "jdoe@example.com"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnUserWithProfile_WhenUserExists()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var publisherMock = new Mock<IMessagePublisher>();
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UserService>>();

        var user = new User
        {
            Username = "asmith",
            Email = "asmith@example.com",
            Profile = new Profile
            {
                FirstName = "Anna",
                LastName = "Smith",
                DateOfBirth = new DateTime(1995, 1, 10)
            }
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var service = new UserService(dbContext, publisherMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.Id);
        Assert.AreEqual("asmith", result.Username);
        Assert.AreEqual("asmith@example.com", result.Email);
        Assert.IsNotNull(result.Profile);
        Assert.AreEqual("Anna", result.Profile.FirstName);
        Assert.AreEqual("Smith", result.Profile.LastName);
        Assert.AreEqual(new DateTime(1995, 1, 10), result.Profile.DateOfBirth);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var publisherMock = new Mock<IMessagePublisher>();
        var loggerMock = new Mock<ILogger<UserService>>();

        var service = new UserService(dbContext, publisherMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}