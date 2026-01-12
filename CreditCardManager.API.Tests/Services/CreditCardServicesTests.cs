using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services;

public class CreditCardServicesTests
{
    private readonly CreditCardServicesMock _creditCardServicesMock;
    private readonly UserServicesMock _userServicesMock;

    public CreditCardServicesTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();

        CreditCardManagerDbContext _context = _sqliteInMemory.CreateContext();

        _creditCardServicesMock = new CreditCardServicesMock(_context);
        _userServicesMock = new UserServicesMock(_context);
    }

    [Fact]
    public void IsUserOwnerOfCard_ShouldReturnTrue_WhenUserIsOwner()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _creditCardServicesMock.IsUserOwnerOfCard(card.Id, user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsUserOwnerOfCard_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new() { UserName = "User1", Email = "user1@example.com", Password = "password" };
        var user1 = _userServicesMock.Create(createUserDto1);

        CreateUserDTO createUserDto2 = new() { UserName = "User2", Email = "user2@example.com", Password = "password" };
        var user2 = _userServicesMock.Create(createUserDto2);

        CreateCreditCardDTO createCardDto = new() { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _creditCardServicesMock.IsUserOwnerOfCard(card.Id, user2.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCardUser_ShouldReturnTrue_WhenUserIsAssociated()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _creditCardServicesMock.IsCardUser(card.Id, user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CardIdExists_ShouldReturnTrue_WhenCardExists()
    {
        // Arrange
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

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
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };

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
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);
        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

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
        CreateUserDTO createUserDto = new() { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto = new() { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

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
        CreateUserDTO createUserDto = new CreateUserDTO { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);

        CreateCreditCardDTO createCardDto1 = new CreateCreditCardDTO { UserId = user.Id, CardName = "Card1", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreateCreditCardDTO createCardDto2 = new CreateCreditCardDTO { UserId = user.Id, CardName = "Card2", ExpiresAt = DateTime.Now.AddYears(1), Limit = 2000.00m };
        _creditCardServicesMock.CreateCreditCard(createCardDto1);
        _creditCardServicesMock.CreateCreditCard(createCardDto2);

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
        CreateUserDTO createUserDto = new CreateUserDTO { UserName = "Test User", Email = "test@example.com", Password = "password" };
        UserDTO user = _userServicesMock.Create(createUserDto);
        CreateCreditCardDTO createCardDto = new CreateCreditCardDTO { UserId = user.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _creditCardServicesMock.UpdateInvoice(card.Id);

        // Assert
        Assert.IsType<decimal>(result);
    }

    [Fact]
    public void AddUser_ShouldReturnTrue_WhenUserAdded()
    {
        // Arrange
        CreateUserDTO createUserDto1 = new CreateUserDTO { UserName = "User1", Email = "user1@example.com", Password = "password" };
        UserDTO user1 = _userServicesMock.Create(createUserDto1);
        CreateUserDTO createUserDto2 = new CreateUserDTO { UserName = "User2", Email = "user2@example.com", Password = "password" };
        UserDTO user2 = _userServicesMock.Create(createUserDto2);
        CreateCreditCardDTO createCardDto = new CreateCreditCardDTO { UserId = user1.Id, CardName = "Test Card", ExpiresAt = DateTime.Now.AddYears(1), Limit = 1000.00m };
        CreditCardDTO card = _creditCardServicesMock.CreateCreditCard(createCardDto);

        // Act
        var result = _creditCardServicesMock.AddUser(card.Id, user2.Id);

        // Assert
        Assert.True(result);
        Assert.True(_creditCardServicesMock.IsCardUser(card.Id, user2.Id));
    }
}
