using System;
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
    public interface ICommentService
    {
        Task<IBaseResponse<CommentResponseModel>> Create(CommentRequestModel model);
        Task<IBaseResponse<IEnumerable<CommentResponseModel>>> GetAll(int postId);
        Task<IBaseResponse<CommentResponseModel>> CreateAnswer(CommentRequestModel model, int postId);
        Task<IBaseResponse<MessageResponseModel>> UserDelete(int id);
        Task<IBaseResponse<MessageResponseModel>> AdminDelete(AdminDeleteCommentRequestModel model);

    }
}
