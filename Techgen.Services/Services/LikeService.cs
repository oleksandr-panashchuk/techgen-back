using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Extensions;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        private string? _userId = null;

        public LikeService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
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

        public async Task<IBaseResponse<PostResponseModel>> Create(string postId)
        {
            var post = _unitOfWork.Repository<Post>().FindById(postId);      
            
            if (post == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };

            var like = post.Likes.FirstOrDefault(x => x.UserId == _userId);
            if (like != null)
                await Delete(postId);
           
            like = new Like { PostId = postId, UserId = _userId };
            post.Likes.Add(like);

            _unitOfWork.Repository<Post>().ReplaceOne(post);

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }

        public async Task<IBaseResponse<PostResponseModel>> Delete(string postId)
        {
            var post = _unitOfWork.Repository<Post>().FindById(postId);          
            if (post == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };

            var like = post.Likes.FirstOrDefault(x => x.UserId == _userId);
            if (like == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find like to this post" };
            
            post.Likes.Remove(like);              

            _unitOfWork.Repository<Post>().ReplaceOne(post);

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    }
}
