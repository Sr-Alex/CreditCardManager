using Microsoft.AspNetCore.Identity;

using CreditCardManager.Data;
using CreditCardManager.Models;
using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;

namespace CreditCardManager.Services
{
    public class UserServices : IUserServices
    {
        private readonly CreditCardManagerDbContext _context;
        private readonly PasswordHasher<UserModel> _userHasher;

        public UserServices(CreditCardManagerDbContext context)
        {
            _context = context;
            _userHasher = new PasswordHasher<UserModel>();
        }

        public bool UserIdExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

        public bool EmailAlreadyExists(string Email)
        {
            bool UserExists = _context.Users.Any(u => u.Email == Email);

            return UserExists;
        }

        public List<UserDTO> GetUsers()
        {
            List<UserDTO> users = _context.Users
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                })
                .ToList();

            return users;
        }

        public UserDTO? GetUser(int id)
        {
            UserDTO? user = _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
            })
            .FirstOrDefault();

            return user;
        }

        public UserDTO Login(LoginUserDTO loginDTO)
        {
            UserModel? user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email)
                ?? throw new Exception("User does not exist.");

            PasswordVerificationResult passwordsMatch = _userHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);

            if (passwordsMatch == PasswordVerificationResult.Failed)
            {
                throw new Exception("Incorrect password.");
            }

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public UserDTO Create(CreateUserDTO createDTO)
        {
            if (EmailAlreadyExists(createDTO.Email))
            {
                throw new Exception("User already exists.");
            }

            UserModel user = new()
            {
                UserName = createDTO.UserName,
                Email = createDTO.Email,
                Password = createDTO.Password
            };

            user.Password = _userHasher.HashPassword(user, user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}