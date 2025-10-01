using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.Tests.Services
{
    public class UserServicesTests
    {
        private readonly SqliteInMemoryController _sqliteInMemory;
        private readonly IUserServices _userServices;

        private readonly CreateUserDTO _defaultUser = new()
        {
            UserName = "user01",
            Email = "user01@gmail.com",
            Password = "newtonManja"
        };

        public UserServicesTests()
        {
            _sqliteInMemory = new();
            _userServices = new UserServices(_sqliteInMemory.CreateContext());
        }

        [Fact]
        public void AddUserTest()
        {
            var response = _userServices.Create(_defaultUser);
            var getUser = _userServices.GetUser(response?.Id ?? 0);

            Assert.IsType<UserDTO>(response);
            Assert.NotNull(getUser);
            Assert.Equal(_defaultUser.UserName, getUser.UserName);
        }

        [Fact]
        public void AddExistingUser()
        {
            _userServices.Create(_defaultUser);

            Assert.Throws<InvalidOperationException>(
                () => _userServices.Create(_defaultUser)
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

            _userServices.Create(_defaultUser);
            var response = _userServices.Login(login);

            Assert.IsType<UserDTO>(response);
        }
    }
}