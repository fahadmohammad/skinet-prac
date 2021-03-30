using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public enum AppRoles
        {
            Administrator,
            Moderator,
            User
        }
        public static async Task SeedUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Administrator.ToString()));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Moderator.ToString()));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.User.ToString()));
            }

            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Bob Marley",
                    Email = "bob@marley.com",
                    UserName = "bob@marley.com",

                    Address = new Address
                    {
                        FirstName = "Bob",
                        LastName = "Marley",
                        City = "New York",
                        Street = "27 Park View",
                        State = "NY",
                        ZipCode = "2020"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, AppRoles.Administrator.ToString());

            }
        }
    }
}