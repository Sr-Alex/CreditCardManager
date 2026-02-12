using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICardUserServices
    {
        bool CardUserExists(int cardId, int userId);
        List<CardUserDTO> GetCardUsers(int cardId);
        bool CreateCardUser(CreateCardUserDTO cardUserDTO);
        bool DeleteCardUser(int cardUserId);
    }
}