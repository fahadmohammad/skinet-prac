using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
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

                await userManager.CreateAsync(user,"Pa$$w0rd");

            }
        }
    }
}