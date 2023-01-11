using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Attributes;

namespace Techgen.Models.RequestModels.Post
{
    public class PostRequestModel
    {
        [StringLength(50, ErrorMessage = "Name should be from 6 to 50 characters", MinimumLength = 6)]
        [CustomRegularExpression(ModelRegularExpression.REG_CANT_START_FROM_SPACES, ErrorMessage = "Name can`t start from spaces")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES_ONLY, ErrorMessage = "Name can’t contain spaces only")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(300, ErrorMessage = "Text should be from 6 to 50 characters", MinimumLength = 6)]
        [CustomRegularExpression(ModelRegularExpression.REG_CANT_START_FROM_SPACES, ErrorMessage = "Text can`t start from spaces")]
        [DataType(DataType.Text)]
        public string Text { get; set; }
    }
}
