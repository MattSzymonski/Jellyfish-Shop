using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class SecurityService
    {
        private readonly ApplicationSettings settings;
        private readonly ILogger<SecurityService> logger;

        public SecurityService(ApplicationSettings settings, ILogger<SecurityService> logger)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public string GenerateTokens(string userId, string email, Claim[] claims = null)
        {
            if (claims == null) {
                claims = new Claim[] 
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Access token
            var accessToken = new JwtSecurityToken(
                settings.JwtSettings.Issuer,
                settings.JwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(settings.JwtSettings.AccessTokenExpiration),
                signingCredentials: credentials
            );
            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            // Return tokens
            return accessTokenString;
        }

        private (ClaimsPrincipal, JwtSecurityToken) DecodeToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new SecurityTokenException("Invalid token");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.Secret));
            var tokenValidationParameters = new TokenValidationParameters 
            {
                ValidateIssuer = true,
                ValidIssuer = settings.JwtSettings.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidAudience = settings.JwtSettings.Audience,
                ValidateAudience = true,
                ValidateLifetime = true
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }
    }
}
