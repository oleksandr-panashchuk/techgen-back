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

namespace Techgen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService acccoutService)
        {
            _accountService = acccoutService;
        }

        [HttpPost]
        [Route("Register")]
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
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var response = await _accountService.Login(model);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(response.Data),
                    expiration = response.Data.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromQuery] string email, [FromQuery] string newPassword)
        {
            var response = await _accountService.ChangePassword(email, newPassword);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(new { Message = "Change password successfully" });
            }
            return new BadRequestObjectResult(new { Message = response.Description });
        }

    }
}
