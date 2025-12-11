using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICreditCardServices
    {
        bool IsUserOwnerOfCard(int cardId, int userId);
        bool CardIdExists(int cardId);
        CreditCardDTO? GetCreditCard(int id);
        List<CreditCardDTO> GetUserCreditCards(int userId);
        bool AddUser(int cardId, int userId);
        CreditCardDTO CreateCreditCard(CreateCreditCardDTO creditCardDTO);
        bool DeleteCreditCard(int cardId);
    }
}