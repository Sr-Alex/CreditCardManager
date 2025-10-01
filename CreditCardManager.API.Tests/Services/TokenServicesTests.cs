using Microsoft.Extensions.Configuration;

using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.DTOs;

namespace CreditCardManager.Tests.Services
{
    public class TokenServicesTests
    {
        private readonly Dictionary<string, string?> _JWT = new()
        {
            ["JWT:SecureKey"] = "734fbdb0-f0ec-442a-a666-51989e310926",
            ["JWT:ValidIssuer"] = "2b92d353-9368-4a03-bb1c-fc2c268c2e0d",
            ["JWT:ValidAudience"] = "7d39810d-3f12-4405-adbf-81d08d7f216c",

        };
        private readonly IConfiguration _config;
        private readonly ITokenServices _tokenServices;

        public TokenServicesTests()
        {
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(_JWT)
                .Build();
            _tokenServices = new TokenServices(_config);
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

            string token = _tokenServices.GenerateUserToken(user);
            var decodedToken = _tokenServices.DecodeUserToken(token);

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

            Assert.Throws<InvalidDataException>(() => _tokenServices.GenerateUserToken(user));
        }

    }
}