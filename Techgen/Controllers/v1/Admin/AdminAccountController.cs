using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Common.Constants;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels.Session;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-account")]
    public class AdminAccountController : _BaseApiController
    {
        private readonly IAccountService _accountService;
        public AdminAccountController(IStringLocalizer<ErrorsResource> errorsLocalizer,
            IAccountService accountService) : base(errorsLocalizer)
        {
            _accountService = accountService;
        }
        #region Admin
        // POST api/v1/admin-account/
        /// <summary>
        /// Register new admin
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/admin-account/
        ///     {                
        ///         "email" : "test@example.com",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with user email and info about email status or HTTP 4XX, 500 with error message</returns>
        [AllowAnonymous]
        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RegisterResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestModel model)
        {
            var response = await _accountService.RegisterAdmin(model);
            return Json(new JsonResponse<IBaseResponse<RegisterResponseModel>>(response));
        }

        #endregion
    }
}
