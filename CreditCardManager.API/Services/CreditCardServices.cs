using Microsoft.EntityFrameworkCore.ChangeTracking;

using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;

namespace CreditCardManager.Services
{
    public class CreditCardServices : ICreditCardServices
    {
        private readonly CreditCardManagerDbContext _context;
        private readonly UserServices _userServices;
        private readonly CardUserServices _cardUserServices;

        public CreditCardServices(CreditCardManagerDbContext context)
        {
            _context = context;
            _userServices = new(context);
            _cardUserServices = new(context);
        }

        public bool IsUserOwnerOfCard(int cardId, int userId)
        {
            CreditCardDTO? card = GetCreditCard(cardId);
            return card != null && card.UserId == userId;
        }

        public bool IsCardUser(int cardId, int userId)
        {
            return _context.CardUsers.Any(cardUser => cardUser.CardId == cardId && cardUser.UserId == userId);
        }

        public bool CardIdExists(int cardId)
        {
            return _context.CreditCards.Any(c => c.Id == cardId);
        }

        public CreditCardDTO? GetCreditCard(int id)
        {
            CreditCardModel? card = _context.CreditCards.FirstOrDefault(c => c.Id == id);

            if (card == null) return null;

            int pendantDebtsCount = _context.Debts
                .Where(d => d.CardId == id && d.IsPaid == false)
                .Count();

            return new CreditCardDTO
            {
                Id = card.Id,
                UserId = card.UserId,
                CardName = card.CardName,
                ExpiresAt = card.ExpiresAt,
                Invoice = card.Invoice,
                Limit = card.Limit,
                PendantDebts = pendantDebtsCount
            };
        }

        public List<CreditCardDTO> GetUserCreditCards(int userId)
        {
            bool userExists = _userServices.UserIdExists(userId);

            if (!userExists) throw new Exception("This user does not exist.");

            List<CreditCardDTO> cards = _context.CreditCards
                .Where(c => c.UserId == userId)
                .GroupJoin(_context.Debts,
                    c => c.Id,
                    d => d.CardId,
                    (c, d) => new CreditCardDTO
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        CardName = c.CardName,
                        ExpiresAt = c.ExpiresAt,
                        Invoice = c.Invoice,
                        Limit = c.Limit,
                        PendantDebts = d.Count(d => d.CardId == c.Id)
                    })
                .ToList();

            return cards;
        }

        public CreditCardDTO CreateCreditCard(CreateCreditCardDTO createDTO)
        {
            bool userExists = _userServices.UserIdExists(createDTO.UserId);

            if (!userExists) throw new Exception("This user does not exist.");

            CreditCardModel createCard = new()
            {
                UserId = createDTO.UserId,
                CardName = createDTO.CardName,
                ExpiresAt = createDTO.ExpiresAt,
                Limit = createDTO.Limit
            };

            EntityEntry<CreditCardModel> card = _context.CreditCards.Add(createCard);
            _context.SaveChanges();

            _cardUserServices.CreateCardUser(new CreateCardUserDTO
            {
                CardId = card.Entity.Id,
                UserId = createDTO.UserId
            });

            return GetCreditCard(card.Entity.Id)!;
        }

        public bool DeleteCreditCard(int cardId)
        {
            CreditCardModel? card = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);

            if (card == null) return false;

            _context.CreditCards.Remove(card);
            _context.SaveChanges();

            return true;
        }

        public decimal UpdateInvoice(int cardId)
        {
            CreditCardModel? card = _context.CreditCards.FirstOrDefault(card => card.Id == cardId)
                ?? throw new Exception("This Credit Card does not exist.");

            card.Invoice = _context.Debts
                .Where(debt => debt.CardId == cardId && debt.IsPaid == false)
                .Sum(debt => debt.Value);

            _context.CreditCards.Update(card);
            _context.SaveChanges();

            return card.Invoice;
        }

        public bool AddUser(int cardId, string userEmail)
        {
            bool cardExists = CardIdExists(cardId);
            UserDTO? user = _userServices.GetUserByEmail(userEmail);

            if (!cardExists || user == null) throw new Exception("This credit card or user does not exist.");

            CreateCardUserDTO createCardUser = new()
            {
                CardId = cardId,
                UserId = user.Id
            };

            return _cardUserServices.CreateCardUser(createCardUser);
        }

        public bool RemoveUser(int cardId, int userId)
        {
            bool cardExists = CardIdExists(cardId);
            bool userExists = _userServices.UserIdExists(userId);

            if (!cardExists || !userExists) throw new Exception("This credit card or user does not exist.");

            return _cardUserServices.DeleteCardUser(cardId, userId);
        }
    }
}