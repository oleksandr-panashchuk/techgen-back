using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.Helpers.Attributes;
using Techgen.Models.Enum;
using Techgen.ResourceLibrary;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class VerificationsController : _BaseApiController
    {
        public VerificationsController(IStringLocalizer<ErrorsResource> errorsLocalizer) : base(errorsLocalizer)
        {
        }

        [HttpGet("email")]
        public async Task<IActionResult> Email([FromQuery]string token)
        {
            return View();
        }
    }
}
