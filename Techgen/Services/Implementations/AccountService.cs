using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Techgen.DAL.Interfaces;
using Techgen.Domain.Entity;
using Techgen.Domain.Enum;
using Techgen.Domain.Helpers;
using Techgen.Domain.Models.Account;
using Techgen.Domain.Response;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IBaseResponse<ClaimsIdentity>> Register(RegisterModel model)
        {
            try
            {
                var user = await _userRepository.FindUser(model.Email);
                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "User already exists"
                    };
                }

                user = new User()
                {
                    Email = model.Email,
                    Password = HashPasswordHelper.HashPassword(model.Password),
                    Role = Role.User.ToString()
                };

                await _userRepository.Create(user);

                var result = Authenticate(user);
                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK,
                    Description = "User added"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Register]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }        
        }

        public async Task<IBaseResponse<ClaimsIdentity>> Login(LoginModel model)
        {
            try
            {
                var user = await _userRepository.FindUser(model.Email);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "User not found",
                    };
                }

                if (user.Password != HashPasswordHelper.HashPassword(model.Password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Incorrect password"
                    };
                }

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };

            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
