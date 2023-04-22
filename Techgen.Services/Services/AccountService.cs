using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Extensions;
using Techgen.Common.Utilities;
using Techgen.Common.Utilities.Interfaces;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Session;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJWTService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHashUtility _hashUtility;
        
        private bool _isUserModerator = false;
        private bool _isUserAdmin = false;
        private int? _userId = null;

        public AccountService(UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IJWTService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IHashUtility hash)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _hashUtility = hash;

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
                _isUserModerator = context.User.IsInRole(Role.SuperAdmin);
                _isUserAdmin = context.User.IsInRole(Role.Admin);

                try
                {
                    _userId = context.User.GetUserId();
                }
                catch
                {
                    _userId = null;
                }
            }
        }

        public async Task<IBaseResponse<RegisterResponseModel>> Register(RegisterRequestModel model)
        {
            model.Email = model.Email.Trim().ToLower();

            ApplicationUser user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Email.ToLower() == model.Email);

            if (user != null && user.EmailConfirmed)
            {
                return new BaseResponse<RegisterResponseModel>
                {
                    Description = "User already exists"
                };
            }

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    IsActive = true,
                    RegistratedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                user.Profile = new Profile() {FirstName = "", LastName = ""};

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return new BaseResponse<RegisterResponseModel>
                    {
                        Description = result.Errors.FirstOrDefault().Description
                    };
                }
                result = await _userManager.AddToRoleAsync(user, Role.User);

                if (!result.Succeeded)
                {
                    return new BaseResponse<RegisterResponseModel>
                    {
                        Description = result.Errors.FirstOrDefault().Description
                    };
                }
            }

            return new BaseResponse<RegisterResponseModel> { Data = new RegisterResponseModel() {Email = user.Email } , StatusCode = HttpStatusCode.Created};
        }

        public async Task<IBaseResponse<RegisterResponseModel>> RegisterAdmin(RegisterRequestModel model)
        {
            model.Email = model.Email.Trim().ToLower();

            ApplicationUser user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Email.ToLower() == model.Email);

            if (user != null && user.EmailConfirmed)
            {
                return new BaseResponse<RegisterResponseModel>
                {
                    Description = "Admin already exists"
                };
            }

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    IsActive = true,
                    RegistratedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                user.Profile = new Profile() {FirstName ="", LastName=""};

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return new BaseResponse<RegisterResponseModel>
                    {
                        Description = result.Errors.FirstOrDefault().Description
                    };
                }
                result = await _userManager.AddToRoleAsync(user, Role.Admin.ToString());

                if (!result.Succeeded)
                {
                    return new BaseResponse<RegisterResponseModel>
                    {
                        Description = result.Errors.FirstOrDefault().Description
                    };
                }

            }

            return new BaseResponse<RegisterResponseModel> { Data = new RegisterResponseModel() { Email = user.Email }, StatusCode = HttpStatusCode.Created };
        }

        public async Task<IBaseResponse<LoginResponseModel>> Login(LoginRequestModel model)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Email == model.Email)
                                                                .Include(w => w.UserRoles)
                                                                    .ThenInclude(w => w.Role)
                                                                .FirstOrDefault();

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) || !user.UserRoles.Any(x => x.Role.Name == Role.User.ToString()))
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Description = "Invalid creaditals"
                };
            }
            if (!string.IsNullOrEmpty(model.Email) && !user.EmailConfirmed)
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Description = "Email is not comfirmed"
                };
            }
            if (user.IsDeleted)
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Description = "Your account was deleted by admin"
                };
            }
            if (!user.IsActive)
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Description = "Your account was blocked"
                };
            }

            var response = await _jwtService.BuildLoginResponse(user, model.AccessTokenLifetime);

            return new BaseResponse<LoginResponseModel> { Data = response, StatusCode = HttpStatusCode.Accepted };
            
        }

        public async Task<IBaseResponse<LoginResponseModel>> AdminLogin(AdminLoginRequestModel model)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Email == model.Email)
                                                                .Include(w => w.UserRoles)
                                                                    .ThenInclude(w => w.Role)
                                                                .FirstOrDefault();

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) || !user.UserRoles.Any(x => x.Role.Name == Role.Admin.ToString() || x.Role.Name == Role.SuperAdmin.ToString()))
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Description = "Invalid creaditals"
                };
            }

            var response = await _jwtService.BuildLoginResponse(user, model.AccessTokenLifetime);

            return new BaseResponse<LoginResponseModel> { Data = response, StatusCode = HttpStatusCode.Accepted };
        }

        public async Task<IBaseResponse<TokenResponseModel>> RefreshTokenAsync(string refreshToken, List<string> roles)
        {
            var token = _unitOfWork.Repository<UserToken>().Find(w => w.RefreshTokenHash == _hashUtility.GetHash(refreshToken) && w.IsActive && w.RefreshExpiresDate > DateTime.UtcNow);
            if (token == null)
            {
                return new BaseResponse<TokenResponseModel>
                {
                    Description = "Invalid creaditals"
                };
            }
            if (!token.User.UserRoles.Any(x => roles.Contains(x.Role.Name)))
            {
                return new BaseResponse<TokenResponseModel>
                {
                    Description = "Access denied"
                };
            }
            var result = await _jwtService.CreateUserTokenAsync(token.User, isRefresh: true);

            return new BaseResponse<TokenResponseModel> { Data = result, StatusCode = HttpStatusCode.Accepted };
        }

        public async Task Logout()
        {
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Id == _userId.Value)
                                                                .Include(w => w.Tokens)
                                                                .FirstOrDefault();

            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _jwtService.ClearUserTokens(user);
        }

    }
}
