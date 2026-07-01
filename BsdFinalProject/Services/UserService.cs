using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.IRepository; // הוספנו את ה-using של הממשקים
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using BsdFinalProject.IService;

namespace BsdFinalProject.Services
{
    public class UserService : IUserService
    {
        // שינוי מ-UserRepository ל-IUserRepository
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        // עדכון הבנאי לקבלת הממשק (Interface)
        public UserService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<(bool Success, string? Token, string? Error)> UserRegister(CreateUserDto dto)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(dto.EMail) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, null, "Email and password are required.");

            var existing = await _repo.GetByEmail(dto.EMail);
            if (existing != null) return (false, null, "Email already in use.");

            // hash password
            var hashed = HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                EMail = dto.EMail,
                Phone = dto.Phone,
                Address = dto.Address,
                Password = hashed,
                Role = Role.User
            };

            var created = await _repo.CreateUser(user);

            var token = CreateToken(created);
            return (true, token, null);
        }

        // Login implementation
        public async Task<(bool Success, string? Token, string? Error)> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.EMail) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, null, "Email and password are required.");

            var user = await _repo.GetByEmail(dto.EMail);
            if (user == null) return (false, null, "Invalid credentials.");

            if (string.IsNullOrEmpty(user.Password) || !VerifyPassword(user.Password, dto.Password))
                return (false, null, "Invalid credentials.");

            var token = CreateToken(user);
            return (true, token, null);
        }

        private string CreateToken(User user)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;

            if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT Key not configured.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EMail ?? string.Empty),
                new Claim("name", user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // PBKDF2 hashing (salt + hash stored as: iterations.saltBase64.hashBase64)
        private static string HashPassword(string password, int iterations = 100_000)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(hash.Length);

            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}