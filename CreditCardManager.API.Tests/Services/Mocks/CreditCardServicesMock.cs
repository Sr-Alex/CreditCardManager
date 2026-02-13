using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services.Mocks;

public class CreditCardServicesMock : ICreditCardServices
{
    private readonly ICreditCardServices _creditCardServices;

    public CreditCardServicesMock(CreditCardManagerDbContext? context = null)
    {
        CreditCardManagerDbContext _context = context ??
            new SqliteInMemoryController().CreateContext();

        _creditCardServices = new CreditCardServices(_context);
    }

    public bool AddUser(int cardId, string userEmail)
    {
        return _creditCardServices.AddUser(cardId, userEmail);
    }

    public bool RemoveUser(int cardId, int userId)
    {
        return _creditCardServices.RemoveUser(cardId, userId);
    }

    public decimal UpdateInvoice(int cardId)
    {
        return _creditCardServices.UpdateInvoice(cardId);
    }

    public bool CardIdExists(int cardId)
    {
        return _creditCardServices.CardIdExists(cardId);
    }

    public CreditCardDTO CreateCreditCard(CreateCreditCardDTO creditCardDTO)
    {
        return _creditCardServices.CreateCreditCard(creditCardDTO);
    }

    public bool DeleteCreditCard(int cardId)
    {
        return _creditCardServices.DeleteCreditCard(cardId);
    }

    public CreditCardDTO? GetCreditCard(int id)
    {
        return _creditCardServices.GetCreditCard(id);
    }

    public List<CreditCardDTO> GetUserCreditCards(int userId)
    {
        return _creditCardServices.GetUserCreditCards(userId);
    }

    public bool IsCardUser(int cardId, int userId)
    {
        return _creditCardServices.IsCardUser(cardId, userId);
    }

    public bool IsUserOwnerOfCard(int cardId, int userId)
    {
        return _creditCardServices.IsUserOwnerOfCard(cardId, userId);
    }
}