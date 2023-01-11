using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.ResponseModels.Post
{
    public class CommentResponseModel
    {
        public int Id { get; set; }
        public int ParentCommentId { get; set; }
        public int AuthorId { get; set; }
        public string Text { get; set; }
        public List<CommentResponseModel> AnswersToComment { get; set; }
    }
}
