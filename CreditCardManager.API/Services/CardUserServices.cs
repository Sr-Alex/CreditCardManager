using CreditCardManager.Data;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Models;

namespace CreditCardManager.Services
{
    public class CardUserServices : ICardUserServices
    {
        #region Fields
        private readonly CreditCardManagerDbContext _context;
        #endregion

        #region Constructor
        public CardUserServices(CreditCardManagerDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        public bool CardUserExists(int cardId, int userId)
        {
            return _context.CardUsers.Any(cUser => cUser.CardId == cardId && cUser.UserId == userId);
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
                        UserId = cardU.u.Id,
                        UserName = cardU.u.UserName,
                        DebtsCount = d.Count(d => d.UserId == cardU.u.Id),
                        PendingDebts = d.Count(d => d.UserId == cardU.u.Id && d.IsPaid)
                    })
                .ToList();

            return cardUsers;
        }
        #endregion
    }
}