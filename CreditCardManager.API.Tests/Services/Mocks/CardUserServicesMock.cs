using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;
using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services.Mocks;

public class CardUserServicesMock : ICardUserServices
{
    private readonly ICardUserServices _cardUserServices;

    public CardUserServicesMock(CreditCardManagerDbContext? context = null)
    {
        CreditCardManagerDbContext _context = context ??
            new SqliteInMemoryController().CreateContext();

        _cardUserServices = new CardUserServices(_context);
    }

    public bool CreateCardUser(CreateCardUserDTO cardUserDTO)
    {
        return _cardUserServices.CreateCardUser(cardUserDTO);
    }

    public CardUsersDTO GetCardUsers(int cardId)
    {
        return _cardUserServices.GetCardUsers(cardId);
    }
}