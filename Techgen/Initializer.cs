using Microsoft.AspNetCore.CookiePolicy;
using System.Collections;
using Techgen.Common.Utilities;
using Techgen.Common.Utilities.Interfaces;
using Techgen.DAL;
using Techgen.DAL.Abstract;
using Techgen.DAL.Repository;
using Techgen.DAL.UnitOfWork;
using Techgen.EmailService;
using Techgen.Services.Interfaces;
using Techgen.Services.Services;

namespace Techgen
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            

        }
    }
}
