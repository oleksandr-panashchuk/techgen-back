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

namespace Techgen.Services.Services
{
    public class LikeService 
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

        public IBaseResponse<PostResponseModel> Create(LikeRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().FindById(model.PostId);         
            if (post == null)
            {
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };
            }

            var like = post.Likes.FirstOrDefault(x => x.PostId == model.PostId && x.UserId == _userId);
            if (like != null)
            {
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "post already liked" };
            }
           
            like = new Like { PostId = model.PostId, UserId = _userId };
            post.Likes.Add(like);
            post.LikesCount += 1;

            _unitOfWork.Repository<Post>().ReplaceOne(post);

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }

        public IBaseResponse<PostResponseModel> Delete(LikeRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().FindById(model.PostId);          
            if (post == null)
            {
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };
            }

            var like = post.Likes.FirstOrDefault(x => x.PostId == model.PostId && x.UserId == _userId);
            if (like == null)
            {
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find like to this post" };
            }                    
            
            post.Likes.Remove(like);              
            post.LikesCount -= 1;

            _unitOfWork.Repository<Post>().ReplaceOne(post);

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    }
}
