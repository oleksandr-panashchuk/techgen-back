using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Techgen.Domain.Entity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;

namespace Techgen.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IBaseResponse<User>> Register(RegisterRequestModel model);
        public Task<IBaseResponse<AuthenticatedResponse>> Login(LoginRequestModel model);
        public IBaseResponse<User> Logout();
        public Task<IBaseResponse<User>> CheckRecoveryCode(string email, string recoveryCode);
        public Task<IBaseResponse<User>> ChangePassword(string email, string newPassword);
    }
}
