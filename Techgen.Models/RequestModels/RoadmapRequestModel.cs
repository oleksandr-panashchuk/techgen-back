using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.RequestModels
{
    public class RoadmapRequestModel
    {
        public IFormFile Image { get; set; }
        public string Markdown { get; set; }
    }
}
