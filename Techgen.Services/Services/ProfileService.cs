using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities;
using Techgen.Domain.Entity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBaseResponse<Profile>> Edit(ProfileRequestModel model)
        {
            var profile = _unitOfWork.Repository<Profile>().FindOne(x => x.Email == model.Email);

            if (profile == null)
            {
                return new BaseResponse<Profile>
                {
                    Description = "Profile not found"
                };
            }

            profile.Age = model.Age;
            profile.Country = model.Country;
            //profile.Name = model.Name;

            _unitOfWork.Repository<Profile>().ReplaceOne(profile);

            return new BaseResponse<Profile>
            {
                Data = profile
            };
        }

        public async Task<IBaseResponse<Profile>> Create(User user)
        {
            var profile = _unitOfWork.Repository<Profile>().FindById(user.Id.ToString());
                        
            if (profile != null)
            {
                return new BaseResponse<Profile>
                {
                    Description = "Profile already exists",
                    StatusCode = HttpStatusCode.OK
                };
            }

            profile = new Profile
            {   
                //F = $"user{user.DigitId}",
                Email = user.Email,
                UserId = user.Id.ToString(),
            };

            _unitOfWork.Repository<Profile>().InsertOne(profile);

            return new BaseResponse<Profile>
            {
                Data = profile,
                StatusCode = HttpStatusCode.OK
            };
        }
        public async Task<IBaseResponse<ProfileResponseModel>> Get(string id)
        {
            try
            {
                var profile = _unitOfWork.Repository<Profile>()
                    .AsQueryable().Select(x => new ProfileResponseModel()
                    {
                        Id = x.Id.ToString(),
                        //Name = x.Name,
                        Age = x.Age,
                        Country = x.Country,
                    }).Single();

                return new BaseResponse<ProfileResponseModel>()
                {
                    Data = profile,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProfileResponseModel>()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Description = $"InternalServerError: {ex.Message}"
                };
            }
        }
    }
}
