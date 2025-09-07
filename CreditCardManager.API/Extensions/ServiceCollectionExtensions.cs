using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using CreditCardManager.Data;
using CreditCardManager.Interfaces;
using CreditCardManager.Services;

namespace CreditCardManager.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JWT:ValidIssuer"],
            ValidAudience = config["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config["JWT:SecureKey"] ?? ""
            ))
        };
    });

            services.AddTransient<ITokenServices, TokenServices>();

            services.AddDbContext<CreditCardManagerDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            services.AddOpenApiDocument(options =>
            {
                options.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "Insert the JWT:"
                });

                options.OperationProcessors.Add(
                    new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ICreditCardServices, CreditCardServices>();
            services.AddScoped<IDebtServices, DebtServices>();
            services.AddScoped<ICardUserServices, CardUserServices>();

            return services;
        }
    }
}