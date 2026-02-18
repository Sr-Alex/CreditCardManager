using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services;

public class CreditCardServicesTests
{
    private readonly CreditCardServicesMock _creditCardServicesMock;
    private readonly UserServicesMock _userServicesMock;

    // Test constants
    private const string DefaultUserName = "Test User";
    private const string DefaultUserEmail = "test@example.com";
    private const string DefaultPassword = "password";
    private const string DefaultCardName = "Test Card";
    private const decimal DefaultLimit = 1000.00m;

    public CreditCardServicesTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();

        CreditCardManagerDbContext _context = _sqliteInMemory.CreateContext();

        _creditCardServicesMock = new CreditCardServicesMock(_context);
        _userServicesMock = new UserServicesMock(_context);
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
            ExpiresAt = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
            Limit = limit ?? DefaultLimit
        });
    }

    [Fact]
    public void IsUserOwnerOfCard_ShouldReturnTrue_WhenUserIsOwner()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.IsUserOwnerOfCard(card.Id, user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUserOwnerOfCard_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        // Arrange
        var user1 = CreateTestUser("User1", "user1@example.com");
        var user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);

        // Act
        var result = _creditCardServicesMock.IsUserOwnerOfCard(card.Id, user2.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCardUser_ShouldReturnTrue_WhenUserIsAssociated()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.IsCardUser(card.Id, user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CardIdExists_ShouldReturnTrue_WhenCardExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.CardIdExists(card.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CardIdExists_ShouldReturnFalse_WhenCardDoesNotExist()
    {
        // Act
        var result = _creditCardServicesMock.CardIdExists(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateCreditCard_ShouldCreateAndReturnCard()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = DefaultCardName, ExpiresAt = DateOnly.FromDateTime(DateTime.Now.AddYears(1)), Limit = DefaultLimit };

        // Act
        CreditCardDTO result = _creditCardServicesMock.CreateCreditCard(createCardDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(createCardDto.CardName, result.CardName);
        Assert.Equal(createCardDto.UserId, result.UserId);
    }

    [Fact]
    public void DeleteCreditCard_ShouldReturnTrue_WhenCardExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.DeleteCreditCard(card.Id);

        // Assert
        Assert.True(result);
        Assert.False(_creditCardServicesMock.CardIdExists(card.Id));
    }

    [Fact]
    public void GetCreditCard_ShouldReturnCard_WhenExists()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.GetCreditCard(card.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(card.Id, result.Id);
        Assert.Equal(card.CardName, result.CardName);
    }

    [Fact]
    public void GetCreditCard_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = _creditCardServicesMock.GetCreditCard(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserCreditCards_ShouldReturnList()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreateTestCard(user.Id, "Card1");
        CreateTestCard(user.Id, "Card2", 2000.00m);

        // Act
        var result = _creditCardServicesMock.GetUserCreditCards(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void UpdateInvoice_ShouldReturnDecimal()
    {
        // Arrange
        UserDTO user = CreateTestUser();
        CreditCardDTO card = CreateTestCard(user.Id);

        // Act
        var result = _creditCardServicesMock.UpdateInvoice(card.Id);

        // Assert
        Assert.IsType<decimal>(result);
    }

    [Fact]
    public void AddUser_ShouldReturnTrue_WhenUserAdded()
    {
        // Arrange
        UserDTO user1 = CreateTestUser("User1", "user1@example.com");
        UserDTO user2 = CreateTestUser("User2", "user2@example.com");
        CreditCardDTO card = CreateTestCard(user1.Id);

        // Act
        var result = _creditCardServicesMock.AddUser(card.Id, user2.Email);

        // Assert
        Assert.True(result);
        Assert.True(_creditCardServicesMock.IsCardUser(card.Id, user2.Id));
    }
}
