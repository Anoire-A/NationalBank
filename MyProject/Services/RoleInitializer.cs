using Microsoft.AspNetCore.Identity;
using Project.Models;

namespace Project.Services
{
    public static class RoleInitializer
    {
        
        /// <param name="serviceProvider">The service provider instance.</param>
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            
            string[] roles = { "NationalBankAdmin", "CommercialBankUser" };

            
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            string adminEmail = "admin@nationalbank.gov";
            string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "National Bank Administrator",
                    
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "NationalBankAdmin");
                }
                else
                {
                    
                    throw new Exception("Failed to create admin: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}