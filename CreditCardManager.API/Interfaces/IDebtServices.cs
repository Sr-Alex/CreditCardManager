using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface IDebtServices
    {
        DebtDTO? GetDebt(int debtId);
        List<DebtDTO> GetDebts(int userId, int cardId);
        bool CreateDebt(CreateDebtDTO debtDTO);
    }
}