using System;
using System.Linq;
using System.Threading.Tasks;
using AnalysisAPI.Data;
using AnalysisAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace AnalysisAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly string _jwtSecret;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                     ?? throw new Exception("JWT_SECRET_KEY is not set in the environment variables.");
        }

        public async Task<(bool Success, string Message, int UserId)> RegisterUser(User user)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
                return (false, "Username or Email already exists", 0);

            user.Password = HashPassword(user.Password);

            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return (true, "User registered successfully", user.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Error occurred during registration: {ex.Message}", 0);
            }
        }

        public async Task<string> ValidateUser(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<(bool Success, string Message)> UpdateUser(int id, UserUpdateDTO updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return (false, "User not found");

            if (await _dbContext.Users.AnyAsync(u =>
                    (u.Username == updatedUser.Username && u.Id != id) ||
                    (u.Email == updatedUser.Email && u.Id != id)))
            {
                return (false, "Username or Email already exists");
            }

            if (!string.IsNullOrWhiteSpace(updatedUser.Username))
                user.Username = updatedUser.Username;

            if (!string.IsNullOrWhiteSpace(updatedUser.Email))
                user.Email = updatedUser.Email;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                user.Password = HashPassword(updatedUser.Password);

            await _dbContext.SaveChangesAsync();
            return (true, "User updated successfully");
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
