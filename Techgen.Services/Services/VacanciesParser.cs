using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Dom.Events;
using MediaBrowser.Common.Events;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Techgen.Common.Constants;
using Techgen.Common.Exceptions;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.Services.Interfaces;

namespace Techgen.Services.Services
{
    public class VacanciesParser : IVacanciesParser
    {
        public async Task<BaseResponse<List<VacancyResponseModel>>> GetAll(VacancyRequestModel model)
        {             
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var jobList = new List<VacancyResponseModel>();

            var tasks = new List<Task<List<VacancyResponseModel>>>
            {
               Task.Run(() => WorkUaParseAsync(context, model)),
               //Task.Run(() => DOUParseAsync(context, model)),
               Task.Run(() => DjinniParseAsync(context, model))
            };

            List<VacancyResponseModel>[] results = await Task.WhenAll(tasks);

            foreach(var result in results)
            {
                jobList.AddRange(result);
            }
            return new BaseResponse<List<VacancyResponseModel>> { Data = jobList, StatusCode = System.Net.HttpStatusCode.OK };
        }

        private async Task<List<VacancyResponseModel>> WorkUaParseAsync(IBrowsingContext context, VacancyRequestModel model)
        {
            string url = CreateUrlWorkUa(model, JobSiteUrl.WORK_UA_Search);

            var document = await context.OpenAsync(url);

            int page = GetPaginationForWorkUa(document);

            var jobList = new List<VacancyResponseModel>();

            for (int i = 1; i <= page; i++)
            {
                
                string ConstUrl = $"https://www.work.ua/jobs/?page={i}?category=1&search=";
 
                string PageUrl = CreateUrlWorkUa(model, ConstUrl);

                var PageDocument = await context.OpenAsync(PageUrl);

                var vacancies = PageDocument.QuerySelectorAll(".job-link");

                foreach (var vacancy in vacancies)
                {

                    string title = vacancy.QuerySelector("h2").TextContent.Trim();
                    string companyName = vacancy.QuerySelector(".add-top-xs").QuerySelector("b").TextContent.Trim();
                    string jobLink = JobSiteUrl.WORK_UA + vacancy.QuerySelector("a").GetAttribute("href");
                    string? companyLogoUrl = vacancy.QuerySelector("img")?.GetAttribute("src");

                    string[] vacancyDescription = vacancy.QuerySelector("p").TextContent.Split(".");
                    string employment = vacancyDescription[0].Trim();
                    string content = vacancyDescription[^1].Trim();

                    string date = vacancy.QuerySelector(".pull-right").QuerySelector("span").TextContent;

                    jobList.Add(
                        new VacancyResponseModel
                        {
                            Title = title,
                            CompanyName = companyName,
                            CompanyLogoUrl = companyLogoUrl,
                            Content = content,
                            Employment = employment,
                            JobLink = jobLink,
                            Date = date
                        });
                }
            }
            return jobList;
        }
        private string CreateUrlWorkUa(VacancyRequestModel model, string Turl)
        {
           // var page = GetPaginationForWorkUa()

            StringBuilder url = new StringBuilder(Turl);
            
            if (model.SearchQuery != null)
            {
                url.Append(' ' + model.SearchQuery);
            }
            if (model.Direction != null)
            {
                url.Append(' ' + model.Direction);
            }          
            if(model.Employment != null)
            {            
                if (model.Employment == "fulltime")
                {
                    url.Append("/&employment=74");
                }
                if (model.Employment == "parttime")
                {
                    url.Append("/&employment=75");
                }
            }

            return url.ToString();
        }

        private int GetPaginationForWorkUa(IDocument document)
        {
            try
            {
                var block = document.QuerySelector(".pagination").QuerySelectorAll(".page-link");
                int page = block != null ? Int32.Parse(block[block.Length - 2].TextContent) : 1;
                return page;
            }
            catch (Exception)
            {
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Job is not found", "Such job is not found");
            }
        }

        private async Task<List<VacancyResponseModel>> DOUParseAsync(IBrowsingContext context, VacancyRequestModel model)
        {
            string url = CreateUrlDOU(model);

            var document = await context.OpenAsync(url);

            var vacancies = document.QuerySelectorAll(".l-vacancy");

            var jobList = new List<VacancyResponseModel>();

            foreach (var vacancy in vacancies)
            {
                string title = vacancy.QuerySelector(".vt").TextContent.Trim();
                string companyName = vacancy.QuerySelector(".company").TextContent.Trim();
                string jobLink = vacancy.QuerySelector(".company").GetAttribute("href");
                string? companyLogoUrl = vacancy.QuerySelector(".company").QuerySelector("img")?.GetAttribute("src");
                string content = vacancy.QuerySelector(".sh-info").TextContent.Trim();
                string date = vacancy.QuerySelector(".date")?.TextContent.Trim();

                jobList.Add(
                    new VacancyResponseModel
                    {
                        Title = title,
                        CompanyName = companyName,
                        CompanyLogoUrl = companyLogoUrl,
                        Content = content,
                        JobLink = jobLink,
                        Date = date
                    });
            }

            return jobList;
        }
        private string CreateUrlDOU(VacancyRequestModel model)
        {
            StringBuilder url = new StringBuilder(JobSiteUrl.DOU_Search);

            if (model.Direction != null)
            {
                url.Append("&category=" + model.Direction);
            }
            if (model.SearchQuery != null)
            {
                url.Append("&search=" + model.SearchQuery);
            }

            return url.ToString();
        }

        private async Task<List<VacancyResponseModel>> DjinniParseAsync(IBrowsingContext context, VacancyRequestModel model)
        {
            string url = CreateUrlDjinni(model, JobSiteUrl.DJINNI_Search);

            var document = await context.OpenAsync(url);

            int page = PaginationFromDjinni(document);

            var jobList = new List<VacancyResponseModel>();

            for (int i = 1; i <= page; i++)
            {
                string ConstUrl = $"https://djinni.co/jobs/?page={i}";
 
                string Pageurl = CreateUrlDjinni(model, ConstUrl);

                var PageDocument = await context.OpenAsync(Pageurl);

                var vacancies = PageDocument.QuerySelectorAll(".list-jobs__item");

                foreach (var vacancy in vacancies)
                {
                    string title = vacancy.QuerySelector(".profile").TextContent.Trim();
                    string companyName = vacancy.QuerySelector(".list-jobs__details__info").QuerySelector("a").TextContent.Trim();
                    string jobLink = JobSiteUrl.DJINNI + vacancy.QuerySelector(".profile").GetAttribute("href");
                    string? companyLogoUrl = vacancy.QuerySelector(".userpic-image").GetAttribute("style")
                        .Replace("background-image: url('", "").Replace("')", "");

                    string content = vacancy.QuerySelector(".list-jobs__description").QuerySelector(".truncated").TextContent.Trim();

                    var vacancyDescription = vacancy.QuerySelector(".list-jobs__details__info").QuerySelectorAll("nobr");

                    var empoyment = vacancyDescription.Length >= 4
                        ? vacancyDescription[1].TextContent.Trim()
                        : vacancyDescription[0].TextContent.Trim();

                    var experience = vacancyDescription.Length >= 4
                        ? vacancyDescription[2].TextContent.Trim().Substring(2)
                        : vacancyDescription[1].TextContent.Trim().Substring(2);

                    string dateBlock = vacancy.QuerySelector(".text-date").TextContent;
                    string pattern = @"^\s*(\d+\s+[а-яА-Я]+\b)";
                    Match match = Regex.Match(dateBlock, pattern);
                    string date = match.Groups[1].Value.Trim();

                    jobList.Add(
                        new VacancyResponseModel
                        {
                            Title = title,
                            CompanyName = companyName,
                            CompanyLogoUrl = companyLogoUrl,
                            Content = content,
                            Employment = empoyment,
                            Experience = experience,
                            JobLink = jobLink,
                            Date = date
                        });
                }
            }

            return jobList;
        }
        private string CreateUrlDjinni(VacancyRequestModel model,string Turl)
        {
            StringBuilder url = new StringBuilder(Turl);

            if (model.Direction != null)
            {
                url.Append("&primary_keyword=" + model.Direction);
            }
            if (model.SearchQuery != null)
            {
                url.Append("&keywords=" + model.SearchQuery);
            }
            if(model.Employment != null)
            {               
                url.Append("&employment=" + model.Employment);               
            }

            return url.ToString();
        }

        private int PaginationFromDjinni(IDocument document)
        {
            try
            {
                var block = document.QuerySelector(".pagination").QuerySelectorAll(".page-link");
                int page = block!=null?Int32.Parse(block[block.Length - 2].TextContent): 1;
                return page;
            }
            catch (Exception)
            {
                throw new CustomException(System.Net.HttpStatusCode.NotFound, "Job is not found", "Such job is not found");
            }
            
        }

    }
}
