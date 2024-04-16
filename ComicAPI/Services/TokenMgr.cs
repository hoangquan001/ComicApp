using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ComicAPI.Services
{
    public interface ITokenMgr
    {
        string CreateToken(int user_id);
        bool IsTokenBlackList(string token);
        bool AddTokenToBlackList(string token);

    }
    public class TokenMgr : ITokenMgr
    {
        private readonly IConfiguration _config;
        public TokenMgr(IConfiguration config)
        {
            _config = config;
        }
        private HashSet<string> _blackListToken = new HashSet<string>();


        public bool IsTokenBlackList(string token)
        {
            string hashedToken = HashToken(token);
            return _blackListToken.Contains(hashedToken);
        }

        private string HashToken(string token)
        {
            using var hash = SHA256.Create();
            var hashedToken = hash.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedToken);
        }
        public string CreateToken(int user_id)
        {
            //TODO: Implement Realistic Implementation
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, $"{user_id}"),
            new Claim(ClaimTypes.Role, $"User"),
            new Claim(ClaimTypes.Name, $"Hoàng Anh Quân"),
            };
            // Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials cred = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken tokenjwt = new
            (
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: cred
            );
            return tokenHandler.WriteToken(tokenjwt);
        }

        public bool AddTokenToBlackList(string token)
        {
            string hashedToken = HashToken(token);
            return _blackListToken.Add(hashedToken);
        }
    }



}