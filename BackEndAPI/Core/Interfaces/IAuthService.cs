using BackEndAPI.Core.Dtos.Auth;
using BackEndAPI.Core.Dtos.General;
using System.Security.Claims;

namespace BackEndAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDto> SeedRoleAsync();
        Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto);
        Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal user, UpdateRoleDto updateRoleDto);
        Task<LoginServiceResponseDto> MeAsync(MeDto meDto);
        Task<IEnumerable<UserInfoResult>> GetUserListAsync();
        Task<UserInfoResult> GetUserDetailsByUserName(string userName);
        Task<IEnumerable<string>> GetUserNamesListAsync();
    }
}
