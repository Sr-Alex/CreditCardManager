using CreditCardManager.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CreditCardManager.Tests.Data
{
    public class SqliteInMemoryController
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<CreditCardManagerDbContext> _contextOptions;

        public SqliteInMemoryController()
        {
            _connection = new("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<CreditCardManagerDbContext>()
                .UseSqlite(_connection)
                .Options;


            using CreditCardManagerDbContext context = new(_contextOptions);
            context.Database.EnsureCreated();
        }

        public CreditCardManagerDbContext CreateContext() => new(_contextOptions);

        public void Dispose() => _connection.Dispose();

    }
}