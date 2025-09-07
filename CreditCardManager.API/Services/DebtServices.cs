using Microsoft.EntityFrameworkCore;

using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;

namespace CreditCardManager.Services
{
    public class DebtServices : IDebtServices
    {
        private readonly CreditCardManagerDbContext _context;
        private readonly UserServices _userServices;
        private readonly CreditCardServices _creditCardServices;

        public DebtServices(CreditCardManagerDbContext context)
        {
            _context = context;
            _userServices = new UserServices(_context);
            _creditCardServices = new CreditCardServices(_context);
        }

        public bool CreateDebt(CreateDebtDTO debtDTO)
        {
            bool userExists = _userServices.UserIdExists(debtDTO.UserId);
            bool cardExists = _creditCardServices.CardIdExists(debtDTO.CardId);

            if (!userExists || !cardExists) throw new Exception("This user or credit card does not exist.");

            _context.Debts.Add(new DebtModel
            {
                UserId = debtDTO.UserId,
                CardId = debtDTO.CardId,
                Label = debtDTO.Label,
                Date = debtDTO.Date,
                Value = debtDTO.Value
            });

            _creditCardServices.AddToInvoice(debtDTO.CardId, debtDTO.Value);
            _context.SaveChanges();

            return true;
        }

        public DebtDTO? GetDebt(int debtId)
        {
            DbSet<DebtModel> debts = _context.Debts;

            DebtDTO? debt = _context.Debts
                .Where(debt => debt.Id == debtId)
                .Select(debt => new DebtDTO
                {
                    Label = debt.Label,
                    Value = debt.Value,
                    Date = debt.Date,
                    User = debt.UserId,
                    Card = debt.CardId
                })
                .FirstOrDefault();

            if (debt == null)
                return null;

            return debt;
        }

        public List<DebtDTO> GetDebts(int userId, int cardId)
        {
            throw new NotImplementedException();
        }
    }
}