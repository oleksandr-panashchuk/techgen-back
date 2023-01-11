using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;

namespace Techgen.Services.Interfaces
{
    public interface IProfileService
    {
        Task<IBaseResponse<UserResponseModel>> Edit(ProfileRequestModel model);
        Task<IBaseResponse<UserResponseModel>> Create(ApplicationUser user);
        Task<IBaseResponse<UserResponseModel>> Get(string id);
    }
}
