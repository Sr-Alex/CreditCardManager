using Microsoft.AspNetCore.Mvc;
using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Controllers;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Controllers;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly UserServicesMock _userServicesMock;
    private readonly TokenServicesMock _tokenServicesMock;
    private readonly CreditCardManagerDbContext _context;

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

    #region GetUsers Tests

    [Fact]
    public void GetUsers_ShouldReturnOkResult_WhenUsersExist()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        string token = _tokenServicesMock.GenerateUserToken(user1);
        string authHeader = token;

        // Set authorization header on controller
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = authHeader;

        // Act
        IActionResult result = _controller.GetUsers();

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<UserDTO> users = Assert.IsType<List<UserDTO>>(okResult.Value);
        Assert.NotEmpty(users);
        Assert.True(users.Count >= 2);
    }

    [Fact]
    public void GetUsers_ShouldReturnNotFound_WhenNoUsersExist()
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
    public void GetUser_ShouldReturnOkResult_WhenUserExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

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
    public void GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
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
    public void CreateUser_ShouldReturnCreated_WhenUserCreatedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "New User", Email = "newuser@example.com", Password = "password123" };

        // Act
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
        
        // Verify the response contains token and user
        var responseObject = createdResult.Value;
        Assert.NotNull(responseObject);
    }

    [Fact]
    public void CreateUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "New User", Email = "newuser@example.com", Password = "password123" };

        // Simulate invalid model state
        _controller.ModelState.AddModelError("Email", "Email is required.");

        // Act
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public void CreateUser_ShouldReturnConflict_WhenUserAlreadyExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Duplicate User", Email = "duplicate@example.com", Password = "password123" };
        _userServicesMock.Create(createUserDto);

        // Act - Try to create same user again
        IActionResult result = _controller.CreateUser(createUserDto);

        // Assert
        ConflictObjectResult conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(conflictResult.Value);
    }

    #endregion

    #region LoginUser Tests

    [Fact]
    public void LoginUser_ShouldReturnOkResult_WhenCredentialsAreValid()
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
        
        // Verify the response contains token and user
        var responseObject = okResult.Value;
        Assert.NotNull(responseObject);
    }

    [Fact]
    public void LoginUser_ShouldReturnUnauthorized_WhenEmailDoesNotExist()
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
    public void LoginUser_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
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
    public void LoginUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        LoginUserDTO loginDto = new() { Email = "login@example.com", Password = "password123" };

        // Simulate invalid model state
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
    public void DeleteUser_ShouldReturnNoContent_WhenUserDeletedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Delete User", Email = "delete@example.com", Password = "password123" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        string token = _tokenServicesMock.GenerateUserToken(user);

        // Set authorization header on controller
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = token;

        // Act
        IActionResult result = _controller.DeleteUser();

        // Assert
        NoContentResult noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.False(_userServicesMock.UserIdExists(user.Id));
    }

    [Fact]
    public void DeleteUser_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        string invalidToken = "invalid_token";

        // Set invalid authorization header on controller
        _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = invalidToken;

        // Act
        IActionResult result = _controller.DeleteUser();

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public void DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        UserDTO fakeUser = new() { Id = 999, UserName = "Fake User", Email = "fake@example.com" };
        string token = _tokenServicesMock.GenerateUserToken(fakeUser);

        // Set authorization header on controller
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
