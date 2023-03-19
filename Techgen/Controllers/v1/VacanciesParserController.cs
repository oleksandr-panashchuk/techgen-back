using Microsoft.AspNetCore.Mvc;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels;
using Techgen.Services.Interfaces;
using Microsoft.Extensions.Localization;
using Techgen.ResourceLibrary;

namespace Techgen.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class VacanciesParserController : _BaseApiController
    {
        private readonly IVacanciesParser _vacanciesParser;

        public VacanciesParserController(IStringLocalizer<ErrorsResource> errorsLocalizer, IVacanciesParser vacanciesParser) : base(errorsLocalizer)
        {
            _vacanciesParser = vacanciesParser;
        }
        
        [Route("get-vacancies")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] VacancyRequestModel model)
        {
            var response = await _vacanciesParser.GetAll(model);
            return Json(new JsonResponse<IBaseResponse<List<VacancyResponseModel>>>(response));
        }
    }
}
