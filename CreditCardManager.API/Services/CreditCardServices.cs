using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public bool CardIdExists(int cardId)
        {
            return _context.CreditCards.Any(c => c.Id == cardId);
        }

        public CreditCardDTO? GetCreditCard(int id)
        {
            CreditCardDTO? card = _context.CreditCards
                .Where(c => c.Id == id)
                .Select(c => new CreditCardDTO
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    CardName = c.CardName,
                    ExpiresAt = c.ExpiresAt,
                    Invoice = c.Invoice.ToString("F2"),
                    Limit = c.Limit.ToString("F2")
                })
                .FirstOrDefault();

            return card;
        }

        public List<CreditCardDTO> GetUserCreditCards(int userId)
        {
            List<CreditCardDTO> cards = _context.CreditCards
                .Where(c => c.UserId == userId)
                .Select(c => new CreditCardDTO
                {
                    Id = c.Id,
                    CardName = c.CardName,
                    ExpiresAt = c.ExpiresAt,
                    Invoice = c.Invoice.ToString("F2"),
                    Limit = c.Limit.ToString("F2")
                })
                .ToList();

            return cards;
        }

        public CreditCardDTO CreateCreditCard(CreateCreditCardDTO createDTO)
        {
            bool userExists = _userServices.UserIdExists(createDTO.UserId);
            
            CreditCardModel createCard = new()
            {
                UserId = createDTO.UserId,
                CardName = createDTO.CardName,
                ExpiresAt = createDTO.ExpiresAt,
                Limit = createDTO.Limit
            };

            if (!userExists) throw new Exception("This user does not exist.");

            EntityEntry<CreditCardModel> card = _context.CreditCards.Add(createCard);

            _cardUserServices.CreateCardUser(new()
            {
                CardId = card.Entity.Id,
                UserId = card.Entity.UserId
            });

            _context.SaveChanges();

            return new CreditCardDTO
            {
                Id = card.Entity.Id,
                CardName = card.Entity.CardName,
                ExpiresAt = card.Entity.ExpiresAt,
                Invoice = card.Entity.Invoice.ToString(),
                Limit = card.Entity.Limit.ToString(),
                UserId = card.Entity.UserId
            };
        }

        public bool DeleteCreditCard(int cardId)
        {
            CreditCardModel? card = _context.CreditCards.FirstOrDefault(c => c.Id == cardId);

            if (card == null) return false;

            _context.CreditCards.Remove(card);
            _context.SaveChanges();

            return true;
        }


        public decimal AddToInvoice(int cardId, decimal value)
        {
            CreditCardModel? card = _context.CreditCards.FirstOrDefault(card => card.Id == cardId)
                ?? throw new Exception("This Credit Card does not exist.");

            card.Invoice += value;

            _context.CreditCards.Update(card);
            _context.SaveChanges();

            return card.Invoice;
        }

        public CardUsersDTO GetUsers(int cardId)
        {
            bool cardExists = CardIdExists(cardId);

            if (!cardExists) throw new Exception("This credit card does not exist.");

            CardUsersDTO cardUsers = _cardUserServices.GetCardUsers(cardId);

            return cardUsers;
        }

        public bool AddUser(int cardId, int userId)
        {
            bool cardExists = CardIdExists(cardId);
            bool userExists = _userServices.UserIdExists(userId);

            if (!cardExists || !userExists) throw new Exception("This credit card or user does not exist.");

            CreateCardUserDTO create = new()
            {
                CardId = cardId,
                UserId = userId
            };

            return _cardUserServices.CreateCardUser(create);
        }
    }
}