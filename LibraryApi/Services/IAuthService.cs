using LibraryApi.Models.DTOs;
using LibraryApi.Models.Entities;

namespace LibraryApi.Services
{
    public interface IAuthService
    {
        string GenerateToken(Users user);
        Task<Users?> AuthenticateAsync(UserLoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}