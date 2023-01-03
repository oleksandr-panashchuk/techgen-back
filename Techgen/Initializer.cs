using System.Collections;
using Techgen.Common.Utilities;
using Techgen.Common.Utilities.Interfaces;
using Techgen.DAL;
using Techgen.DAL.Abstract;
using Techgen.DAL.Repository;
using Techgen.DAL.UnitOfWork;
using Techgen.Domain.Entity;
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
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}
