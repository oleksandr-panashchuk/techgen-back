using Techgen.DAL;
using Techgen.DAL.Abstract;
using Techgen.DAL.Interfaces;
using Techgen.DAL.Repositories;
using Techgen.DAL.Repository;
using Techgen.Domain.Entity;
using Techgen.Services.Implementations;
using Techgen.Services.Interfaces;

namespace Techgen
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();

        }
    }
}
