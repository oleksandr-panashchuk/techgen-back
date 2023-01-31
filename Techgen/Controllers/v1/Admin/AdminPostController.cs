using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.Models.Enum;
using Techgen.Models.RequestModels.Post;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    public class AdminPostController : _BaseApiController
    {
        private readonly IPostService _postService;

        public AdminPostController(IStringLocalizer<ErrorsResource> errorsLocalizer,
                                   IPostService postService) : base(errorsLocalizer)
        {
            _postService = postService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePost([FromBody] PostRequestModel model)
        {
            var response = await _postService.Create(model);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(response);
            return Forbidden();
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditPost([FromBody] PostRequestModel model, [FromRoute] string id)
        {
            var response = await _postService.Update(id, model);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(response);
            return Forbidden();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] string id)
        {
            var response = await _postService.Delete(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(response);
            return Forbidden();
        }
    }
}
