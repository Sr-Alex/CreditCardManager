using CreditCardManager.API.Tests.Services.Mocks;
using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.Tests.Services;

public class UserServicesTests
{
    private readonly UserServicesMock _userServicesMock;

    private readonly CreateUserDTO _defaultUser = new()
    {
        UserName = "user01",
        Email = "user01@gmail.com",
        Password = "newtonManja"
    };

    public UserServicesTests()
    {
        SqliteInMemoryController _sqliteInMemory = new();

        CreditCardManagerDbContext _context = _sqliteInMemory.CreateContext();

        _userServicesMock = new UserServicesMock(_context);
    }

    [Fact]
    public void AddUserTest()
    {
        var response = _userServicesMock.Create(_defaultUser);
        var getUser = _userServicesMock.GetUser(response?.Id ?? 0);

        Assert.IsType<UserDTO>(response);
        Assert.NotNull(getUser);
        Assert.Equal(_defaultUser.UserName, getUser.UserName);
    }

    [Fact]
    public void AddExistingUser()
    {
        _userServicesMock.Create(_defaultUser);

        Assert.Throws<InvalidOperationException>(
            () => _userServicesMock.Create(_defaultUser)
        );
    }

    [Fact]
    public void LoginUserTest()
    {
        LoginUserDTO login = new()
        {
            Email = _defaultUser.Email,
            Password = _defaultUser.Password
        };

        _userServicesMock.Create(_defaultUser);
        var response = _userServicesMock.Login(login);

        Assert.IsType<UserDTO>(response);
    }
}
