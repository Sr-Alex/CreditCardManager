using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICreditCardServices
    {
        bool IsUserOwnerOfCard(int cardId, int userId);
        CreditCardDTO? GetCreditCard(int id);
        List<CreditCardDTO> GetUserCreditCards(int userId);
        CardUsersDTO GetUsers(int cardId);
        bool AddUser(int cardId, int userId);
        bool CreateCreditCard(CreateCreditCardDTO creditCardDTO);
        bool DeleteCreditCard(int cardId);
    }
}