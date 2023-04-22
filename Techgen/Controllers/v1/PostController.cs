using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using System.Web.Http.Results;
using Techgen.Common.Constants;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Models.ResponseModels.Session;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class PostController : _BaseApiController
    {
        private readonly IPostService _postService;

        public PostController(IStringLocalizer<ErrorsResource> errorsLocalizer,
                              IPostService postService) : base(errorsLocalizer)
        {
            _postService = postService;
        }

        // GET api/v1/post/
        /// <summary>
        /// Get post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/post/123
        ///
        /// </remarks>
        /// <returns>HTTP 201 with PostResponseModel or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<PostResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(404, ResponseMessages.BadRequest, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            var response = await _postService.Get(id);
            return Json(new JsonResponse<IBaseResponse<PostResponseModel>>(response));
        }

        // GET api/v1/post/getall
        /// <summary>
        /// Get all posts
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/post/getall
        ///
        /// </remarks>
        /// <returns>HTTP 201 with PostResponseModel or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<IEnumerable<SmallPostResponseModel>>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(404, ResponseMessages.BadRequest, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _postService.GetAll();
            return Json(new JsonResponse<IBaseResponse<IEnumerable<SmallPostResponseModel>>>(response));

        }

    }
}
