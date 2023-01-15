using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;

namespace Techgen.Services.Interfaces
{
    public interface ILikeService
    {
        Task<IBaseResponse<PostResponseModel>> Create(string postId);
        Task<IBaseResponse<PostResponseModel>> Delete(string postId);
    }
}
