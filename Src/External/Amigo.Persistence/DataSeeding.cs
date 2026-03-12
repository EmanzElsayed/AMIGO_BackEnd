using Amigo.Domain.Abstraction;
using Amigo.Domain.Enum;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence
{
    public class DataSeeding(RoleManager<IdentityRole> _roleManager,
                                         UserManager<ApplicationUser> _userManager,
                                         IConfiguration _configuration,
                                         
                                         AmigoDbContext _dbIdentityContext)
       : IDataSeeding
    {
        public async Task IdentityDataSeedAsync()
        {
            try
            {
                if ((await _dbIdentityContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _dbIdentityContext.Database.MigrateAsync();
                }
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    await _roleManager.CreateAsync(new IdentityRole("VIP"));

                }
                if (!_userManager.Users.Any())
                {
                    var admin = new ApplicationUser()
                    {
                        FullName = "Emera.AI Company",
                        Email = _configuration["AdminInfo:Email"],
                        UserName = "Emera.AI",
                        EmailConfirmed = true,
                        PhoneNumber = "+201111111111",
                        PhoneNumberConfirmed = true,
                        Nationality = "Egyptian",
                        Gender = Gender.Female,
                        Language = Language.English,
                        Address = new Address{
                            BuildingNumber = 1,
                            City = "Ismailia",
                            Country = "Egypt"
                        }


                    };
                    var result = await _userManager.CreateAsync(admin, _configuration["AdminInfo:Password"]);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
