using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels;

namespace Techgen.Services.Interfaces
{
    public interface IVacanciesParser
    {
        public Task<BaseResponse<List<VacancyResponseModel>>> GetAll(VacancyRequestModel model);
    }
}
