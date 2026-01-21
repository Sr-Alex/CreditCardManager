using System.Data;
using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services;

public class DebtServicesTests
{
    private readonly DebtServicesMock _debtServicesMock;
    private readonly CreditCardServicesMock _creditCardServicesMock;
    private readonly UserServicesMock _userServicesMock;

    public DebtServicesTests()
    {
        SqliteInMemoryController _sqliteInMemoryController = new();
        CreditCardManagerDbContext _context = _sqliteInMemoryController.CreateContext();

        _creditCardServicesMock = new CreditCardServicesMock(_context);
        _userServicesMock = new UserServicesMock(_context);
        _debtServicesMock = new DebtServicesMock(_context);
    }

    [Fact]
    public void CreateDebt_ShouldReturnTrue_WhenValid()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };

        // Act
        var result = _debtServicesMock.CreateDebt(createDebtDto);
        var debts = _debtServicesMock.GetCardDebts(card.Id);

        // Assert
        Assert.True(result);
        Assert.Single(debts);
    }

    [Fact]
    public void CreateDebt_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        CreateDebtDTO createDebtDto = new() { UserId = 999, CardId = 1, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };

        // Act & Assert
        Assert.Throws<Exception>(() => _debtServicesMock.CreateDebt(createDebtDto));
    }

    [Fact]
    public void CreateDebt_ShouldThrowException_WhenCardDoesNotExist()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = 999, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };

        // Act & Assert
        Assert.Throws<Exception>(() => _debtServicesMock.CreateDebt(createDebtDto));
    }

    [Fact]
    public void GetDebt_ShouldReturnDebt_WhenExists()
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
        var result = _debtServicesMock.GetDebt(debtId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(debtId, result.Id);
        Assert.Equal(createDebtDto.Label, result.Label);
    }

    [Fact]
    public void GetDebt_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = _debtServicesMock.GetDebt(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCardDebts_ShouldReturnList_WhenCardHasDebts()
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

        // Act
        var result = _debtServicesMock.GetCardDebts(card.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetCardDebts_ShouldReturnEmptyList_WhenCardHasNoDebts()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _debtServicesMock.GetCardDebts(card.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void UpdateDebt_ShouldReturnTrue_WhenExists()
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

        // Act
        var result = _debtServicesMock.UpdateDebt(debtId, updateDebtDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Debt", result.Label);
        Assert.Equal(150.00m, result.Value);
    }

    [Fact]
    public void UpdateDebt_ShouldThrowException_WhenNotExists()
    {
        // Arrange
        UpdateDebtDTO updateDebtDto = new() { Label = "Updated Debt" };

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _debtServicesMock.UpdateDebt(999, updateDebtDto));
    }

    [Fact]
    public void UpdateDebt_ShouldChangeCardInvoice_whenValueChanges()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        CreateDebtDTO createDebtDto = new() { UserId = user.Id, CardId = card.Id, Label = "Test Debt", Date = DateTime.Now.AddDays(-1), Value = 100.00m };
        _debtServicesMock.CreateDebt(createDebtDto);

        // Act
        var debtsValue = _debtServicesMock.GetCardDebts(card.Id).Sum(d => d.Value);
        var updatedCard = _creditCardServicesMock.GetCreditCard(createDebtDto.CardId);

        // Assert 
        Assert.NotNull(updatedCard);
        Assert.Equal(100.00m, debtsValue);
        Assert.Equal(decimal.Parse(updatedCard.Invoice), debtsValue);
    }

    [Fact]
    public void DeleteDebt_ShouldReturnTrue_WhenExists()
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
        var result = _debtServicesMock.DeleteDebt(debtId);

        // Assert
        Assert.True(result);
        Assert.Null(_debtServicesMock.GetDebt(debtId));
    }

    [Fact]
    public void DeleteDebt_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var result = _debtServicesMock.DeleteDebt(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeleteDebt_ShouldUpdateCardInvoice_WhenDebtIsDeleted()
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
        var result = _debtServicesMock.DeleteDebt(debtId);
        var updatedCard = _creditCardServicesMock.GetCreditCard(card.Id);

        // Assert 
        Assert.True(result);
        Assert.NotNull(updatedCard);
        Assert.Equal(0, decimal.Parse(updatedCard.Invoice));
    }
}