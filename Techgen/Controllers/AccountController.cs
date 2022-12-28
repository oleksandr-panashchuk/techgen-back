using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Techgen.Services.Implementations;
using Techgen.Services.Interfaces;
using Techgen.Domain.Models.Account;
using Techgen.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Techgen.Domain.Helpers;
using AspNetCore.Identity.MongoDbCore.Models;
using System.ComponentModel.DataAnnotations;

namespace Techgen.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private RoleManager<ApplicationRole> _roleManager;

        public AccountController(IAccountService acccoutService, RoleManager<ApplicationRole> roleManager)
        {
            _accountService = acccoutService;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var response = await _accountService.Login(model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
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
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            var response = await _accountService.Register(model);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok("User created successfully.");
            }
            return new BadRequestObjectResult(new { Message = response.Data });
        }

        [Route("SendRecoveryCode")]
        [HttpPost]
        public async Task<IActionResult> SendRecoveryCode([FromQuery] string email, [FromQuery] string recoveryCode)
        {
            var response = await _accountService.SendEmailNewRecoveryCode(email, recoveryCode);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(new { Message = "Send message with new recovery code successfully" });
            }
            return new BadRequestObjectResult(new { Message = "Send messagen with new recovery failed" });
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromQuery] string email, [FromQuery] string recoveryCode)
        {
            var response = await _accountService.SendEmailNewPassword(email, recoveryCode);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return Ok(new { Message = "Change password successfully" });
            }
            return new BadRequestObjectResult(new { Message = "Change password failed" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromQuery]string name)
        {           
            IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = name });
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role Created Successfully"});
            }                                           
            return new BadRequestObjectResult(new { Message = "Failed to create role" });
        }
    }
}
