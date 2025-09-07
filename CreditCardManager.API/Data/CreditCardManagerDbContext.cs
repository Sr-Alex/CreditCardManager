using CreditCardManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditCardManager.Data
{
    public class CreditCardManagerDbContext : DbContext
    {

        public DbSet<UserModel> Users { get; set; }
        public DbSet<CreditCardModel> CreditCards { get; set; }
        public DbSet<DebtModel> Debts { get; set; }
        public DbSet<CardUserModel> CardUsers { get; set; }

        private readonly IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public CreditCardManagerDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}