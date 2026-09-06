using Microsoft.AspNetCore.Identity;

namespace EMEDS_Project.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roles =
            {
                "Admin",
                "Customer"
            };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            // Create default Admin user if it does not exist.
            string adminEmail = "admin@emedstest.com";
            string adminPassword = "Admin@123";

            ApplicationUser? adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result =
                    await userManager.CreateAsync(
                        adminUser,
                        adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(
                        adminUser,
                        "Admin"))
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");
                }
            }
        }
    }
}