using Amigo.Domain.Abstraction;
using Amigo.Domain.Enum;

namespace Amigo.Persistence;

public class DataSeeding(RoleManager<IdentityRole> _roleManager,
                        UserManager<ApplicationUser> _userManager,
                        IConfiguration _configuration,
                        AmigoDbContext _dbIdentityContext,
                        IUnitOfWork _unitOfWork)
      : IDataSeeding
{
    public async Task IdentityDataSeedAsync()
    {
        bool needSave = false;
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
                    Language.en,
                    new Address
                    {
                        BuildingNumber = "25",
                        City = "Ismailia",
                        Country = "EG"
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
            if (!_dbIdentityContext.Currency.Any())
            {
                await AddCurrency();
                needSave = true;
            }
            if (!_dbIdentityContext.CountryInfo.Any())
            {
                await AddCountryInfo();
                needSave = true;
            }
            if (needSave)
            { 
                await _unitOfWork.SaveChangesAsync();
            }

        }
        catch (Exception)
        {
            throw;
        }
    }


    private async Task AddCountryInfo()
    {
        var countries = new List<CountryInfo>
        {
            // EG
            new CountryInfo
            {
                Id = Guid.NewGuid(),
                CountryCode = CountryCode.EG,
                PhoneCode = "+20",
                Translations = new List<CountryInfoTranslation>
                {
                    new() { Id = Guid.NewGuid(), Language = Language.en, Name = "Egypt" },
                    new() { Id = Guid.NewGuid(), Language = Language.es, Name = "Egipto" },
                    new() { Id = Guid.NewGuid(), Language = Language.fr, Name = "Égypte" },
                    new() { Id = Guid.NewGuid(), Language = Language.it, Name = "Egitto" },
                    new() { Id = Guid.NewGuid(), Language = Language.pt, Name = "Egito" },
                    new() { Id = Guid.NewGuid(), Language = Language.br, Name = "Egito" }
                }
            },

            // UAE
            new CountryInfo
            {
                Id = Guid.NewGuid(),
                CountryCode = CountryCode.UAE,
                PhoneCode = "+971",
                Translations = new List<CountryInfoTranslation>
                {
                    new() { Id = Guid.NewGuid(), Language = Language.en, Name = "United Arab Emirates" },
                    new() { Id = Guid.NewGuid(), Language = Language.es, Name = "Emiratos Árabes Unidos" },
                    new() { Id = Guid.NewGuid(), Language = Language.fr, Name = "Émirats arabes unis" },
                    new() { Id = Guid.NewGuid(), Language = Language.it, Name = "Emirati Arabi Uniti" },
                    new() { Id = Guid.NewGuid(), Language = Language.pt, Name = "Emirados Árabes Unidos" },
                    new() { Id = Guid.NewGuid(), Language = Language.br, Name = "Emirados Árabes Unidos" }
                }
            },

            // TR
            new CountryInfo
            {
                Id = Guid.NewGuid(),
                CountryCode = CountryCode.TR,
                PhoneCode = "+90",
                Translations = new List<CountryInfoTranslation>
                {
                    new() { Id = Guid.NewGuid(), Language = Language.en, Name = "Turkey" },
                    new() { Id = Guid.NewGuid(), Language = Language.es, Name = "Turquía" },
                    new() { Id = Guid.NewGuid(), Language = Language.fr, Name = "Turquie" },
                    new() { Id = Guid.NewGuid(), Language = Language.it, Name = "Turchia" },
                    new() { Id = Guid.NewGuid(), Language = Language.pt, Name = "Turquia" },
                    new() { Id = Guid.NewGuid(), Language = Language.br, Name = "Turquia" }
                }
            },

            // Saudi Arabia
            new CountryInfo
            {
                Id = Guid.NewGuid(),
                CountryCode = CountryCode.KSA,
                PhoneCode = "+966",
                Translations = new List<CountryInfoTranslation>
                {
                    new() { Id = Guid.NewGuid(), Language = Language.en, Name = "Saudi Arabia" },
                    new() { Id = Guid.NewGuid(), Language = Language.es, Name = "Arabia Saudita" },
                    new() { Id = Guid.NewGuid(), Language = Language.fr, Name = "Arabie saoudite" },
                    new() { Id = Guid.NewGuid(), Language = Language.it, Name = "Arabia Saudita" },
                    new() { Id = Guid.NewGuid(), Language = Language.pt, Name = "Arábia Saudita" },
                    new() { Id = Guid.NewGuid(), Language = Language.br, Name = "Arábia Saudita" }
                }
            }
        };

        await _unitOfWork.GetRepository<CountryInfo, Guid>().AddRangeAsync(countries);

    }

    private async Task AddCurrency()
    {

        var poundStr = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.GBP,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216202/pound_pyfhhn.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Pound sterling",
                        Language = Language.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sterlina",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Livre sterling",
                        Language = Language.fr
                    },
                      
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina",
                        Language = Language.pt
                    },
                      
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina britânica",
                        Language = Language.br
                    },
            }
        };

        var Euro = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.Euro,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216203/euro_ngyshv.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Euro",
                        Language = Language.br
                    },
            }
        };
        var brazillianReal = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.BRL,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216202/brazilian-real_e0dcby.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Brazilian real",
                        Language = Language.en
                    },  
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasiliano",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileño",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Réal brésilien",
                        Language = Language.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileiro",
                        Language = Language.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileiro",
                        Language = Language.br
                    },
            }
        };

        var peruvianSol = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.PEN,
            CodeIcon = "S/",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peruvian sol",
                        Language = Language.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruviano",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol péruvien",
                        Language = Language.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = Language.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = Language.br
                    },
            }
        };

        var argentinePeso = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.ARS,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216201/argentina-money_daz5jk.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Argentine peso",
                        Language = Language.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentin",
                        Language = Language.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = Language.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = Language.br
                    },
            }
        };
        var colombianPeso = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.COP,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216837/peso_1_gehnq9.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Colombian peso",
                        Language = Language.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombiano",
                        Language = Language.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombiano",
                        Language = Language.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombien",
                        Language = Language.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso Colombiano",
                        Language = Language.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso Colombiano",
                        Language = Language.br
                    },
            }
        };


        var mexicanPeso = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.MXN,
            CodeIcon = "MXN",
            Translations =
    {
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Mexican peso",
            Language = Language.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicano",
            Language = Language.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicano",
            Language = Language.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicain",
            Language = Language.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Mexicano",
            Language = Language.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Mexicano",
            Language = Language.br
        },
    }
        };

        var unitedStatesDollar = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.USD,
            CodeIcon = "US",
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776215325/dollar_zjaa5h.png",
            Translations =
    {
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "United States dollar",
            Language = Language.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dollaro statunitense",
            Language = Language.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar estadounidense",
            Language = Language.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dollar américain",
            Language = Language.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar dos Estados Unidos",
            Language = Language.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar dos Estados Unidos",
            Language = Language.br
        },
    }
        };

        var chileanPeso = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.CLP,
            CodeIcon = "CL",
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776215325/dollar_zjaa5h.png",
            Translations =
    {
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Chilean peso",
            Language = Language.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso cileno",
            Language = Language.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso chileno",
            Language = Language.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso chilien",
            Language = Language.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Chileno",
            Language = Language.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Chileno",
            Language = Language.br
        },
    }
        };

        List<Currency> currencies = new List<Currency>();

        currencies.Add(chileanPeso);
        currencies.Add(poundStr);
        currencies.Add(Euro);
        currencies.Add(brazillianReal);
        currencies.Add(peruvianSol);
        currencies.Add(argentinePeso);
        currencies.Add(colombianPeso);
        currencies.Add(mexicanPeso);
        currencies.Add(unitedStatesDollar);

        await _unitOfWork.GetRepository<Currency, Guid>().AddRangeAsync(currencies);
    }

}