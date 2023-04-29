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

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentController : _BaseApiController
    {
        private readonly ICommentService _commentService;
        public CommentController(IStringLocalizer<ErrorsResource> errorsLocalizer, ICommentService commentService) : base(errorsLocalizer)
        {
            _commentService = commentService;
        }

        // POST api/v1/comment/
        /// <summary>
        /// Create a comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/comment/
        ///     {                
        ///         "Text" : "sample comment",
        ///         "PostId" : "123",
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with comment response model or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<CommentResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("")]
        public async Task<IActionResult> Comment([FromBody] CommentRequestModel model)
        {
            var response = await _commentService.Create(model);
            return Json(new JsonResponse<IBaseResponse<CommentResponseModel>>(response));
        }

        // POST api/v1/comment/{id}
        /// <summary>
        /// Create an answer for a comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/comment/123
        ///     {                
        ///         "Text" : "sample answer",
        ///         "PostId" : "123",
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with comment response model or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<CommentResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("{id}")]
        public async Task<IActionResult> Answer([FromRoute]int id,[FromBody] CommentRequestModel model)
        {
            var response = await _commentService.CreateAnswer(model, id);
            return Json(new JsonResponse<IBaseResponse<CommentResponseModel>>(response));
        }

        // DELETE api/v1/comment/{id}
        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/comment/123
        ///
        /// </remarks>
        /// <returns>HTTP 201 with comment response model or HTTP 4XX, 500 with error message</returns>
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<CommentResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> UserDeleteComment([FromRoute] int id)
        {
            var response = await _commentService.UserDelete(id);
            return Json(new JsonResponse<IBaseResponse<MessageResponseModel>>(response));
        }

        // DELETE api/v1/comment/
        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/comment/
        ///
        /// </remarks>
        /// <returns>HTTP 201 with comment response model or HTTP 4XX, 500 with error message</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
        [SwaggerResponse(201, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IBaseResponse<CommentResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpDelete("")]
        public async Task<IActionResult> AdminDeleteComment([FromBody] AdminDeleteCommentRequestModel model)
        {
            var response = await _commentService.AdminDelete(model);
            return Json(new JsonResponse<IBaseResponse<MessageResponseModel>>(response));
        }

        
    }
}
