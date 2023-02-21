using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Extensions;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.Identity;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        private int? _userId = null;

        public CommentService(IUnitOfWork unitOfWork,
                              IHttpContextAccessor httpContextAccessor,
                              IMapper mapper)
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

        public async Task<IBaseResponse<CommentResponseModel>> Create(CommentRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().GetById(model.PostId);

            if (post == null)
            {
                return new BaseResponse<CommentResponseModel>() {StatusCode = System.Net.HttpStatusCode.NotFound, Description="That post, you tried send commment, does not exist"};
            }
            var comment = new Comment() { PostId = post.Id, Text = model.Text, UserId = _userId.Value };

            post.Comments.Add(comment);

            _unitOfWork.Repository<Post>().Update(post);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<CommentResponseModel>(comment);
            return new BaseResponse<CommentResponseModel>() { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }

        public async Task<IBaseResponse<MessageResponseModel>> AdminDelete(AdminDeleteCommentRequestModel model)
        {
            var post = _unitOfWork.Repository<Post>().GetById(model.PostId);

            if (post == null) 
            { 
            }
            var comment = post.Comments.FirstOrDefault(x => x.Id.ToString() == model.CommentId);
            if (comment == null)
            { 
            }
            post.Comments.Remove(comment);

            _unitOfWork.Repository<Post>().Update(post);
            _unitOfWork.SaveChanges();

            return new BaseResponse<MessageResponseModel>(new MessageResponseModel($"deleted {comment.Id} from {post.Id}"));
        }

        public async Task<IBaseResponse<MessageResponseModel>> UserDelete(int commentId)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Id == _userId.Value).FirstOrDefault();

            if (user == null)
            { 
            }

            var comment = user.Comments.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            { 
            }
            _unitOfWork.Repository<Comment>().DeleteById(commentId);
            return new BaseResponse<MessageResponseModel>(new MessageResponseModel($"deleted {comment.Id} from {user.Id} user"));
        }

        public async Task<IBaseResponse<IEnumerable<CommentResponseModel>>> GetAll(int postId)
        {
            var post = _unitOfWork.Repository<Post>().GetById(postId);

            if (post == null)
            {
                return new BaseResponse<IEnumerable<CommentResponseModel>>() {StatusCode = System.Net.HttpStatusCode.NotFound, Description = "That post does not exist"};
            }
            var response = _mapper.Map<IEnumerable<CommentResponseModel>>(post.Comments);
            return new BaseResponse<IEnumerable<CommentResponseModel>>() {Data=response, StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<CommentResponseModel>> CreateAnswer(CommentRequestModel model, int parentCommentId)
        {
            var comment = _unitOfWork.Repository<Comment>().GetById(parentCommentId);
            var authorId = comment.UserId;

            if (comment == null)
            {
                return new BaseResponse<CommentResponseModel>() { StatusCode = System.Net.HttpStatusCode.NotFound, Description = "that comment does not exist" };
            }

            var answer = new Comment() { PostId = comment.PostId, UserId = _userId.Value, Text = model.Text, ParentCommentId = parentCommentId};

            while (comment.ParentCommentId != null)
            {
                comment = _unitOfWork.Repository<Comment>().Get(x => x.Id == comment.ParentCommentId).Include(w => w.Answers).FirstOrDefault();
            }

            answer.Text = $"Пользователю {authorId}, " + model.Text;
            comment.Answers.Add(answer);

            _unitOfWork.Repository<Comment>().Update(comment);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<CommentResponseModel>(answer);
            return new BaseResponse<CommentResponseModel>() {Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    
    }
}
