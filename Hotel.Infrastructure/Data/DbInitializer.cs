using Hotel.Application.Common.Intilizer;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Data
{
    public class DbInitializer(ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) : IDbInitializer
    {
        public void Initialize()
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                    context.Database.Migrate();
                if (!roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
                    userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = "mohamed",
                        Email = "mo.zonkol@gmail.com",
                        Name = "mohamed",
                        PhoneNumber = "01029054588",
                        NormalizedEmail = "MO.ZONKOL@GMAIL.COM",
                        NormalizedUserName = "MOHAMED",
                    }, "Mo@123").GetAwaiter().GetResult();
                    var user = context.ApplicationUsers.FirstOrDefault(x => x.Email == "mo.zonkol@gmail.com");
                    userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
