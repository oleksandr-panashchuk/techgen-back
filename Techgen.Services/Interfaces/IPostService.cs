﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;

namespace Techgen.Services.Interfaces
{
    public interface IPostService
    {
        Task<IBaseResponse<PostResponseModel>> Create(PostRequestModel model);
        Task<IBaseResponse<PostResponseModel>> Update(int id,PostRequestModel model);
        Task<IBaseResponse<PostResponseModel>> Get(int id);
        Task<IBaseResponse<IEnumerable<SmallPostResponseModel>>> GetAll();
        Task<IBaseResponse<MessageResponseModel>> Delete (int id);
    }
}
