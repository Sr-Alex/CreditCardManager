using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICardUserServices
    {
        List<CardUserDTO> GetCardUsers(int cardId);
        bool CreateCardUser(CreateCardUserDTO cardUserDTO);
    }
}