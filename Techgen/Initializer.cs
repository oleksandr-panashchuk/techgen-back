using Techgen.DAL.Interfaces;
using Techgen.DAL.Repositories;
using Techgen.Domain.Entity;
using Techgen.Services.EmailService;
using Techgen.Services.Implementations;
using Techgen.Services.Interfaces;

namespace Techgen
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}
