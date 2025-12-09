using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using CreditCardManager.DTOs;
using CreditCardManager.Interfaces;
using CreditCardManager.Validators;

namespace CreditCardManager.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _JWThandler;

        public TokenServices(IConfiguration configuration)
        {
            _config = configuration;
            _JWThandler = new();
        }

        public UserDTO DecodeUserToken(string JWTtoken)
        {
            const string bearerPrefix = "bearer ";

            if (JWTtoken.Trim().StartsWith(bearerPrefix))
            {
                JWTtoken = JWTtoken[bearerPrefix.Length..].Trim();
            }

            JwtSecurityToken token = _JWThandler.ReadJwtToken(JWTtoken);

            Claim? userId = token.Claims.FirstOrDefault(c => c.Type == "id");
            Claim? userName = token.Claims.FirstOrDefault(c => c.Type == "userName");
            Claim? userEmail = token.Claims.FirstOrDefault(c => c.Type == "email");

            if (userId == null || userEmail == null || userName == null)
                throw new Exception("Invalid user claims.");

            UserDTO user = new()
            {
                Id = int.Parse(userId.Value),
                UserName = userName.Value,
                Email = userEmail.Value
            };

            return user;
        }

        public string GenerateUserToken(UserDTO userDTO)
        {
            if (ModelStateValidator.Validate(userDTO).Count != 0)
            {
                throw new InvalidDataException("Invalid user data.");
            }

            var secureKey = _config.GetSection("JWT").GetValue<string>("SecureKey");
            if (string.IsNullOrEmpty(secureKey))
            {
                throw new InvalidOperationException("Secure key configuration is missing.");
            }

            SigningCredentials credentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey)),
                SecurityAlgorithms.HmacSha256Signature
            );

            JwtSecurityToken jwt = _JWThandler.CreateJwtSecurityToken(
                issuer: _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
                audience: _config.GetSection("JWT").GetValue<string>("ValidAudience"),
                subject: GenerateUserClaims(userDTO),
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return _JWThandler.WriteToken(jwt);
        }

        private static ClaimsIdentity GenerateUserClaims(UserDTO userDTO)
        {
            Claim[] claims = [
                new Claim("id", userDTO.Id.ToString()),
                new Claim("userName", userDTO.UserName),
                new Claim(ClaimTypes.Email, userDTO.Email)
            ];

            return new ClaimsIdentity(claims);
        }
    }
}