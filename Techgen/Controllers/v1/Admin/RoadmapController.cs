using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Techgen.ResourceLibrary;
using Techgen.Services.Interfaces;

namespace Techgen.Controllers.v1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class RoadmapController : _BaseApiController
    {
        private readonly IRoadmapService _roadmapService;
        public RoadmapController(IStringLocalizer<ErrorsResource> errorsLocalizer, IRoadmapService roadmapService) : base(errorsLocalizer)
        {
            _roadmapService = roadmapService;
        }

    }
}
