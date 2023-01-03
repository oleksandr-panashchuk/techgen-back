using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Services.Interfaces
{
    public interface IJWTService
    {
        public JwtSecurityToken GetToken(List<Claim> authClaims);
    }
}
