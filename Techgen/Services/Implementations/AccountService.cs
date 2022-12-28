using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Techgen.DAL.Interfaces;
using Techgen.Domain.Entity;
using Techgen.Domain.Enum;
using Techgen.Domain.Helpers;
using Techgen.Domain.Models.Account;
using Techgen.Domain.Response;
using Techgen.Services.EmailService;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(IUserRepository userRepository, ILogger<AccountService> logger, IEmailSender emailSender, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<IBaseResponse<IdentityUser>> Register(RegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    return new BaseResponse<IdentityUser>()
                    {
                        Description = "User with this username already exists!",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                ApplicationUser user = new()
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    DigitId = $"{DigitIDHelper.GetDigitID:00000000}",
                    RecoveryCode = DigitRecoveryHelper.GenereteCodeRecovery(_userRepository.GetAllRecovery())
                };

                await _userManager.AddToRoleAsync(user, "User");
                var result = await _userManager.CreateAsync(user, model.Password);

                //Send email
                var message = new Message(new string[] { $"{model.Email}" }, "TECHGEN account registration", $"Welcome to TECHGEN, your recovery code {user.RecoveryCode}", null);
                await _emailSender.SendEmailAsync(message);

                if (!result.Succeeded)
                {
                    return new BaseResponse<IdentityUser>()
                    {
                        Description = "Failed to create user, please try again.",
                        StatusCode = StatusCode.InternalServerError
                    };
                }
                return new BaseResponse<IdentityUser>()
                {   
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Register]: {ex.Message}");
                return new BaseResponse<IdentityUser>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }        
        }

        public async Task<IBaseResponse<JwtSecurityToken>> Login(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    //Send email
                    var message = new Message(new string[] { $"{model.Email}" }, "TECHGEN login in your account ", "Successful login", null);
                    await _emailSender.SendEmailAsync(message);

                    return new BaseResponse<JwtSecurityToken>()
                    {
                        Data = GetToken(authClaims),
                        StatusCode = StatusCode.OK
                    };
                }
                return new BaseResponse<JwtSecurityToken>()
                {   
                    Description = "Failed to login user, please try again.",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<JwtSecurityToken>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                authClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return token;
        }

        public async Task<IBaseResponse<ApplicationUser>> SendEmailNewRecoveryCode(string email, string recoveryCode)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.RecoveryCode == recoveryCode)
                {
                    string newRecoveryCode = DigitRecoveryHelper.GenereteCodeRecovery(_userRepository.GetAllRecovery());                
                    user.RecoveryCode = newRecoveryCode;
                    await _userManager.UpdateAsync(user);

                    var message = new Message(new string[] { email }, "TECHGEN your new recovery code", $"Your new recovery code {newRecoveryCode}", null);
                    await _emailSender.SendEmailAsync(message);

                    return new BaseResponse<ApplicationUser>()
                    {   
                        Data = user,
                        StatusCode = StatusCode.OK
                    };
                }
                else
                {
                    throw new Exception("Could not send email with new recovery code ");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"[SendMessageWithRecoveryCode]: {ex.Message}");
                return new BaseResponse<ApplicationUser>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }        
        }

        //Check if the new revocery code which was sending to email is equal and generate new password
        public async Task<IBaseResponse<ApplicationUser>> SendEmailNewPassword(string email, string newRecoveryCode)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.RecoveryCode == newRecoveryCode)
                {
                    string newPassword = CreatePasswordHelper.GeneratePassword();
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to change password.");
                    }

                    var message = new Message(new string[] { email }, "TECHGEN your new password", $"Your your new password {newPassword}", null);
                    await _emailSender.SendEmailAsync(message);
                    
                    return new BaseResponse<ApplicationUser>()
                    {
                        Data = user,
                        StatusCode = StatusCode.OK
                    };
                }                         
                return new BaseResponse<ApplicationUser>()
                {
                    StatusCode = StatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SendEmailNewPassword]: {ex.Message}");
                return new BaseResponse<ApplicationUser>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
