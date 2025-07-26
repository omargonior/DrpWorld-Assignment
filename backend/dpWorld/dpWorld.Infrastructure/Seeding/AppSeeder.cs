using dpWorld.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Infrastructure.Seeding
{
    public static class AppSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string email = "admin@db.com";
            string password = "Admin@123"; //  Strong password for demo
            string phone = "01153552018";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var adminUser = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = email,
                    Email = email,
                    PhoneNumber = phone,
                    NationalId = "30405060708090",
                    Age = 22
                };

                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
