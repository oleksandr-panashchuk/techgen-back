using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Exceptions;
using Techgen.Common.Extensions;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.Identity;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Services.Interfaces;
using static MongoDB.Libmongocrypt.CryptContext;

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
            var post = _unitOfWork.Repository<Post>().Get(x => x.Id == model.PostId)
                                                    .Include(w => w.Comments)
                                                    .FirstOrDefault();

            if (post == null)
            {
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Post not found" ,"That post, you tried send commment, does not exist");
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
            var post = _unitOfWork.Repository<Post>().Get(x => x.Id == model.PostId)
                                                    .Include(w => w.Comments)
                                                    .FirstOrDefault();
            if (post == null)
                throw new CustomException(System.Net.HttpStatusCode.Forbidden, "Post not found", "Such post does not exist");

            var comment = post.Comments.FirstOrDefault(x => x.Id == model.CommentId);

            if (comment == null)
                throw new CustomException(System.Net.HttpStatusCode.Forbidden, "Comment not found" ,"Such comment does not exist" );

            post.Comments.Remove(comment);

            _unitOfWork.Repository<Post>().Update(post);
            _unitOfWork.SaveChanges();

            return new BaseResponse<MessageResponseModel>(new MessageResponseModel($"deleted {comment.Id} from {post.Id}"));
        }

        public async Task<IBaseResponse<MessageResponseModel>> UserDelete(int commentId)
        {
            var user = _unitOfWork.Repository<ApplicationUser>().Get(x => x.Id == _userId.Value)
                                                                .Include(w => w.Comments)
                                                                .FirstOrDefault();

            if (user == null)
                throw new CustomException(System.Net.HttpStatusCode.Forbidden, "User not found" ,"Such user-session does not exist" );

            var comment = user.Comments.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Comment not found", "Such comment does not exist");

            _unitOfWork.Repository<Comment>().DeleteById(commentId);
            _unitOfWork.SaveChanges();
            return new BaseResponse<MessageResponseModel>(new MessageResponseModel($"deleted {comment.Id} from {user.Id} user"));
        }

        public async Task<IBaseResponse<IEnumerable<CommentResponseModel>>> GetAll(int postId)
        {
            var post = _unitOfWork.Repository<Post>().GetById(postId);

            if (post == null)
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Post not found","That post does not exist");

            var response = _mapper.Map<IEnumerable<CommentResponseModel>>(post.Comments);
            return new BaseResponse<IEnumerable<CommentResponseModel>>() {Data=response, StatusCode = System.Net.HttpStatusCode.OK};
        }

        public async Task<IBaseResponse<CommentResponseModel>> CreateAnswer(CommentRequestModel model, int parentCommentId)
        {
            var comment = _unitOfWork.Repository<Comment>().Get(x=> x.Id == parentCommentId)
                                                           .Include(w => w.User)
                                                           .Include(w => w.Answers)
                                                           .FirstOrDefault();

            if (comment == null)
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Comment not found" ,"that comment does not exist" );

            var authorUserName = comment.User.UserName;

            var answer = new Comment() { PostId = comment.PostId, UserId = _userId.Value, Text = model.Text, ParentCommentId = parentCommentId};

            answer.Text = $"Пользователю {authorUserName}, " + model.Text;
            comment.Answers.Add(answer);

            _unitOfWork.Repository<Comment>().Update(comment);
            _unitOfWork.SaveChanges();

            var response = _mapper.Map<CommentResponseModel>(answer);
            return new BaseResponse<CommentResponseModel>() {Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    
    }
}
