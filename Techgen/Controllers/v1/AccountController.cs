using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Techgen.Services.Interfaces;
using System.Net;
using Techgen.Models.RequestModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Localization;
using Techgen.ResourceLibrary;
using Techgen.Models.ResponseModels;
using Techgen.Common.Constants;
using Techgen.Models.ResponseModels.Session;
using Swashbuckle.AspNetCore.Annotations;
using Techgen.Helpers.Attributes;
using Techgen.Models.ResponseModels.Base;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class AccountController : _BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IStringLocalizer<ErrorsResource> errorsLocalizer, IAccountService acccoutService) : base(errorsLocalizer)
        {
            _accountService = acccoutService;
        }

        // POST api/v1/account/register
        /// <summary>
        /// Register new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/account/register
        ///     {                
        ///         "email" : "test@example.com",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with user email and info about email status or HTTP 4XX, 500 with error message</returns>
        [AllowAnonymous]
        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<RegisterResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            var response = await _accountService.Register(model);
            return Json(new JsonResponse<IBaseResponse<RegisterResponseModel>>(response));
        }

        // POST api/v1/account
        /// <summary>
        /// Login
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/acoount/login
        ///     {                
        ///         "email" : "test@example.com",
        ///         "password" : "1simplepassword",
        ///         "AccessTokenLifetime" : "10000"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 201 with token and user response model or HTTP 4XX, 500 with error message</returns>
        [AllowAnonymous]
        [SwaggerResponse(201, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<IBaseResponse<LoginResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var response = await _accountService.Login(model);
            return Json(new JsonResponse<IBaseResponse<LoginResponseModel>>(response));
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return Json(new MessageResponseModel("you have been logout"));
        }

        /*[Route("checkRecoveryCode")]
        [HttpPost]
        public async Task<IActionResult> CheckRecoveryCode([FromQuery] string email, [FromQuery] string recoveryCode)
        {
            var response = await _accountService.CheckRecoveryCode(email, recoveryCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return new BadRequestObjectResult(response.Description);
        }*/

        /*[HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromQuery] string email, [FromQuery] string newPassword)
        {
            var response = await _accountService.ChangePassword(email, newPassword);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(new { Message = "Change password successfully" });
            }
            return new BadRequestObjectResult(new { Message = response.Description });
        }*/

    }
}
