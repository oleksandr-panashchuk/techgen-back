using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.Helpers.Attributes;
using Techgen.Models.Enum;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1.Admin
{
    [Validate]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-settings")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admins)]
    public class AdminSettingsController : _BaseApiController
    {
        private readonly IAccountService _accountService;

        public AdminSettingsController(IStringLocalizer<ErrorsResource> errorsLocalizer,
            IAccountService accountService)
            : base(errorsLocalizer)
        {
            _accountService = accountService;
        }

    }
}
