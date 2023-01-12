﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.ResponseModels.Base
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public T Data { get; set; }

        public BaseResponse(string description)
        {
            Description = description;
        }

        public BaseResponse(T data)
        {
            Data = data;
        }

        public BaseResponse()
        {

        }
    }

    public interface IBaseResponse<T>
    {
        string Description { get; }
        HttpStatusCode StatusCode { get; }
        T Data { get; }
    }
}
