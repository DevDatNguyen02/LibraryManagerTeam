using LibTeam.Models;
using Microsoft.AspNetCore.Identity;

namespace LibTeam.DbContext
{
    using Microsoft.AspNetCore.Identity;
    using LibTeam.Models;

    public class ApplicationSeedData
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Staff" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<AppUserModel> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");

            if (adminUser == null)
            {
                var newUser = new AppUserModel
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };
                await userManager.CreateAsync(newUser, "Admin@123");

                // Gán vai trò Admin cho người dùng này
                await userManager.AddToRoleAsync(newUser, "Admin");
            }
        }

        public static async Task SeedStaffUserAsync(UserManager<AppUserModel> userManager)
        {
            var staffUser = await userManager.FindByEmailAsync("staff@example.com");

            if (staffUser == null)
            {
                var newUser = new AppUserModel
                {
                    UserName = "staff",
                    Email = "staff@example.com",
                    EmailConfirmed = true,
                };
                await userManager.CreateAsync(newUser, "Staff@123");

                // Gán vai trò Staff cho người dùng này
                await userManager.AddToRoleAsync(newUser, "Staff");
            }
        }
    }


}
