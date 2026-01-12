using CreditCardManager.DTOs;
using CreditCardManager.API.Tests.Services.Mocks;

namespace CreditCardManager.Tests.Services;

public class TokenServicesTests
{
    private readonly TokenServicesMock _tokenServicesMock;

    public TokenServicesTests()
    {
        _tokenServicesMock = new TokenServicesMock();
    }

    [Fact]
    public void GeneratesConsistentToken()
    {
        UserDTO user = new()
        {
            Id = 1,
            Email = "testuser@gmail.com",
            UserName = "TestUser"
        };

        string token = _tokenServicesMock.GenerateUserToken(user);
        var decodedToken = _tokenServicesMock.DecodeUserToken(token);

        Assert.IsType<UserDTO>(decodedToken);
        Assert.Equivalent(user, decodedToken, true);
    }

    [Fact]
    public void OnlyGeneratesWithCorrectPayload()
    {
        UserDTO user = new()
        {
            Id = 0,
            Email = "",
            UserName = ""
        };

        Assert.Throws<InvalidDataException>(() => _tokenServicesMock.GenerateUserToken(user));
    }

}
