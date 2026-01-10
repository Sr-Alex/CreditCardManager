using Microsoft.EntityFrameworkCore;

using CreditCardManager.Data;

namespace CreditCardManager.API.Extensions
{
    public static class DbServicesExtensions
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            if (environment.IsDevelopment()) AddSqliteDb(services, connectionString ?? "");
            else AddSqlServerDb(services, connectionString ?? "");

            return services;
        }

        private static void AddSqliteDb(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CreditCardManagerDbContext>(options =>
                options.UseSqlite(connectionString)
            );
        }

        private static void AddSqlServerDb(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CreditCardManagerDbContext>(options =>
                options.UseSqlServer(connectionString)
            );
        }
    }
}