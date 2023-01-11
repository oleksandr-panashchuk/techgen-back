using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Session;
using Techgen.Domain.Entities.Identity;

namespace Techgen.Services.Interfaces
{
    public interface IJWTService
    {
        /// <summary>
        /// Get identity
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isRefreshToken"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> GetIdentity(ApplicationUser user, bool isRefreshToken);

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="now"></param>
        /// <param name="identity"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        JwtSecurityToken CreateToken(DateTime now, ClaimsIdentity identity, DateTime lifetime);

        /// <summary>
        /// Create user token
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="lifetime">Token lifetime in seconds</param>
        /// <param name="isRefresh"></param>
        /// <returns></returns>
        Task<TokenResponseModel> CreateUserTokenAsync(ApplicationUser user, int? lifetime = null, bool isRefresh = false);

        /// <summary>
        /// Build login response 
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="accessTokenLifetime">Access token lifetime in seconds</param>
        /// <returns></returns>
        Task<LoginResponseModel> BuildLoginResponse(ApplicationUser user, int? accessTokenLifetime = null);

        /// <summary>
        /// Clear user's tokens
        /// </summary>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task ClearUserTokens(ApplicationUser user);
    }
}
