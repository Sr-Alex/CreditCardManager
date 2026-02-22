using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface ICreditCardServices
    {
        bool IsUserOwnerOfCard(int cardId, int userId);
        bool IsCardUser(int cardId, int userId);
        bool CardIdExists(int cardId);
        CreditCardDTO CreateCreditCard(CreateCreditCardDTO creditCardDTO);
        CreditCardDTO UpdateCreditCard(int id, UpdateCreditCardDTO creditCardDTO);
        bool DeleteCreditCard(int cardId);
        CreditCardDTO? GetCreditCard(int id);
        List<CreditCardDTO> GetUserCreditCards(int userId);
        decimal UpdateInvoice(int cardId);
        bool AddUser(int cardId, string userEmail);
        bool RemoveUser(int cardId, int userId);
    }
}