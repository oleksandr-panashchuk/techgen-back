using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Utilities;
using Techgen.Common.Utilities.Interfaces;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entity;
using Techgen.EmailService;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;
using static MongoDB.Libmongocrypt.CryptContext;

namespace Techgen.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IProfileService _profileService;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger,
            IEmailSender emailSender, IJWTService jwtService, IConfiguration configuration, 
            IProfileService profileService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            _jwtService = jwtService;
            _configuration = configuration;
            _profileService = profileService;
        }

        public async Task<IBaseResponse<User>> Register(RegisterRequestModel model)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().FindOneAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    return new BaseResponse<User>()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Description = "User with this username already exists!",
                    };
                }

                user = new User()
                {
                    Email = model.Email,
                    Role = Role.User.ToString(),
                    Password = HashUtility.GetHash(model.Password),
                    DigitId = $"{DigitIdUtility.GetDigitID:00000000}",
                    RecoveryCode = RecoveryCodeUtility.GenereteRecoveryCode()
                };

                await _unitOfWork.Repository<User>().InsertOneAsync(user);
                _profileService.Create(user);

                //Send email
                var message = new Message(new string[] { $"{model.Email}" }, "TECHGEN account registration", $"Welcome to TECHGEN, your recovery code {user.RecoveryCode}", null);
                await _emailSender.SendEmailAsync(message);

                return new BaseResponse<User>()
                {   
                    Data = user,
                    Description = "User added",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Register]: {ex.Message}");
                return new BaseResponse<User>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AuthenticatedResponse>> Login(LoginRequestModel model)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().FindOneAsync(x => x.Email == model.Email);
                if (user == null || user.Password != HashUtility.GetHash(model.Password))
                {
                    return new BaseResponse<AuthenticatedResponse>()
                    {
                        Description = "Invalid password"
                    };
                }

                var authClaims = new List<Claim>
                {   
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var accessToken = _jwtService.GenerateAccessToken(authClaims);
                var refreshToken = _jwtService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                _unitOfWork.Repository<User>().ReplaceOne(user);

                //Send email
                var message = new Message(new string[] { $"{model.Email}" }, "TECHGEN login in your account ", "Successful login", null);
                await _emailSender.SendEmailAsync(message);

                return new BaseResponse<AuthenticatedResponse>()
                {
                    Data = new AuthenticatedResponse
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<AuthenticatedResponse>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<User>> CheckRecoveryCode(string email, string recoveryCode)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().FindOneAsync(x => x.Email == email);
                if (user != null && user.RecoveryCode == recoveryCode)
                {
                    return new BaseResponse<User>()
                    {
                        Data = user,
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    throw new Exception("Recovery code is incorrect");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CheckRecoveryCode]: {ex.Message}");
                return new BaseResponse<User>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<User>> ChangePassword(string email, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().FindOneAsync(x => x.Email == email);
                if (user != null)
                {
                    user.Password = HashUtility.GetHash(newPassword);
                    _unitOfWork.Repository<User>().ReplaceOne(user);

                    var message = new Message(new string[] { email }, "TECHGEN change password", $"Your password was changed successful", null);
                    await _emailSender.SendEmailAsync(message);

                    return new BaseResponse<User>()
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }
                throw new Exception("Failed to change password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ChangePassword]: {ex.Message}");
                return new BaseResponse<User>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };

            }
        }

        public IBaseResponse<User> Logout()
        {
            string email = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email).Value;
            var user = _unitOfWork.Repository<User>().FindOne(x => x.Email == email);
            if (user == null)
            {
                return new BaseResponse<User>()
                {
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            user.RefreshToken = null;
            _unitOfWork.Repository<User>().ReplaceOne(user);

            return new BaseResponse<User>()
            {
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
