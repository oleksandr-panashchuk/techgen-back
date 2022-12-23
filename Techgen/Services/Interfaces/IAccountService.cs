using System.Security.Claims;
using Techgen.Domain.Models.Account;
using Techgen.Domain.Response;

namespace Techgen.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IBaseResponse<ClaimsIdentity>> Register(RegisterModel model);
        public Task<IBaseResponse<ClaimsIdentity>> Login(LoginModel model);
    }
}
