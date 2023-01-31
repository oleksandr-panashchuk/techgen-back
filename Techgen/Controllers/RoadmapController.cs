using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadmapController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        public RoadmapController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(IFormFile image, string markdown)
        {
            var response = await _roadmapService.Create(image, markdown);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Description);
        }

        [HttpGet("get")]
        public IActionResult Get(string id)
        {
            var response = _roadmapService.Get(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return NotFound();
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(string id, RoadmapRequestModel model)
        {
            var response = await _roadmapService.Update(id, model);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Description);
        }

        [HttpPost("delete")]
        public IActionResult Delete(string id)
        {
            var response = _roadmapService.Delete(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(response.Data);
            }
            return NotFound(response.Description);
        }
    }
}
