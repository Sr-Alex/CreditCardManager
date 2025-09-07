using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ITokenServices
    {
        string GenerateUserToken(UserDTO userDTO);
        UserDTO DecodeUserToken(string JWTtoken);
    }
}