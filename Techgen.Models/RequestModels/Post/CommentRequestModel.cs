using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Attributes;

namespace Techgen.Models.RequestModels.Post
{
    public class CommentRequestModel
    {
        [Required(ErrorMessage = "text is required")]
        [StringLength(300, ErrorMessage = "Text should be from 6 to 300 characters", MinimumLength = 6)]
        [CustomRegularExpression(ModelRegularExpression.REG_CANT_START_FROM_SPACES, ErrorMessage = "Text can`t start from spaces")]
        [DataType(DataType.Text)]
        public string Text { get; set; }

        [Required(ErrorMessage = "post id is required")]
        public string PostId { get; set; }
    }
}
