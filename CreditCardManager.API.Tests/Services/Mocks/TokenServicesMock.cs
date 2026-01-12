using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.Tests.Data;
using Microsoft.Extensions.Configuration;

namespace CreditCardManager.API.Tests.Services.Mocks;

public class TokenServicesMock : ITokenServices
{
    private readonly Dictionary<string, string?> _JWTMock;
    private readonly IConfiguration _config;

    private readonly ITokenServices _tokenServices;

    public TokenServicesMock()
    {
        _JWTMock = new()
        {
            ["JWT:SecureKey"] = "734fbdb0-f0ec-442a-a666-51989e310926",
            ["JWT:ValidIssuer"] = "2b92d353-9368-4a03-bb1c-fc2c268c2e0d",
            ["JWT:ValidAudience"] = "7d39810d-3f12-4405-adbf-81d08d7f216c",

        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(_JWTMock)
            .Build();

        _tokenServices = new TokenServices(_config);
    }

    public UserDTO DecodeUserToken(string JWTtoken)
    {
        return _tokenServices.DecodeUserToken(JWTtoken);
    }

    public string GenerateUserToken(UserDTO userDTO)
    {
        return _tokenServices.GenerateUserToken(userDTO);
    }

}