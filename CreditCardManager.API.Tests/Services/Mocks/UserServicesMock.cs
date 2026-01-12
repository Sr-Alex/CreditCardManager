using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services.Mocks;

public class UserServicesMock : IUserServices
{
    private readonly IUserServices _userServices;

    public UserServicesMock(CreditCardManagerDbContext? context = null)
    {
        CreditCardManagerDbContext _context = context ??
            new SqliteInMemoryController().CreateContext();

        _userServices = new UserServices(_context);
    }

    public UserDTO Create(CreateUserDTO user)
    {
        return _userServices.Create(user);
    }

    public UserDTO? GetUser(int userId)
    {
        return _userServices.GetUser(userId);
    }

    public UserDTO Login(LoginUserDTO login)
    {
        return _userServices.Login(login);
    }

    public bool UserIdExists(int id)
    {
        return _userServices.UserIdExists(id);
    }

    public bool EmailAlreadyExists(string Email)
    {
        return _userServices.EmailAlreadyExists(Email);
    }

    public List<UserDTO> GetUsers()
    {
        return _userServices.GetUsers();
    }

    public void Delete(int id)
    {
        _userServices.Delete(id);
    }
}