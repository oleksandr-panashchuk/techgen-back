using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        private int? _userId = null;

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

        public async Task<IBaseResponse<PostResponseModel>> Create(int postId)
        {
            var post = _unitOfWork.Repository<Post>().Get(x=>x.Id == postId).Include(w => w.Likes).FirstOrDefault();      
            
            if (post == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };

            var like = post.Likes.FirstOrDefault(x => x.UserId == _userId);
            if (like != null)
                await Delete(postId);
           
            like = new Like { PostId = postId, UserId = _userId.Value};
            post.Likes.Add(like);

            _unitOfWork.Repository<Post>().Update(post);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }

        public async Task<IBaseResponse<PostResponseModel>> Delete(int postId)
        {
            var post = _unitOfWork.Repository<Post>().GetById(postId);          
            if (post == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find post" };

            var like = post.Likes.FirstOrDefault(x => x.UserId == _userId);
            if (like == null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "cannot find like to this post" };
            
            post.Likes.Remove(like);              

            _unitOfWork.Repository<Post>().Update(post);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    }
}
