using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Session;

namespace Techgen.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Refresh tokens
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="roles">Roles</param>
        /// <returns></returns>
        Task<IBaseResponse<TokenResponseModel>> RefreshTokenAsync(string refreshToken, List<string> roles);

        /// <summary>
        /// Register a new user using email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IBaseResponse<RegisterResponseModel>> Register(RegisterRequestModel model);

        /// <summary>
        /// Register a new admin using email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IBaseResponse<RegisterResponseModel>> RegisterAdmin(RegisterRequestModel model);
        /// <summary>
        /// Login using email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IBaseResponse<LoginResponseModel>> Login(LoginRequestModel model);

        /// <summary>
        /// Admin login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<IBaseResponse<LoginResponseModel>> AdminLogin(AdminLoginRequestModel model);

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        Task Logout();
    }
}
