using AutoMapper;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Extensions;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        private bool _isUserModerator = false;
        private bool _isUserAdmin = false;
        private string? _userId = null;

        public ProfileService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
                _isUserModerator = context.User.IsInRole(Role.Moderator.ToString());
                _isUserAdmin = context.User.IsInRole(Role.Admin.ToString());

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

        public async Task<IBaseResponse<UserResponseModel>> Edit(ProfileRequestModel model)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().FindOne(x => x.Id.ToString() == _userId);

            if (user == null)
            {
                return new BaseResponse<UserResponseModel>
                {
                    Description = "User not found"
                };
            }

            user.Profile.Age = model.Age;
            user.Profile.Country = model.Country;
            user.UserName = model.Name;

            _unitOfWork.Repository<ApplicationUser>().ReplaceOne(user);

            return new BaseResponse<UserResponseModel>
            {
                Data = _mapper.Map<UserResponseModel>(user),
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IBaseResponse<UserResponseModel>> Create(ApplicationUser usermodel)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().FindById(usermodel.Id.ToString());
                        
            if (user != null)
            {
                return new BaseResponse<UserResponseModel>
                {
                    Description = "Profile already exists",
                    StatusCode = HttpStatusCode.OK
                };
            }

            user.Profile = new Domain.Entities.Identity.Profile();

            _unitOfWork.Repository<ApplicationUser>().InsertOne(user);

            return new BaseResponse<UserResponseModel>
            {
                Data = _mapper.Map<UserResponseModel>(user.Profile),
                StatusCode = HttpStatusCode.OK
            };
        }
        public async Task<IBaseResponse<UserResponseModel>> Get(string id)
        {
            try
            {
                var user = _unitOfWork.Repository<ApplicationUser>().FindOne(x => x.Id.ToString() == id);
                var response = _mapper.Map<UserResponseModel>(user.Profile);

                return new BaseResponse<UserResponseModel>()
                {
                    Data = response,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserResponseModel>()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Description = $"InternalServerError: {ex.Message}"
                };
            }
        }
    }
}
