using LibraryApi.Data;
using LibraryApi.Models.DTOs;
using LibraryApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly LibraryDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LibraryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Users?> AuthenticateAsync(UserLoginDto loginDto)
        {
            var user = _context.Usuarios.FirstOrDefaultAsync(u => u.Usuario == loginDto.Username).Result;
            if (user == null)
            { 
                return null;
            }

            // Comparación directa de contraseñas (solo para fines educativos)
            return (user.Contrasena == loginDto.Password) ? user : null;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) 
                return false;

            if (user.Contrasena != currentPassword)
                return false;

            user.Contrasena = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateToken(Users user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("Admin", user.Admin.ToString().ToLower())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}