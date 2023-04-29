using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Common.Constants;
using Techgen.Common.Helpers.SwaggerFilters;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Session;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    public class RoadmapController : _BaseApiController
    {
        private readonly IRoadmapService _roadmapService;
        public RoadmapController(IStringLocalizer<ErrorsResource> errorsLocalizer, IRoadmapService roadmapService) : base(errorsLocalizer)
        {
            _roadmapService = roadmapService;
        }


        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RoadmapResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]

        [HttpPost("CreateRoadMap")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(IFormFile image,  [FromForm] string markdown)
        {
            var response = await _roadmapService.Create(image, markdown);

            return Json(new JsonResponse<IBaseResponse<RoadmapResponseModel>>(response));
        }

        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RoadmapResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]

        [HttpPost("TestCreateRoadMap")]
        public async Task<IActionResult> TestCreat(IFormFile image, string markdown)
        {
            var response = await _roadmapService.Create(image, markdown);

            return Json(new JsonResponse<IBaseResponse<RoadmapResponseModel>>(response));
        }

        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RoadmapResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]

        [HttpGet("GetRoadMap")]
        public async Task<IActionResult> Get(int id)
        {
            var response =  _roadmapService.Get(id);

            return Json(new JsonResponse<IBaseResponse<RoadmapResponseModel>>(response));
        }

        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RoadmapResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]

        [HttpDelete("DeleteRoadMap")]
        public async Task<IActionResult> Delete(int id)
        {
            var response =  _roadmapService.Delete(id);

            return Json(new JsonResponse<IBaseResponse<RoadmapResponseModel>>(response));
        }


        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RoadmapResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]

        [HttpPut("UpdateRoadMap")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id,  [FromForm] RoadmapRequestModel model)
        {
            var response = await _roadmapService.Update(id, model);

            return Json(new JsonResponse<IBaseResponse<RoadmapResponseModel>>(response));
        }

    }
}
