using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Techgen.Domain.Entity;
using Techgen.Domain.Models.Account;
using Techgen.Domain.Response;

namespace Techgen.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IBaseResponse<IdentityUser>> Register(RegisterModel model);
        public Task<IBaseResponse<JwtSecurityToken>> Login(LoginModel model);
        public Task<IBaseResponse<ApplicationUser>> CheckRecoveryCode(string email, string recoveryCode);
        public Task<IBaseResponse<ApplicationUser>> ChangePassword(string email, string newRecoveryCode);
    }
}
