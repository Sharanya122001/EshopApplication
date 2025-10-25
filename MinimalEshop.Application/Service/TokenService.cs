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
        public TokenService(IOptionsMonitor<JwtSettings> jwtSettings)
            {
            if (jwtSettings == null)
                throw new ArgumentNullException(nameof(jwtSettings));

            _jwtSettings = jwtSettings.CurrentValue;

            if (_jwtSettings == null || string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new ArgumentException("JWT Key is missing in configuration.");

            _keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            }

        public string GenerateToken(string username, string role)
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(_keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
