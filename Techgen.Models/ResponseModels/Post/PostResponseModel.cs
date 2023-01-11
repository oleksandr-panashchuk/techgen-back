using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.ResponseModels.Post
{
    public class PostResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int LikesCount { get; set; }
        public List<CommentResponseModel> Comments { get; set; }
    }
}
