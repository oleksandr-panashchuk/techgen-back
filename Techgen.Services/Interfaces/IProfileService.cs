using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities;
using Techgen.Domain.Entity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;

namespace Techgen.Services.Interfaces
{
    public interface IProfileService
    {
        Task<IBaseResponse<Profile>> Edit(ProfileRequestModel model);
        Task<IBaseResponse<Profile>> Create(User user);
        Task<IBaseResponse<ProfileResponseModel>> Get(string id);
    }
}
