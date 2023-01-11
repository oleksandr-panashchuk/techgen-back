using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Session;

namespace Techgen.Services
{
    public class AutoMapperProfileConfiguration : AutoMapper.Profile
    {
        public AutoMapperProfileConfiguration()
        : this("MyProfile")
        {
        }

        protected AutoMapperProfileConfiguration(string profileName)
        : base(profileName)
        {
            #region User model

            CreateMap<Profile, UserResponseModel>()
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User != null ? x.User.Email : ""))
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.User != null ? x.User.PhoneNumber : ""))
                .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => x.User != null ? !x.User.IsActive : false));

            CreateMap<ApplicationUser, UserBaseResponseModel>()
               .IncludeAllDerived();

            CreateMap<ApplicationUser, UserResponseModel>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(x => x.Profile.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(x => x.Profile.LastName))
                .ForMember(x => x.IsBlocked, opt => opt.MapFrom(x => !x.IsActive))
                .IncludeAllDerived();



            CreateMap<ApplicationUser, UserRoleResponseModel>();

            #endregion

        }
    }
}
