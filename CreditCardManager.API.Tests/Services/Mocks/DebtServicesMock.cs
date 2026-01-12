using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;

using CreditCardManager.Tests.Data;

namespace CreditCardManager.API.Tests.Services.Mocks;

public class DebtServicesMock : IDebtServices
{
    private readonly IDebtServices _debtServices;

    public DebtServicesMock(CreditCardManagerDbContext? context = null)
    {
        CreditCardManagerDbContext _context = context ??
            new SqliteInMemoryController().CreateContext();

        _debtServices = new DebtServices(_context);
    }

    public bool CreateDebt(CreateDebtDTO debtDTO)
    {
        return _debtServices.CreateDebt(debtDTO);
    }

    public bool DeleteDebt(int debtId)
    {
        return _debtServices.DeleteDebt(debtId);
    }

    public List<DebtDTO> GetCardDebts(int cardId)
    {
        return _debtServices.GetCardDebts(cardId);
    }

    public DebtDTO? GetDebt(int debtId)
    {
        return _debtServices.GetDebt(debtId);
    }

    public bool UpdateDebt(int debtId, UpdateDebtDTO debtDTO)
    {
        return _debtServices.UpdateDebt(debtId, debtDTO);
    }

}
