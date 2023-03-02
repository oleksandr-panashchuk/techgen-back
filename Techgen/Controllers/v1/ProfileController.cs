using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class ProfileController : _BaseApiController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IStringLocalizer<ErrorsResource> errorsLocalizer, IProfileService profileService) : base(errorsLocalizer)
        {
            _profileService = profileService;
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ProfileRequestModel profile)
        {
            var response = await _profileService.Edit(profile);
            return Json(new JsonResponse<IBaseResponse<UserResponseModel>>(response));
        }

        [Route("details")]
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var response = await _profileService.Get();
            return Json(new JsonResponse<IBaseResponse<UserResponseModel>>(response));
        }
    }
}
