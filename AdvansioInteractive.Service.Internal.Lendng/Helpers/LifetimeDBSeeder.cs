using AdvansioInteractive.Service.Internal.Lendng.Models;
using Microsoft.AspNetCore.Identity;

namespace AdvansioInteractive.Service.Internal.Lendng.Helpers
{
    public static class LifetimeDBSeeder
    {
        public static async Task SeedMongoIdentityDatabaseRoles(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleNames = new string[] { "USER", "MODERATOR", "ADMINISTRATOR" };
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                foreach (var role in roleNames)
                {
                    var isExists = await roleManager.RoleExistsAsync(role);
                    if (!isExists)
                    {
                        await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    }
                }
            }
        }
    }
}
