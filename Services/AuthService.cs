using ApiCrud.Data.Repositories;
using ApiCrud.DTOs;
using ApiCrud.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCrud.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<object?> RegisterAsync(RegisterRequestDTO dto)
        {
            try
            {
                var existing = await _userRepository.GetUserByUsernameAsync(dto.Username);
                if (existing != null) return null;

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    FirstName = dto.FirstName ?? string.Empty,
                    LastName = dto.LastName ?? string.Empty,
                    Email = dto.Email ?? string.Empty
                };

                await _userRepository.CreateUserAsync(user);
                return new { user.Id, user.Username, user.Email };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro no RegisterAsync: {ex.Message}", ex);
            }
        }
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(dto.Username);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;
            var token = GenerateJwtToken(user);
            return new LoginResponseDTO
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                Username = user.Username
            };
        }
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
