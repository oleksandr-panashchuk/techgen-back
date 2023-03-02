using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Common.Constants;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels.Post;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Post;
using Techgen.Models.ResponseModels.Session;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    public class AdminPostController : _BaseApiController
    {
        private readonly IPostService _postService;

        public AdminPostController(IStringLocalizer<ErrorsResource> errorsLocalizer,
                                   IPostService postService) : base(errorsLocalizer)
        {
            _postService = postService;
        }

        // POST api/v1/adminpost/
        /// <summary>
        /// Create new post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/adminpost/
        ///     {                
        ///         "name" : "MyPost",
        ///         "text" : "That is text  ",
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with PostResponseModel or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.SuccessfulLogin, typeof(JsonResponse<IBaseResponse<PostResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("")]
        public async Task<IActionResult> CreatePost([FromBody] PostRequestModel model)
        {
            var response = await _postService.Create(model);
            return Json(new JsonResponse<IBaseResponse<PostResponseModel>>(response));
        }

        // POST api/v1/adminpost/edit/{id}
        /// <summary>
        /// Create new post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/adminpost/edit/123
        ///     {                
        ///         "name" : "MyPost2",
        ///         "text" : "That is text from post2 ",
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with PostResponseModel or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.SuccessfulLogin, typeof(JsonResponse<IBaseResponse<PostResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditPost([FromBody] PostRequestModel model, [FromRoute] int id)
        {
            var response = await _postService.Update(id, model);
            return Json(new JsonResponse<IBaseResponse<PostResponseModel>>(response));

        }

        // DELETE api/v1/adminpost/{id}
        /// <summary>
        /// Delete post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/adminpost/123
        ///
        /// </remarks>
        /// <returns>HTTP 201 with PostResponseModel or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(MessageResponseModel))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.Forbidden, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            var response = await _postService.Delete(id);
            return Json(new JsonResponse<IBaseResponse<MessageResponseModel>>(response));
        }
    }
}
