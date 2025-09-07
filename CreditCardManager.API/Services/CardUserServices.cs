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

        public CardUsersDTO GetCardUsers(int cardId)
        {
            List<UserDTO> users =
                (from cardU in _context.CardUsers
                 join user in _context.Users on cardU.UserId equals user.Id
                 where cardU.CardId == cardId
                 select new UserDTO { Id = user.Id, Email = user.Email, UserName = user.UserName }
                ).ToList();

            CardUsersDTO cardUsers = new()
            {
                CardId = cardId,
                Users = users
            };

            return cardUsers;
        }
    }
}