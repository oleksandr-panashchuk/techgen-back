﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.RequestModels
{
    public class TokenRequestModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
