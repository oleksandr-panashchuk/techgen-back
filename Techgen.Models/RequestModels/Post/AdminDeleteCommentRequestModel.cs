using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.RequestModels.Post
{
    public class AdminDeleteCommentRequestModel
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
    }
}
