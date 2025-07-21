using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ExpenseTracker.Model.DTOs;

namespace ExpenseTracker.Services
{
    public class JwtService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration config)
        {
            var jwtSection = config.GetSection("Jwt");

            var secretKey = jwtSection["Key"] ?? throw new KeyNotFoundException("JWT key not found in configuration.");
            _issuer = jwtSection["Issuer"] ?? throw new KeyNotFoundException("JWT issuer not found in configuration.");
            _audience = jwtSection["Audience"] ?? throw new KeyNotFoundException("JWT audience not found in configuration.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string GenerateToken(UserDto user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            };
        }
    }
}
