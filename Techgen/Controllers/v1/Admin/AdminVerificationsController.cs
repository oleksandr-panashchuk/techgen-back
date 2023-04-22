using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.Helpers.Attributes;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-verifications")]
    [Validate]
    public class AdminVerificationsController : _BaseApiController
    {
        private readonly IAccountService _accountService;

        public AdminVerificationsController(IStringLocalizer<ErrorsResource> errorsLocalizer,
            IAccountService accountService)
            : base(errorsLocalizer)
        {
            _accountService = accountService;
        }

    }
}
