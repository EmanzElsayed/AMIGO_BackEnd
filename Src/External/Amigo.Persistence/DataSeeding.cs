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
                    var adminEmail = _configuration["AdminInfo:Email"] ?? string.Empty;
                    var adminPassword = _configuration["AdminInfo:Password"];

                    if (string.IsNullOrWhiteSpace(adminPassword))
                    {
                        throw new InvalidOperationException("AdminInfo:Password is not configured in appsettings.");
                    }

                    var admin = new ApplicationUser(
                        adminEmail,
                        "Emera.AI Company",
                        new DateOnly(1990, 1, 1),
                        "+201111111111",
                        Gender.Female,
                        Language.English,
                        new Address
                        {
                            BuildingNumber = 1,
                            City = "Ismailia",
                            Country = "Egypt"
                        },
                        "Emera.AI",
                        "Egyptian"
                    )
                    {
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(admin, adminPassword);
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
