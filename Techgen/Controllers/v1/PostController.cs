using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : _BaseApiController
    {
        private readonly IPostService _postService;

        public PostController(IStringLocalizer<ErrorsResource> errorsLocalizer,
                              IPostService postService) : base(errorsLocalizer)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] string id)
        {
            var response = await _postService.Get(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(response);
            return NotFound();
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _postService.GetAll();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Ok(response);
            return Forbidden();
        }

    }
}
