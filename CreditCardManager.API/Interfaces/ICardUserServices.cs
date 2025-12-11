using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICardUserServices
    {
        CardUsersDTO GetCardUsers(int cardId);
        bool CreateCardUser(CreateCardUserDTO cardUserDTO);
    }
}