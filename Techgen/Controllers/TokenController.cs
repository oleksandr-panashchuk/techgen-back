using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Helpers;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {   
        private readonly IJWTService _jwtService;

        public TokenController(IJWTService jwtService, IUnitOfWork unitOfWork)
        {
            _jwtService = jwtService;
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenRequestModel tokenModel)
        {
            var response = _jwtService.RefreshToken(tokenModel);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest("Invalid client request");
            }
            return Ok(response.Data);
        }
    }
}
