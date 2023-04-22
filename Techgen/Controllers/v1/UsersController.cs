using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Common.Constants;
using Techgen.Helpers.Attributes;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
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
    [Validate]
    public class UsersController : _BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public UsersController(IStringLocalizer<ErrorsResource> localizer,
            IUserService userService,
            IAccountService accountService)
             : base(localizer)
        {
            _userService = userService;
            _accountService = accountService;
        }

        // POST api/v1/users
        /// <summary>
        /// Register new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users
        ///     {                
        ///         "email" : "test@example.com",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with user email and info about email status or HTTP 4XX, 500 with error message</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "Register")]
        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<RegisterResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            return Created(new JsonResponse<IBaseResponse<RegisterResponseModel>>(await _accountService.Register(model)));
        }

    }
}
