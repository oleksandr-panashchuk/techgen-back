using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public JWTService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(IEnumerable<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                authClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
                
            return principal;
        }

        public IBaseResponse<AuthenticatedResponse> RefreshToken(TokenRequestModel tokenModel)
        {
            string accessToken = tokenModel.AccessToken!;
            string refreshToken = tokenModel.RefreshToken!;

            var principal = GetPrincipalFromExpiredToken(accessToken);

            string email = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email).Value;

            var user = _unitOfWork.Repository<User>().FindOne(x => x.Email == email);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new BaseResponse<AuthenticatedResponse>
                {
                    Description = "Invalid client request",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                };
            }

            var newAccessToken = GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            _unitOfWork.Repository<User>().ReplaceOne(user);

            return new BaseResponse<AuthenticatedResponse>
            {
               Data = new AuthenticatedResponse
               {
                   Token = newAccessToken,
                   RefreshToken = newRefreshToken
               },
               StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
    }
}
