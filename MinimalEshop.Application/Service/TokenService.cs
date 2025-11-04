using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalEshop.Application.Service
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly byte[] _keyBytes;
        public TokenService(IConfiguration configuration)
            {
            _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
                           ?? throw new ArgumentException("JWT configuration section is missing.");

            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new ArgumentException("JWT Key is missing in configuration.");

            _keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        }

        public string GenerateToken(string userId,string username, string role)
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(_keyBytes);//read
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())//read
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes),
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }
    }
}
