using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.RoadmapEntity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Models.ResponseModels;

namespace Techgen.Services.Interfaces
{
    public interface IRoadmapService
    {
        public Task<IBaseResponse<RoadmapModelResponse>> Create(IFormFile image, string markdown);

        public IBaseResponse<RoadmapModelResponse> Get(int id);

        public IBaseResponse<RoadmapModelResponse> Delete(int id);

        public Task<IBaseResponse<RoadmapModelResponse>> Update(int id, RoadmapRequestModel model);
    }
}
