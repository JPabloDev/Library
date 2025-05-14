using LibraryApi.Models;

namespace LibraryApi.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        Task<User?> AuthenticateAsync(UserLoginDto loginDto);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}