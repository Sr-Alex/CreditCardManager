using System.Configuration;
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

        private readonly IConfiguration? _configuration;

        public CreditCardManagerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public CreditCardManagerDbContext(DbContextOptions options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _configuration != null)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
    }
}