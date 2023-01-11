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

namespace Techgen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : _BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IStringLocalizer<ErrorsResource> errorsLocalizer, IAccountService acccoutService) : base(errorsLocalizer)
        {
            _accountService = acccoutService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            var response = await _accountService.Register(model);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok("User created successfully.");
            }
            return new BadRequestObjectResult(new { Message = response.Description });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var response = await _accountService.Login(model);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPost]
        [Route("revoke")]
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
