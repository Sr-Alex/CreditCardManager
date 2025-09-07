using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface IUserServices
    {
        bool UserIdExists(int id);
        bool EmailAlreadyExists(string Email);
        List<UserDTO> GetUsers();
        UserDTO? GetUser(int id);

        UserDTO Login(LoginUserDTO loginDTO);
        UserDTO Create(CreateUserDTO userDTO);
    }
}