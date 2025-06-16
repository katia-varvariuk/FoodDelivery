using FoodDelivery.BLL.DTOs;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterUserDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginUserDto loginDto);
        Task<AuthResultDto> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> UpdateUserAsync(string userId, UserDto userDto);
    }
}