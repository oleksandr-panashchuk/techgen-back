using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using PasswordGenerator;
using System.Data;
using Techgen.Domain.DB;
using Techgen.Domain.Entity;
using Techgen.Domain.Enum;

namespace Techgen
{
    public static class DataSeed
    {
        public static void SeedData(IMongoDatabase mongoDb, IServiceProvider serviceProvider)
        {       
            var users = mongoDb.GetCollection<ApplicationUser>("Users");

            //Create admins
            var admins = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "admin1@gmail.com",
                    Email = "admin1@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "admin2@gmail.com",
                    Email = "admin2@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "admin3@gmail.com",
                    Email = "admin3@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };
            var adminPassword = "Admin_123";

            foreach (var user in admins)
            {
                if (!users.AsQueryable().Any(u => u.Email == user.Email))
                {
                    SeedAdmin(user, adminPassword, "Admin");
                }
            }
            
            void SeedAdmin(ApplicationUser user, string password, string role)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                IdentityResult result = userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                }
            }
        }
    }
}
