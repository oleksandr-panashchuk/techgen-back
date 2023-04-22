﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;
using Techgen.Models.Enum;

namespace Techgen.DAL.Migrations
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context, IConfiguration Configuration, IServiceProvider serviceProvider)
        {
            context.Database.Migrate();

            #region Roles

            new List<ApplicationRole>
            {
                new ApplicationRole { Name = "User", NormalizedName = "USER" },
                new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN" },
                new ApplicationRole { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" }
            }
            .ForEach(i =>
            {
                if (!context.Roles.Any(x => x.Name == i.Name))
                    context.Roles.Add(i);
            });

            context.SaveChanges();

            #endregion

            #region Creating Super Admins

            var moderators = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "admin@superadmin.com",
                    Email = "admin@superadmin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "dev@superadmin.com",
                    Email = "dev@superadmin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "dev2@moderator.com",
                    Email = "dev2@moderator.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "qa@superadmin.com",
                    Email = "qa@superadmin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "qa2@superadmin.com",
                    Email = "qa2@superadmin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
            };

            var admins = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "dev@admin.com",
                    Email = "dev@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "dev2@admin.com",
                    Email = "dev2@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "qa@admin.com",
                    Email = "qa@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    UserName = "qa2@admin.com",
                    Email = "qa2@admin.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
            };

            var password = "Admin123";

            foreach (var user in moderators)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    SeedAdmin(user, password, Role.SuperAdmin);
            }

            foreach (var user in admins)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    SeedAdmin(user, password, Role.Admin);
            }

            void SeedAdmin(ApplicationUser user, string passwordString, string role)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                IdentityResult result = userManager.CreateAsync(user, passwordString).Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, role).Wait();
            }

            #endregion 
        }
    }
}
