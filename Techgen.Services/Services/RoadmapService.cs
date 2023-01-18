﻿using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Techgen.Common.Extensions;
using Techgen.Common.Markdown.MarkdownParser;
using Techgen.DAL.Abstract;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Domain.Entities.RoadmapEntity;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;
 
namespace Techgen.Services.Services
{
    public class RoadmapService : IRoadmapService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _appEnvironment;

        public RoadmapService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment appEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;          
            _appEnvironment = appEnvironment;
        }

        public async Task<IBaseResponse<RoadmapModelResponse>> Create(IFormFile image, string markdown)
        {
            if (image == null && markdown == null)
            {
                return new BaseResponse<RoadmapModelResponse> { Description = "roadmap is null", StatusCode = System.Net.HttpStatusCode.BadRequest };
            }

            var roadmap = new Roadmap();

            if (image != null)
            {
                string path = "/Resources/RoadmapImages/" + image.Name;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                roadmap.ImageName = image.FileName;
                roadmap.Path = path;
            }

            if (markdown != null)
            {
                roadmap.Markdown = Markdown.Parse(markdown);
            }

            await _unitOfWork.Repository<Roadmap>().InsertOneAsync(roadmap);

            var response = _mapper.Map<RoadmapModelResponse>(roadmap);
            return new BaseResponse<RoadmapModelResponse>
            {
                Data = response,
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }

        public IBaseResponse<RoadmapModelResponse> Get(string id)
        {
            var roadmap = _unitOfWork.Repository<Roadmap>().FindById(id);
            if(roadmap == null)
            {
                return new BaseResponse<RoadmapModelResponse> { Description = "that roadmap does not exist", StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            var response = _mapper.Map<RoadmapModelResponse>(roadmap);
            return new BaseResponse<RoadmapModelResponse>{ Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }

        public IBaseResponse<RoadmapModelResponse> Delete (string id)
        {
            var roadmap = _unitOfWork.Repository<Roadmap>().FindById(id);
            if (roadmap == null)
            {
                return new BaseResponse<RoadmapModelResponse>() { Description = "that roadmap does not exist", StatusCode = System.Net.HttpStatusCode.NotFound };
            }
              
            _unitOfWork.Repository<Roadmap>().DeleteById(id);
            return new BaseResponse<RoadmapModelResponse>() { Description = $"{id} was deleted", StatusCode = System.Net.HttpStatusCode.OK };
        }

        public async Task<IBaseResponse<RoadmapModelResponse>> Update(string id, RoadmapRequestModel model)
        {
            if (model.Image == null && model.Markdown == null)
            {
                return new BaseResponse<RoadmapModelResponse> { Description = "roadmap is null", StatusCode = System.Net.HttpStatusCode.BadRequest };
            }

            var roadmap = _unitOfWork.Repository<Roadmap>().FindById(id);
            if (roadmap == null)
            {
                return new BaseResponse<RoadmapModelResponse>() { Description = "that roadmap does not exist", StatusCode = System.Net.HttpStatusCode.NotFound };
            }

            if(model.Image != null)
            {          
                string path = "/Resources/RoadmapImages/" + model.Image.Name;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                roadmap.ImageName = model.Image.FileName;
                roadmap.Path = path;              
            }

            if(model.Markdown != null)
            {
                roadmap.Markdown = Markdown.Parse(model.Markdown);
            }

            _unitOfWork.Repository<Roadmap>().ReplaceOne(roadmap);

            var response = _mapper.Map<RoadmapModelResponse>(roadmap);
            return new BaseResponse<RoadmapModelResponse> { Data = response, StatusCode = System.Net.HttpStatusCode.OK };
        }
    }
}
