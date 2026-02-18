using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;

namespace CreditCardManager.Services
{
    public class CardUserServices : ICardUserServices
    {
        private readonly CreditCardManagerDbContext _context;

        public CardUserServices(CreditCardManagerDbContext context)
        {
            _context = context;
        }

        public bool CardUserExists(int cardId, int userId)
        {
            return _context.CardUsers.Any(cardUser => cardUser.CardId == cardId && cardUser.UserId == userId);
        }

        public List<CardUserDTO> GetCardUsers(int cardId)
        {
            List<CardUserDTO> cardUsers = _context.CardUsers
                .Where(cardU => cardU.CardId == cardId)
                .Join(_context.Users,
                    cardU => cardU.UserId,
                    u => u.Id,
                    (cardU, u) => new { cardU, u })
                .GroupJoin(_context.Debts,
                    cardU => cardU.cardU.CardId,
                    d => d.CardId,
                    (cardU, d) => new CardUserDTO
                    {
                        Id = cardU.cardU.Id,
                        UserId = cardU.u.Id,
                        UserName = cardU.u.UserName,
                        DebtsCount = d.Count(d => d.UserId == cardU.u.Id),
                        PendingDebts = d.Count(d => d.UserId == cardU.u.Id && !d.IsPaid),
                        TotalAmount = d.Where(d => d.UserId == cardU.u.Id).Sum(d => d.Value),
                        AmountToPay = d.Where(d => d.UserId == cardU.u.Id && !d.IsPaid).Sum(d => d.Value),
                    })
                .ToList();

            return cardUsers;
        }

        public bool CreateCardUser(CreateCardUserDTO createDTO)
        {
            if (CardUserExists(createDTO.CardId, createDTO.UserId)) return false;

            _context.Add(new CardUserModel
            {
                CardId = createDTO.CardId,
                UserId = createDTO.UserId
            });
            _context.SaveChanges();

            return true;
        }

        public bool DeleteCardUser(int cardUserId)
        {
            CardUserModel? cardUser = _context.CardUsers
                .FirstOrDefault(cardUser => cardUser.Id == cardUserId);

            if (cardUser == null) return false;

            _context.CardUsers.Remove(cardUser);
            _context.SaveChanges();

            return true;
        }

    }
}