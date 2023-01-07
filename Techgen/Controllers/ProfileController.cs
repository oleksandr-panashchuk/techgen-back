using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techgen.Models.RequestModels;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Route("edit")]
        [HttpPost]
        public IActionResult Edit([FromBody] ProfileRequestModel profile)
        {
            if (ModelState.IsValid)
            {
                var response = _profileService.Edit(profile);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(new { Message = "Edit profile Successfully" });
                }
            }
            return new BadRequestObjectResult(new { Message = "Failed to edit profile" });
        }

        [Route("details")]
        [HttpGet]
        public IActionResult Details()
        {
            var response = _profileService.Get(User.Identity.Name);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return new BadRequestObjectResult(new { Message = "Something went wrong" });
        }
    }
}
