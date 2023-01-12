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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            

        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
        }
    }
}
