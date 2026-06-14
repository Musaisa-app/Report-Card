using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Service for JWT token generation and management
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(System.Security.Claims.ClaimTypes.Email, user.Email),
                new(System.Security.Claims.ClaimTypes.Name, user.FullName),
                new("PhoneNumber", user.PhoneNumber ?? string.Empty),
                new("RoleId", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "AbuAshraf",
                audience: "AbuAshrafUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress, string userAgent)
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var token = Convert.ToBase64String(randomNumber);

            return new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays),
                IsRevoked = false,
                CreatedDate = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = "AbuAshraf",
                    ValidateAudience = true,
                    ValidAudience = "AbuAshrafUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<string, object> GetTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = new Dictionary<string, object>();

            foreach (var claim in jwtToken.Claims)
            {
                claims[claim.Type] = claim.Value;
            }

            return claims;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            // Implementation in persistence layer
            await Task.CompletedTask;
        }
    }
}
