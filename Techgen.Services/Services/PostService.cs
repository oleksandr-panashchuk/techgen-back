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
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        private int? _userId = null;

        public PostService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
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

        public async Task<IBaseResponse<MessageResponseModel>> Delete(int id)
        {
            var post = _unitOfWork.Repository<Post>().GetById(id);
            if (post == null)
                return new BaseResponse<MessageResponseModel>() { Data = new MessageResponseModel($"post does not exist"), StatusCode = System.Net.HttpStatusCode.NotFound };

            _unitOfWork.Repository<Post>().DeleteById(id);
            return new BaseResponse<MessageResponseModel>() { Data = new MessageResponseModel($"{id} was deleted"), StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<PostResponseModel>> Update(int id, PostRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().GetById(id);

            if (post == null)
                return new BaseResponse<PostResponseModel>() {StatusCode = System.Net.HttpStatusCode.NotFound, Description="that post does not exist" };

            post.Title = model.Name;
            post.Text = model.Text;

            _unitOfWork.Repository<Post>().Update(post);
            var response = _mapper.Map<PostResponseModel>(post);

            return new BaseResponse<PostResponseModel>() {Data = response, StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<IEnumerable<SmallPostResponseModel>>> GetAll()
        {
            var posts = _unitOfWork.Repository<Post>().GetAll();
            var response = _mapper.Map<IEnumerable<SmallPostResponseModel>>(posts);
            return new BaseResponse<IEnumerable<SmallPostResponseModel>>() {Data = response, StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<PostResponseModel>> Get(int id)
        {
            var post = _unitOfWork.Repository<Post>().Get(x => x.Id == id)
                                                     .Include(w => w.Likes)
                                                     .Include(w => w.Comments)
                                                     .FirstOrDefault();

            if (post == null)
                return new BaseResponse<PostResponseModel>() {StatusCode = System.Net.HttpStatusCode.NotFound, Description="post does not exist"};

            var response = _mapper.Map<PostResponseModel>(post);
            response.LikesCount = post.Likes.Count();

            return new BaseResponse<PostResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<PostResponseModel>> Create(PostRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().Get(x => x.Title == model.Name).FirstOrDefault(); 

            if (post != null)
                return new BaseResponse<PostResponseModel>() { StatusCode = System.Net.HttpStatusCode.Forbidden, Description="post with such title already exist"};

            post = new Post()
            {
                Title = model.Name,
                Text = model.Text,
                UserId = _userId.Value
            };

            _unitOfWork.Repository<Post>().Insert(post);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<PostResponseModel>(post);
            return new BaseResponse<PostResponseModel>() {Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    }
}
