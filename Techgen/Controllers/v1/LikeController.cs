using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Common.Constants;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1
{  
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class LikeController : _BaseApiController
    {
       private readonly ILikeService _likeSwrvice;

        public LikeController(IStringLocalizer<ErrorsResource> errorsLocalizer,ILikeService likeService  ) : base(errorsLocalizer)
        {
            _likeSwrvice = likeService;
        }

        [SwaggerResponse(201, ResponseMessages.SuccessfulLogin, typeof(JsonResponse<IBaseResponse<PostResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("CreateLike")]
        public async Task<IActionResult> Create (int postId)
        {
            var response = await _likeSwrvice.Create (postId);
            return Json(new JsonResponse<IBaseResponse<PostResponseModel>>(response));
        }

    }
}
