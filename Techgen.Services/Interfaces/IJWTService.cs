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

namespace Techgen.Services.Interfaces
{
    public interface IJWTService
    {
        public string GenerateAccessToken(IEnumerable<Claim> authClaims);
        public string GenerateRefreshToken();
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        public IBaseResponse<AuthenticatedResponse> RefreshToken(TokenRequestModel tokenModel);
    }
}
