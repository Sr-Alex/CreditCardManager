using Microsoft.EntityFrameworkCore;

using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;
using System.Data;

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
                Value = debtDTO.Value,
                IsPaid = debtDTO.IsPaid,
            });
            _context.SaveChanges();

            _creditCardServices.UpdateInvoice(debtDTO.CardId);

            return true;
        }

        public DebtDTO? GetDebt(int debtId)
        {
            DebtModel? debt = _context.Debts
                .Where(debt => debt.Id == debtId).FirstOrDefault();

            if (debt == null) return null;

            return new DebtDTO
            {
                Id = debt.Id,
                User = debt.UserId,
                Card = debt.CardId,
                Label = debt.Label,
                Value = debt.Value,
                Date = debt.Date,
                IsPaid = debt.IsPaid
            };
        }

        public List<DebtDTO> GetCardDebts(int cardId)
        {
            List<DebtDTO> debts = _context.Debts.Where(debt => debt.CardId == cardId)
                .Select(
                    debt => new DebtDTO
                    {
                        Id = debt.Id,
                        User = debt.UserId,
                        Card = debt.CardId,
                        Label = debt.Label,
                        Value = debt.Value,
                        Date = debt.Date,
                        IsPaid = debt.IsPaid
                    }
                ).ToList();

            return debts;
        }

        public DebtDTO UpdateDebt(int debtId, UpdateDebtDTO debtData)
        {
            DebtModel? debt = _context.Debts.FirstOrDefault(d => d.Id == debtId) ?? throw new Exception("Debt not found.");

            if (debtData.Label != null) debt.Label = debtData.Label;
            if (debtData.Date.HasValue) debt.Date = debtData.Date.Value;
            if (debtData.Value.HasValue) debt.Value = debtData.Value.Value;

            _context.Debts.Update(debt);
            _context.SaveChanges();

            _creditCardServices.UpdateInvoice(debt.CardId);

            return GetDebt(debtId)!;
        }

        public bool DeleteDebt(int debtId)
        {
            DebtModel? debt = _context.Debts.FirstOrDefault(d => d.Id == debtId);
            if (debt == null) return false;

            _context.Debts.Remove(debt);
            _context.SaveChanges();

            _creditCardServices.UpdateInvoice(debt.CardId);

            return true;
        }

        public bool PayDebt(int debtId)
        {
            DebtModel? debt = _context.Debts.FirstOrDefault(d => d.Id == debtId);

            if (debt == null) return false;

            debt.IsPaid = true;
            _context.Debts.Update(debt);
            _context.SaveChanges();

            _creditCardServices.UpdateInvoice(debt.CardId);

            return true;
        }
    }
}