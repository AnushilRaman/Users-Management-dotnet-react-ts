using BackEndAPI.Core.Dtos.Auth;
using BackEndAPI.Core.Dtos.General;
using BackEndAPI.Core.Interfaces;
using System.Security.Claims;

namespace BackEndAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        public Task<UserInfoResult> GetUserDetailsByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserInfoResult>> GetUserListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserNamesListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public Task<LoginServiceResponseDto> MeAsync(MeDto meDto)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralServiceResponseDto> SeedRoleAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal user, UpdateRoleDto updateRoleDto)
        {
            throw new NotImplementedException();
        }
    }
}
