using SmartSchoolManagementSystem.Core.DTOs.Identity;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(LoginDto loginDto);
        Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> LogoutAsync();
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
