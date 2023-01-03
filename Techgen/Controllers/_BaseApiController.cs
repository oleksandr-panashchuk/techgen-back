using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Security.Claims;
using Techgen.Common.Constants;
using Techgen.Models.Enum;
using Techgen.Models.ResponseModels.Base;
using Techgen.ResourceLibrary;

namespace Techgen.Controllers
{
    public class _BaseApiController : Controller
    {
        protected ErrorResponseModel Errors;
        private readonly IStringLocalizer<ErrorsResource> _errorsLocalizer;

        public _BaseApiController(IStringLocalizer<ErrorsResource> errorsLocalizer)
        {
            _errorsLocalizer = errorsLocalizer;
            Errors = new ErrorResponseModel(_errorsLocalizer);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Forbidden()
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(new ErrorResponseModel(_errorsLocalizer)
                {
                    Code = ErrorCode.Forbidden,
                }, new JsonSerializerSettings { Formatting = Formatting.Indented }),
                StatusCode = 403,
                ContentType = "application/json"
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ContentResult Created(object reponse) => new ContentResult()
        {
            Content = JsonConvert.SerializeObject(reponse, new JsonSerializerSettings { Formatting = Formatting.Indented }),
            StatusCode = 201,
            ContentType = "application/json"
        };


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> IsAdminAsync(ClaimsPrincipal User)
        {
            return User.IsInRole(Role.Admin.ToString());
        }
    }
}
