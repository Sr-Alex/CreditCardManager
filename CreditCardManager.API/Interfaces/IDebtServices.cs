using CreditCardManager.DTOs;

namespace CreditCardManager.Interfaces
{
    public interface IDebtServices
    {
        bool IsDebtOwner(int debtId, int userId);
        DebtDTO? GetDebt(int debtId);
        List<DebtDTO> GetCardDebts(int cardId);
        bool CreateDebt(CreateDebtDTO debtDTO);
        DebtDTO UpdateDebt(int debtId, UpdateDebtDTO debtDTO);
        bool DeleteDebt(int debtId);
        bool PayDebt(int debtId);
    }
}