using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface IDebtServices
    {
        DebtDTO? GetDebt(int debtId);
        List<DebtDTO> GetCardDebts(int cardId);
        bool CreateDebt(CreateDebtDTO debtDTO);
        DebtDTO UpdateDebt(int debtId, UpdateDebtDTO debtDTO);
        bool DeleteDebt(int debtId);
    }
}