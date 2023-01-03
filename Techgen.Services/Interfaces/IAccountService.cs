using System.Security.Claims;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;

namespace Techgen.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IBaseResponse<ClaimsIdentity>> Register(RegisterRequestModel model);
        public Task<IBaseResponse<ClaimsIdentity>> Login(LoginRequestModel model);
    }
}
