

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config) // Inject IConfiguration to access app settings json
        {
            _config = config;
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["JWT:SigningKey"])); // encrypt in unique way
        }
        public string CreateToken(AppUser user) // takes an AppUser object and creates a JWT token
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

// This service creates a JWT token when the user logs in.
// The token contains claims (e.g., username, email) and security credentials.
// Clients send this token with requests to authenticate and access protected routes.
