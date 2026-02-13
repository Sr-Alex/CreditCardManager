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

    // Test constants
    private const string DefaultUserName = "Test User";
    private const string DefaultUserEmail = "test@example.com";
    private const string DefaultPassword = "password";
    private const string DefaultCardName = "Test Card";
    private const decimal DefaultLimit = 1000.00m;

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

    // Helper methods for test setup
    private UserDTO CreateTestUser(string userName = DefaultUserName, string email = DefaultUserEmail)
    {
        return _userServicesMock.Create(new CreateUserDTO { UserName = userName, Email = email, Password = DefaultPassword });
    }

    private CreditCardDTO CreateTestCard(int userId, string cardName = DefaultCardName, decimal? limit = null)
    {
        return _creditCardServicesMock.CreateCreditCard(new CreateCreditCardDTO
        {
            UserId = userId,
            CardName = cardName,
            ExpiresAt = DateTime.Now.AddYears(1),
            Limit = limit ?? DefaultLimit
        });
    }

    #region GetCreditCards Tests

    [Fact]
    public void GetCreditCards_Should_ReturnOkResult_When_CardsExist()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateTestCard(user.Id);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.GetCreditCards(user.Id, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CreditCardDTO> cards = Assert.IsType<List<CreditCardDTO>>(okResult.Value);
        Assert.NotEmpty(cards);
        Assert.Single(cards);
    }

    [Fact]
    public void GetCreditCards_Should_ReturnNotFound_When_NoCardsExist()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        string token = _tokenServicesMock.GenerateUserToken(user);
        int nonExistentUserId = 999;

        // Act
        IActionResult result = _controller.GetCreditCards(nonExistentUserId, token);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public void GetCreditCards_Should_ReturnMultipleCards_When_UserHasMultipleCards()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateTestCard(user.Id, "Card 1", 1000m);
        CreateTestCard(user.Id, "Card 2", 2000m);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.GetCreditCards(user.Id, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CreditCardDTO> cards = Assert.IsType<List<CreditCardDTO>>(okResult.Value);
        Assert.Equal(2, cards.Count);
    }

    #endregion

    #region GetCreditCard Tests

    [Fact]
    public void GetCreditCard_Should_ReturnOkResult_When_CardExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        IActionResult result = _controller.GetCreditCard(card.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        CreditCardDTO returnedCard = Assert.IsType<CreditCardDTO>(okResult.Value);
        Assert.Equal(card.Id, returnedCard.Id);
        Assert.Equal(card.CardName, returnedCard.CardName);
    }

    [Fact]
    public void GetCreditCard_Should_ReturnNotFound_When_CardDoesNotExist()
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
    public void GetCreditCardUsers_Should_ReturnOkResult_When_CardExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        IActionResult result = _controller.GetCreditCardUsers(card.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<CardUserDTO> users = Assert.IsType<List<CardUserDTO>>(okResult.Value);
        Assert.NotNull(users);
    }

    [Fact]
    public void GetCreditCardUsers_Should_ReturnNotFound_When_CardDoesNotExist()
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
    public void AddUser_Should_ReturnCreated_When_UserAddedSuccessfully()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        string token = _tokenServicesMock.GenerateUserToken(user1);

        // Act
        IActionResult result = _controller.AddUser(card.Id, new UserEmailDTO { UserEmail = user2.Email }, token);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public void AddUser_Should_ReturnConflict_When_UserAlreadyAdded()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        _creditCardServicesMock.AddUser(card.Id, user2.Email);
        string token = _tokenServicesMock.GenerateUserToken(user1);

        // Act
        IActionResult result = _controller.AddUser(card.Id, new UserEmailDTO { UserEmail = user2.Email }, token);

        // Assert
        ConflictObjectResult conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(conflictResult.Value);
    }

    [Fact]
    public void AddUser_Should_ReturnUnauthorized_When_UserIsNotOwner()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        UserDTO user3 = CreateTestUser("User3", "user3@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        string token = _tokenServicesMock.GenerateUserToken(user2);

        // Act
        IActionResult result = _controller.AddUser(card.Id,  new UserEmailDTO { UserEmail = user3.Email }, token);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region CreateCreditCard Tests

    [Fact]
    public void CreateCreditCard_Should_ReturnCreated_When_CardCreatedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateCreditCardDTO createCardDto = new() { CardName = "New Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000m };
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.CreateCreditCard(createCardDto, token);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        CreditCardDTO returnedCard = Assert.IsType<CreditCardDTO>(createdResult.Value);
        Assert.Equal(createCardDto.CardName, returnedCard.CardName);
        Assert.Equal(user.Id, returnedCard.UserId);
    }

    [Fact]
    public void CreateCreditCard_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateCreditCardDTO createCardDto = new() { CardName = "New Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000m };
        _controller.ModelState.AddModelError("CardName", "Card name is required.");
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.CreateCreditCard(createCardDto, token);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    #endregion

    #region DeleteCreditCard Tests

    [Fact]
    public void DeleteCreditCard_Should_ReturnOk_When_CardDeletedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.DeleteCreditCard(card.Id, token);

        // Assert
        OkResult okResult = Assert.IsType<OkResult>(result);
        Assert.False(_creditCardServicesMock.CardIdExists(card.Id));
    }

    [Fact]
    public void DeleteCreditCard_Should_ReturnNotFound_When_CardDoesNotExist()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        string token = _tokenServicesMock.GenerateUserToken(user);
        int nonExistentCardId = 999;

        // Act
        IActionResult result = _controller.DeleteCreditCard(nonExistentCardId, token);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteCreditCard_Should_ReturnUnauthorized_When_UserIsNotOwner()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        string token = _tokenServicesMock.GenerateUserToken(user2);

        // Act
        IActionResult result = _controller.DeleteCreditCard(card.Id, token);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion
}
