using Microsoft.AspNetCore.Mvc;
using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Controllers;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Controllers;

public class DebtControllerTests
{
    private readonly DebtController _controller;
    private readonly DebtServicesMock _debtServicesMock;
    private readonly CreditCardServicesMock _creditCardServicesMock;
    private readonly TokenServicesMock _tokenServicesMock;
    private readonly UserServicesMock _userServicesMock;
    private readonly CreditCardManagerDbContext _context;

    // Test constants
    private const string DefaultUserName = "Test User";
    private const string DefaultUserEmail = "test@example.com";
    private const string DefaultPassword = "password";
    private const string DefaultCardName = "Test Card";
    private const string DefaultDebtLabel = "Test Debt";
    private const decimal DefaultCardLimit = 1000.00m;
    private const decimal DefaultDebtValue = 100.00m;

    public DebtControllerTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();
        _context = _sqliteInMemory.CreateContext();

        _debtServicesMock = new DebtServicesMock(_context);
        _creditCardServicesMock = new CreditCardServicesMock(_context);
        _tokenServicesMock = new TokenServicesMock();
        _userServicesMock = new UserServicesMock(_context);

        _controller = new DebtController(
            _tokenServicesMock,
            _debtServicesMock,
            _creditCardServicesMock
        );
    }

    // Helper methods
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
            Limit = limit ?? DefaultCardLimit
        });
    }

    private DebtDTO CreateTestDebt(int userId, int cardId, string label = DefaultDebtLabel, decimal? value = null)
    {
        var debtDto = new CreateDebtDTO
        {
            UserId = userId,
            CardId = cardId,
            Label = label,
            Date = DateTime.Now.AddDays(-1),
            Value = value ?? DefaultDebtValue
        };
        _debtServicesMock.CreateDebt(debtDto);
        return _debtServicesMock.GetCardDebts(cardId).First();
    }

    #region GetDebt Tests

    [Fact]
    public void GetDebt_Should_ReturnOkResult_When_DebtExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        DebtDTO debt = CreateTestDebt(user.Id, card.Id);

        // Act
        IActionResult result = _controller.GetDebt(debt.Id);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        DebtDTO returnedDebt = Assert.IsType<DebtDTO>(okResult.Value);
        Assert.Equal(debt.Id, returnedDebt.Id);
        Assert.Equal(debt.Label, returnedDebt.Label);
    }

    [Fact]
    public void GetDebt_Should_ReturnNotFound_When_DebtDoesNotExist()
    {
        // Arrange
        int nonExistentDebtId = 999;

        // Act
        IActionResult result = _controller.GetDebt(nonExistentDebtId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region GetCardDebts Tests

    [Fact]
    public void GetCardDebts_Should_ReturnOkResult_When_CardHasDebts()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        CreateTestDebt(user.Id, card.Id, "Debt1", 100m);
        CreateTestDebt(user.Id, card.Id, "Debt2", 200m);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.GetCardDebts(card.Id, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<DebtDTO> debts = Assert.IsType<List<DebtDTO>>(okResult.Value);
        Assert.NotEmpty(debts);
        Assert.Equal(2, debts.Count);
    }

    [Fact]
    public void GetCardDebts_Should_ReturnOkResult_When_CardHasNoDebts()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.GetCardDebts(card.Id, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<DebtDTO> debts = Assert.IsType<List<DebtDTO>>(okResult.Value);
        Assert.Empty(debts);
    }

    [Fact]
    public void GetCardDebts_Should_ReturnUnauthorized_When_UserDoesNotHaveAccess()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        string token = _tokenServicesMock.GenerateUserToken(user2);

        // Act
        IActionResult result = _controller.GetCardDebts(card.Id, token);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region CreateDebt Tests

    [Fact]
    public void CreateDebt_Should_ReturnCreated_When_DebtCreatedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        CreateDebtDTO createDebtDto = new() { CardId = card.Id, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150m };
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.CreateDebt(createDebtDto, token);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public void CreateDebt_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateDebtDTO createDebtDto = new() { CardId = 1, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150m };
        _controller.ModelState.AddModelError("Label", "Label is required.");
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.CreateDebt(createDebtDto, token);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public void CreateDebt_Should_ReturnUnauthorized_When_AuthorizationFails()
    {
        // Arrange
        CreateDebtDTO createDebtDto = new() { CardId = 1, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150m };
        string invalidToken = "invalid_token";

        // Act
        IActionResult result = _controller.CreateDebt(createDebtDto, invalidToken);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region UpdateDebt Tests

    [Fact]
    public void UpdateDebt_Should_ReturnOkResult_When_DebtUpdatedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        DebtDTO debt = CreateTestDebt(user.Id, card.Id);
        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt", Value = 150m };
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.UpdateDebt(debt.Id, updateDebtDto, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        DebtDTO returnedDebt = Assert.IsType<DebtDTO>(okResult.Value);
        Assert.Equal("Updated Debt", returnedDebt.Label);
        Assert.Equal(150m, returnedDebt.Value);
    }

    [Fact]
    public void UpdateDebt_Should_ReturnNotFound_When_DebtDoesNotExist()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt" };
        string token = _tokenServicesMock.GenerateUserToken(user);
        int nonExistentDebtId = 999;

        // Act
        IActionResult result = _controller.UpdateDebt(nonExistentDebtId, updateDebtDto, token);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void UpdateDebt_Should_ReturnUnauthorized_When_UserIsNotOwner()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        DebtDTO debt = CreateTestDebt(user1.Id, card.Id);
        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt" };
        string token = _tokenServicesMock.GenerateUserToken(user2);

        // Act
        IActionResult result = _controller.UpdateDebt(debt.Id, updateDebtDto, token);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region DeleteDebt Tests

    [Fact]
    public void DeleteDebt_Should_ReturnOkResult_When_DebtDeletedSuccessfully()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);
        DebtDTO debt = CreateTestDebt(user.Id, card.Id);
        string token = _tokenServicesMock.GenerateUserToken(user);

        // Act
        IActionResult result = _controller.DeleteDebt(debt.Id, token);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(_debtServicesMock.GetDebt(debt.Id));
    }

    [Fact]
    public void DeleteDebt_Should_ReturnNotFound_When_DebtDoesNotExist()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        string token = _tokenServicesMock.GenerateUserToken(user);
        int nonExistentDebtId = 999;

        // Act
        IActionResult result = _controller.DeleteDebt(nonExistentDebtId, token);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public void DeleteDebt_Should_ReturnUnauthorized_When_UserIsNotOwner()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);
        DebtDTO debt = CreateTestDebt(user1.Id, card.Id);
        string token = _tokenServicesMock.GenerateUserToken(user2);

        // Act
        IActionResult result = _controller.DeleteDebt(debt.Id, token);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion
}
