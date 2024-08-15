using BackEndAPI.Core.Constants;
using BackEndAPI.Core.Dtos.Auth;
using BackEndAPI.Core.Dtos.General;
using BackEndAPI.Core.Entities;
using BackEndAPI.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEndAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUsers> userManager, ILogService logService, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logService = logService;
            _configuration = configuration;
        }


        public async Task<GeneralServiceResponseDto> SeedRoleAsync()
        {
            bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isManagerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.MANAGER);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
            if (isOwnerRoleExists && isAdminRoleExists && isManagerRoleExists && isUserRoleExists)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Role Seeding is already done."
                };
            }
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.MANAGER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            return new GeneralServiceResponseDto()
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Roles Seeding done successfully."
            };
        }

        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser is not null)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "User already exists."
                };
            }
            ApplicationUsers applicationUsers = new ApplicationUsers()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.Lastname,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Address = registerDto.Address,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var createdUserResult = await _userManager.CreateAsync(applicationUsers, registerDto.Password);
            if (!createdUserResult.Succeeded)
            {
                var errorString = "";
                foreach (var item in createdUserResult.Errors)
                {
                    errorString += item.Description;
                }
                return new GeneralServiceResponseDto() { IsSucceed = false, StatusCode = 400, Message = errorString };
            };
            await _userManager.AddToRoleAsync(applicationUsers, StaticUserRoles.USER);
            await _logService.SaveNewLog(applicationUsers.UserName, "Register to website");
            return new GeneralServiceResponseDto() { IsSucceed = true, StatusCode = 200, Message = "User created successfully" };
        }

        public async Task<LoginServiceResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null) { return null; }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect) { return null; }
            var newToken = await this.GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            await _logService.SaveNewLog(user.UserName, "New Login");

            return new LoginServiceResponseDto()
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }


        public async Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto)
        {
            var user = await _userManager.FindByNameAsync(updateRoleDto.UserName);
            if (user is null)
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Invalid UserName"
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            // Just The OWNER and ADMIN can update roles
            if (User.IsInRole(StaticUserRoles.ADMIN))
            {
                // User is admin
                if (updateRoleDto.NewRole == RoleType.USER || updateRoleDto.NewRole == RoleType.MANAGER)
                {
                    // admin can change the role of everyone except for owners and admins
                    if (userRoles.Any(q => q.Equals(StaticUserRoles.OWNER) || q.Equals(StaticUserRoles.ADMIN)))
                    {
                        return new GeneralServiceResponseDto()
                        {
                            IsSucceed = false,
                            StatusCode = 403,
                            Message = "You are not allowed to change role of this user"
                        };
                    }
                    else
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                        await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                        await _logService.SaveNewLog(user.UserName, "User Roles Updated");
                        return new GeneralServiceResponseDto()
                        {
                            IsSucceed = true,
                            StatusCode = 200,
                            Message = "Role updated successfully"
                        };
                    }
                }
                else return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 403,
                    Message = "You are not allowed to change role of this user"
                };
            }
            else
            {
                // user is owner
                if (userRoles.Any(q => q.Equals(StaticUserRoles.OWNER)))
                {
                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = false,
                        StatusCode = 403,
                        Message = "You are not allowed to change role of this user"
                    };
                }
                else
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                    await _logService.SaveNewLog(user.UserName, "User Roles Updated");

                    return new GeneralServiceResponseDto()
                    {
                        IsSucceed = true,
                        StatusCode = 200,
                        Message = "Role updated successfully"
                    };
                }
            }
        }

        public async Task<LoginServiceResponseDto> MeAsync(MeDto meDto)
        {
            ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meDto.Token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
            }, out SecurityToken securityToken);

            string decodedUserName = handler.Claims.First(q => q.Type == ClaimTypes.Name).Value;
            if (decodedUserName is null)
                return null;

            var user = await _userManager.FindByNameAsync(decodedUserName);
            if (user is null)
                return null;

            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            await _logService.SaveNewLog(user.UserName, "New Token Generated");

            return new LoginServiceResponseDto()
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }
        public async Task<IEnumerable<UserInfoResult>> GetUserListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserInfoResult> userInfoResults = new List<UserInfoResult>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfoObject(user, roles);
                userInfoResults.Add(userInfo);
            }

            return userInfoResults;
        }

        public async Task<UserInfoResult> GetUserDetailsByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            return userInfo;
        }

        public async Task<IEnumerable<string>> GetUserNamesListAsync()
        {
            var userNames = await _userManager.Users
             .Select(q => q.UserName)
             .ToListAsync();

            return userNames;
        }

        #region private methods

        private async Task<string> GenerateJWTTokenAsync(ApplicationUsers applicationUsers)
        {
            var userRoles = await _userManager.GetRolesAsync(applicationUsers);
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,applicationUsers.UserName),
                new Claim(ClaimTypes.NameIdentifier,applicationUsers.Id),
                new Claim("FirstName",applicationUsers.FirstName),
                new Claim("LastName",applicationUsers.LastName),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        private UserInfoResult GenerateUserInfoObject(ApplicationUsers user, IEnumerable<string> Roles)
        {
            // Instead of this, You can use Automapper packages. But i don't want it in this project
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Lastname = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreateAt = user.CreateAt,
                Roles = Roles
            };
        }
        #endregion

    }
}
