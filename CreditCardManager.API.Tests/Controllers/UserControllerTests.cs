using Microsoft.AspNetCore.Mvc;
using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Controllers;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.Tests.Controllers;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly UserServicesMock _userServicesMock;
    private readonly TokenServicesMock _tokenServicesMock;
    private readonly CreditCardManagerDbContext _context;

    // Test constants
    private const string DefaultUserName = "Test User";
    private const string DefaultUserEmail = "test@example.com";
    private const string DefaultPassword = "password";

    public UserControllerTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();
        _context = _sqliteInMemory.CreateContext();

        _userServicesMock = new UserServicesMock(_context);
        _tokenServicesMock = new TokenServicesMock();

        _controller = new UserController(
            _userServicesMock,
            _tokenServicesMock
        );
    }

    // Helper methods
    private UserDTO CreateTestUser(string userName = DefaultUserName, string email = DefaultUserEmail)
    {
        return _userServicesMock.Create(new CreateUserDTO { UserName = userName, Email = email, Password = DefaultPassword });
    }

    #region GetUsers Tests

    [Fact]
    public void GetUsers_Should_ReturnOkResult_When_UsersExist()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");

        string token = _tokenServicesMock.GenerateUserToken(user1);

        // Set authorization header on controller
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = token;

        // Act
        IActionResult result = _controller.GetUsers();

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<UserDTO> users = Assert.IsType<List<UserDTO>>(okResult.Value);
        Assert.NotEmpty(users);
        Assert.True(users.Count >= 2);
    }

    [Fact]
    public void GetUsers_Should_ReturnNotFound_When_NoUsersExist()
    {
        // Arrange - Create a fresh context with no users
        SqliteInMemoryController _sqliteInMemory = new();
        CreditCardManagerDbContext _freshContext = _sqliteInMemory.CreateContext();
        UserServicesMock _freshUserServicesMock = new UserServicesMock(_freshContext);

        UserController _freshController = new UserController(
            _freshUserServicesMock,
            _tokenServicesMock
        );

        // Act
        IActionResult result = _freshController.GetUsers();

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion

    #region GetUser Tests

    [Fact]
    public void GetUser_Should_ReturnOkResult_When_UserExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();

        // Act
        IActionResult result = _controller.GetUser(user.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        UserDTO returnedUser = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(user.UserName, returnedUser.UserName);
        Assert.Equal(user.Email, returnedUser.Email);
    }

    [Fact]
    public void GetUser_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        // Arrange
        int nonExistentUserId = 999;

        // Act
        IActionResult result = _controller.GetUser(nonExistentUserId);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public void CreateUser_Should_ReturnCreated_When_UserCreatedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "New User", Email = "newuser@example.com", Password = "password123" };

        // Act
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public void CreateUser_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "New User", Email = "newuser@example.com", Password = "password123" };
        _controller.ModelState.AddModelError("Email", "Email is required.");

        // Act
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public void CreateUser_Should_ReturnConflict_When_UserAlreadyExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Duplicate User", Email = "duplicate@example.com", Password = "password123" };
        _userServicesMock.Create(createUserDto);

        // Act
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        ConflictObjectResult conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(conflictResult.Value);
    }

    #endregion

    #region LoginUser Tests

    [Fact]
    public void LoginUser_Should_ReturnOkResult_When_CredentialsAreValid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Login User", Email = "login@example.com", Password = "password123" };
        _userServicesMock.Create(createUserDto);
        LoginUserDTO loginDto = new() { Email = "login@example.com", Password = "password123" };

        // Act
        IActionResult result = _controller.LoginUser(loginDto);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void LoginUser_Should_ReturnUnauthorized_When_EmailDoesNotExist()
    {
        // Arrange
        LoginUserDTO loginDto = new() { Email = "nonexistent@example.com", Password = "password123" };

        // Act
        IActionResult result = _controller.LoginUser(loginDto);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void LoginUser_Should_ReturnUnauthorized_When_PasswordIsIncorrect()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Login User", Email = "login@example.com", Password = "password123" };
        _userServicesMock.Create(createUserDto);
        LoginUserDTO loginDto = new() { Email = "login@example.com", Password = "wrongpassword" };

        // Act
        IActionResult result = _controller.LoginUser(loginDto);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void LoginUser_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        LoginUserDTO loginDto = new() { Email = "login@example.com", Password = "password123" };
        _controller.ModelState.AddModelError("Email", "Email is required.");

        // Act
        IActionResult result = _controller.LoginUser(loginDto);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public void DeleteUser_Should_ReturnNoContent_When_UserDeletedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser("Delete User", "delete@example.com");
        string token = _tokenServicesMock.GenerateUserToken(user);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = token;

        // Act
        IActionResult result = _controller.DeleteUser();

        // Assert
        NoContentResult noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.False(_userServicesMock.UserIdExists(user.Id), "User should be deleted");
    }

    [Fact]
    public void DeleteUser_Should_ReturnUnauthorized_When_TokenIsInvalid()
    {
        // Arrange
        string invalidToken = "invalid_token";
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = invalidToken;

        // Act
        IActionResult result = _controller.DeleteUser();

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void DeleteUser_Should_ReturnNotFound_When_UserDoesNotExist()
    {
        // Arrange
        UserDTO fakeUser = new() { Id = 999, UserName = "Fake User", Email = "fake@example.com" };
        string token = _tokenServicesMock.GenerateUserToken(fakeUser);
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = token;

        // Act
        IActionResult result = _controller.DeleteUser();

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    #endregion
}
