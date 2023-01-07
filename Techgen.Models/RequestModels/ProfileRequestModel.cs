using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.RequestModels
{
    public class ProfileRequestModel
    {   
        public string Email { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string? Country { get; set; }
    }
}
