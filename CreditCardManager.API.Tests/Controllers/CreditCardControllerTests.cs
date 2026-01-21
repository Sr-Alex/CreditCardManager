using Microsoft.AspNetCore.Mvc;
using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Controllers;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Controllers;

public class CreditCardControllerTests
{
    private readonly CreditCardController _controller;
    private readonly CreditCardServicesMock _creditCardServicesMock;
    private readonly CardUserServicesMock _cardUserServicesMock;
    private readonly TokenServicesMock _tokenServicesMock;
    private readonly UserServicesMock _userServicesMock;
    private readonly CreditCardManagerDbContext _context;

    public CreditCardControllerTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();
        _context = _sqliteInMemory.CreateContext();

        _creditCardServicesMock = new CreditCardServicesMock(_context);
        _cardUserServicesMock = new CardUserServicesMock(_context);
        _tokenServicesMock = new TokenServicesMock();
        _userServicesMock = new UserServicesMock(_context);

        _controller = new CreditCardController(
            _creditCardServicesMock,
            _cardUserServicesMock,
            _tokenServicesMock
        );
    }

    #region GetCreditCards Tests

    [Fact]
    public void GetCreditCards_ShouldReturnOkResult_WhenCardsExist()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        IActionResult result = _controller.GetCreditCards(user.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CreditCardDTO> cards = Assert.IsType<List<CreditCardDTO>>(okResult.Value);
        Assert.NotEmpty(cards);
        Assert.Single(cards);
    }

    [Fact]
    public void GetCreditCards_ShouldReturnNotFound_WhenNoCardsExist()
    {
        // Arrange
        int nonExistentUserId = 999;

        // Act
        IActionResult result = _controller.GetCreditCards(nonExistentUserId);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public void GetCreditCards_ShouldReturnMultipleCards_WhenUserHasMultipleCards()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto1 = new() { UserId = user.Id, CardName = "Card 1", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreateCreditCardDTO createCardDto2 = new() { UserId = user.Id, CardName = "Card 2", ExpiresAt = DateTime.Now.AddYears(1), Limit = 2000.00m };
        _creditCardServicesMock.CreateCreditCard(createCardDto1);
        _creditCardServicesMock.CreateCreditCard(createCardDto2);

        // Act
        IActionResult result = _controller.GetCreditCards(user.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CreditCardDTO> cards = Assert.IsType<List<CreditCardDTO>>(okResult.Value);
        Assert.Equal(2, cards.Count);
    }

    #endregion

    #region GetCreditCard Tests

    [Fact]
    public void GetCreditCard_ShouldReturnOkResult_WhenCardExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        IActionResult result = _controller.GetCreditCard(card.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        CreditCardDTO returnedCard = Assert.IsType<CreditCardDTO>(okResult.Value);
        Assert.Equal(card.Id, returnedCard.Id);
        Assert.Equal(card.CardName, returnedCard.CardName);
    }

    [Fact]
    public void GetCreditCard_ShouldReturnNotFound_WhenCardDoesNotExist()
    {
        // Arrange
        int nonExistentCardId = 999;

        // Act
        IActionResult result = _controller.GetCreditCard(nonExistentCardId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region GetCreditCardUsers Tests

    [Fact]
    public void GetCreditCardUsers_ShouldReturnOkResult_WhenCardExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        IActionResult result = _controller.GetCreditCardUsers(card.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CardUserDTO> users = Assert.IsType<List<CardUserDTO>>(okResult.Value);
        Assert.NotNull(users);
    }

    [Fact]
    public void GetCreditCardUsers_ShouldReturnNotFound_WhenCardDoesNotExist()
    {
        // Arrange
        int nonExistentCardId = 999;

        // Act
        IActionResult result = _controller.GetCreditCardUsers(nonExistentCardId);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("This credit card does not exist.", notFoundResult.Value);
    }

    #endregion

    #region AddUser Tests

    [Fact]
    public void AddUser_ShouldReturnCreated_WhenUserAddedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user1);
        string authHeader = token;

        // Act
        IActionResult result = _controller.AddUser(card.Id, user2.Id, authHeader);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public void AddUser_ShouldReturnConflict_WhenUserAlreadyAdded()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user1);
        string authHeader = token;

        // Add user first time
        _creditCardServicesMock.AddUser(card.Id, user2.Id);

        // Act - Try to add same user again
        IActionResult result = _controller.AddUser(card.Id, user2.Id, authHeader);

        // Assert
        ConflictObjectResult conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(conflictResult.Value);
    }

    [Fact]
    public void AddUser_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateUserDTO createUserDto3 = new() { UserName = "User3", Email = "user3@example.com", Password = "password" };
        UserDTO user3 = _userServicesMock.Create(createUserDto3);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user2);
        string authHeader = token;

        // Act
        IActionResult result = _controller.AddUser(card.Id, user3.Id, authHeader);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region CreateCreditCard Tests

    [Fact]
    public void CreateCreditCard_ShouldReturnCreated_WhenCardCreatedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { CardName = "New Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.CreateCreditCard(createCardDto, authHeader);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        CreditCardDTO returnedCard = Assert.IsType<CreditCardDTO>(createdResult.Value);
        Assert.Equal(createCardDto.CardName, returnedCard.CardName);
        Assert.Equal(user.Id, returnedCard.UserId);
    }

    [Fact]
    public void CreateCreditCard_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { CardName = "New Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Simulate invalid model state
        _controller.ModelState.AddModelError("CardName", "Card name is required.");

        // Act
        IActionResult result = _controller.CreateCreditCard(createCardDto, authHeader);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    #endregion

    #region DeleteCreditCard Tests

    [Fact]
    public void DeleteCreditCard_ShouldReturnOk_WhenCardDeletedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.DeleteCreditCard(card.Id, authHeader);

        // Assert
        OkResult okResult = Assert.IsType<OkResult>(result);
        Assert.False(_creditCardServicesMock.CardIdExists(card.Id));
    }

    [Fact]
    public void DeleteCreditCard_ShouldReturnNotFound_WhenCardDoesNotExist()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = $"Bearer {token}";

        int nonExistentCardId = 999;

        // Act
        IActionResult result = _controller.DeleteCreditCard(nonExistentCardId, authHeader);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteCreditCard_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user2);
        string authHeader = token;

        // Act
        IActionResult result = _controller.DeleteCreditCard(card.Id, authHeader);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion
}
