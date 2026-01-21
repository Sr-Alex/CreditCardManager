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

    #region GetDebt Tests

    [Fact]
    public void GetDebt_ShouldReturnOkResult_WhenDebtExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        var debts = _debtServicesMock.GetCardDebts(card.Id);
        var debtId = debts.First().Id;

        // Act
        IActionResult result = _controller.GetDebt(debtId);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        DebtDTO returnedDebt = Assert.IsType<DebtDTO>(okResult.Value);
        Assert.Equal(debtId, returnedDebt.Id);
        Assert.Equal(createDebtDto.Label, returnedDebt.Label);
    }

    [Fact]
    public void GetDebt_ShouldReturnNotFound_WhenDebtDoesNotExist()
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
    public void GetCardDebts_ShouldReturnOkResult_WhenCardHasDebts()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto1 = new() { UserId = user.Id, CardId = card.Id, Label = "Debt1", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        CreateDebtDTO createDebtDto2 = new() { UserId = user.Id, CardId = card.Id, Label = "Debt2", Date = DateTime.Now.AddDays(-2), Value = 200.00m };
        _debtServicesMock.CreateDebt(createDebtDto1);
        _debtServicesMock.CreateDebt(createDebtDto2);

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.GetCardDebts(card.Id, authHeader);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<DebtDTO> debts = Assert.IsType<List<DebtDTO>>(okResult.Value);
        Assert.NotEmpty(debts);
        Assert.Equal(2, debts.Count);
    }

    [Fact]
    public void GetCardDebts_ShouldReturnOkResult_WhenCardHasNoDebts()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.GetCardDebts(card.Id, authHeader);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        List<DebtDTO> debts = Assert.IsType<List<DebtDTO>>(okResult.Value);
        Assert.Empty(debts);
    }

    [Fact]
    public void GetCardDebts_ShouldReturnUnauthorized_WhenUserDoesNotHaveAccess()
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
        IActionResult result = _controller.GetCardDebts(card.Id, authHeader);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region CreateDebt Tests

    [Fact]
    public void CreateDebt_ShouldReturnCreated_WhenDebtCreatedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { CardId = card.Id, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150.00m };

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.CreateDebt(createDebtDto, authHeader);

        // Assert
        CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public void CreateDebt_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateDebtDTO createDebtDto = new() { CardId = 1, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150.00m };

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Simulate invalid model state
        _controller.ModelState.AddModelError("Label", "Label is required.");

        // Act
        IActionResult result = _controller.CreateDebt(createDebtDto, authHeader);

        // Assert
        BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public void CreateDebt_ShouldReturnUnauthorized_WhenAuthorizationFails()
    {
        // Arrange
        CreateDebtDTO createDebtDto = new() { CardId = 1, Label = "New Debt", Date = DateTime.Now.AddDays(-1), Value = 150.00m };
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
    public void UpdateDebt_ShouldReturnOkResult_WhenDebtUpdatedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        var debts = _debtServicesMock.GetCardDebts(card.Id);
        var debtId = debts.First().Id;

        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt", Value = 150.00m };

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.UpdateDebt(debtId, updateDebtDto, authHeader);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        DebtDTO returnedDebt = Assert.IsType<DebtDTO>(okResult.Value);
        Assert.Equal("Updated Debt", returnedDebt.Label);
        Assert.Equal(150.00m, returnedDebt.Value);
    }

    [Fact]
    public void UpdateDebt_ShouldReturnNotFound_WhenDebtDoesNotExist()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt" };
        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        int nonExistentDebtId = 999;

        // Act
        IActionResult result = _controller.UpdateDebt(nonExistentDebtId, updateDebtDto, authHeader);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void UpdateDebt_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user1.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        var debts = _debtServicesMock.GetCardDebts(card.Id);
        var debtId = debts.First().Id;

        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt" };

        string token = _tokenServicesMock.GenerateUserToken(user2);
        string authHeader = token;

        // Act
        IActionResult result = _controller.UpdateDebt(debtId, updateDebtDto, authHeader);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion

    #region DeleteDebt Tests

    [Fact]
    public void DeleteDebt_ShouldReturnOkResult_WhenDebtDeletedSuccessfully()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        var debts = _debtServicesMock.GetCardDebts(card.Id);
        var debtId = debts.First().Id;

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        // Act
        IActionResult result = _controller.DeleteDebt(debtId, authHeader);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(_debtServicesMock.GetDebt(debtId));
    }

    [Fact]
    public void DeleteDebt_ShouldReturnNotFound_WhenDebtDoesNotExist()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        string token = _tokenServicesMock.GenerateUserToken(user);
        string authHeader = token;

        int nonExistentDebtId = 999;

        // Act
        IActionResult result = _controller.DeleteDebt(nonExistentDebtId, authHeader);

        // Assert
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public void DeleteDebt_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user1.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        var debts = _debtServicesMock.GetCardDebts(card.Id);
        var debtId = debts.First().Id;

        string token = _tokenServicesMock.GenerateUserToken(user2);
        string authHeader = token;

        // Act
        IActionResult result = _controller.DeleteDebt(debtId, authHeader);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    #endregion
}
