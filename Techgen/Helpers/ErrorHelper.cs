﻿using Microsoft.Extensions.Localization;
using Techgen.Common.Constants;
using Techgen.Models.ResponseModels.Base;
using Techgen.ResourceLibrary;

namespace Techgen.Helpers
{
    public class ErrorHelper
    {
        public static ErrorResponseModel GetError(IStringLocalizer<ErrorsResource> localizer, int code)
        {
            var errorResponse = new ErrorResponseModel(localizer);

            switch (code)
            {
                case 400:
                    errorResponse.Code = ErrorCode.BadRequest;
                    break;
                case 401:
                    errorResponse.Code = ErrorCode.Unauthorized;
                    errorResponse.AddError("token", "Token invalid");
                    break;
                case 403:
                    errorResponse.Code = ErrorCode.Forbidden;
                    break;
                case 404:
                    errorResponse.Code = ErrorCode.NotFound;
                    break;
                case 405:
                    errorResponse.Code = ErrorCode.NotAllowed;
                    break;
                case 500:
                    errorResponse.Code = ErrorCode.InternalServerError;
                    break;
            }

            return errorResponse;
        }
    }
}
