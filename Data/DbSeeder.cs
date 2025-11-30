using Microsoft.AspNetCore.Identity;

namespace Mini_Social_Media.Data {
    public static class DbSeeder {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service) {
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole<int>>>();

            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            await roleManager.CreateAsync(new IdentityRole<int>("User"));

            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null) {
                var newAdmin = new User {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true,
                    AvatarUrl = "/images/default-avatar.png",
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(newAdmin, "Admin@12345");

                if (createResult.Succeeded) {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}