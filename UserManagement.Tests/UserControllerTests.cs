using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Api.Controllers;
using UserManagement.Api.Contracts.Requests;
using UserManagement.Api.Contracts.Responses;
using UserManagement.Api.Services;
using Microsoft.Extensions.Logging;

namespace UserManagement.Api.Tests.Controllers;

[TestClass]
public class UsersControllerTests
{
    [TestMethod]
    public async Task GetById_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var serviceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<UsersController>>();

        var expectedUser = new UserResponse
        {
            Id = 1,
            Username = "jdoe",
            Email = "jdoe@example.com",
            Profile = new ProfileResponse
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 5, 15)
            }
        };

        serviceMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var controller = new UsersController(serviceMock.Object, loggerMock.Object);

        // Act
        var result = await controller.GetById(1, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var value = okResult.Value as UserResponse;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Id);
        Assert.AreEqual("jdoe", value.Username);
    }

    [TestMethod]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var serviceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<UsersController>>();

        serviceMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse?)null);

        var controller = new UsersController(serviceMock.Object,loggerMock.Object);

        // Act
        var result = await controller.GetById(999, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task Create_ShouldReturnCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var serviceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<UsersController>>();

        var request = new CreateUserRequest
        {
            Username = "jdoe",
            Email = "jdoe@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var createdUser = new UserResponse
        {
            Id = 1,
            Username = "jdoe",
            Email = "jdoe@example.com",
            Profile = new ProfileResponse
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 5, 15)
            }
        };

        serviceMock
            .Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        var controller = new UsersController(serviceMock.Object, loggerMock.Object);

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Result);

        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        Assert.AreEqual(nameof(UsersController.GetById), createdAtActionResult.ActionName);
        Assert.AreEqual(1, createdAtActionResult.RouteValues!["id"]);

        var value = createdAtActionResult.Value as UserResponse;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Id);
        Assert.AreEqual("jdoe", value.Username);
    }
}