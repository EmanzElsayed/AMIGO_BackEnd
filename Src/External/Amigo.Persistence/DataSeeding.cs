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
                await _roleManager.CreateAsync(new IdentityRole("Public"));
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
                    SupportedLanguage.en,
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
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.en, Name = "Egypt",Capital = "Cairo" , OfficialLanguage = "Arabic" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.es, Name = "Egipto" ,Capital = "El Cairo" , OfficialLanguage = "Árabe" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.fr, Name = "Égypte" ,Capital = "Le Caire" , OfficialLanguage = "Arabe" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.it, Name = "Egitto" ,Capital = "Il Cairo" , OfficialLanguage = "Arabo" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.pt, Name = "Egito" ,Capital = "Cairo" , OfficialLanguage = "Árabe" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.br, Name = "Egito" ,Capital = "Cairo" , OfficialLanguage = "Árabe" }
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
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.en, Name = "United Arab Emirates"   ,Capital = "Abu Dhabi" , OfficialLanguage = "Arabic" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.es, Name = "Emiratos Árabes Unidos" ,Capital = "Abu Dabi" , OfficialLanguage = "Árabe"   },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.fr, Name = "Émirats arabes unis"    ,Capital = "Abou Dabi" , OfficialLanguage = "Arabe"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.it, Name = "Emirati Arabi Uniti"    ,Capital = "Abu Dhabi" , OfficialLanguage = "Arabo"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.pt, Name = "Emirados Árabes Unidos" ,Capital = "Abu Dhabi" , OfficialLanguage = "Árabe"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.br, Name = "Emirados Árabes Unidos" ,Capital = "Abu Dhabi" , OfficialLanguage = "Árabe"  }
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
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.en, Name = "Turkey"  ,Capital = "Ankara" , OfficialLanguage = "Turkish"   },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.es, Name = "Turquía" ,Capital = "Ankara" , OfficialLanguage = "Turco"     },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.fr, Name = "Turquie" ,Capital = "Ankara" , OfficialLanguage = "Turc"    },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.it, Name = "Turchia" ,Capital = "Ankara" , OfficialLanguage = "Turco"     },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.pt, Name = "Turquia" ,Capital = "Ancara" , OfficialLanguage = "Turco"    },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.br, Name = "Turquia" ,Capital = "Ancara" , OfficialLanguage = "Turco"    }
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
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.en, Name = "Saudi Arabia"    ,Capital = "Riyadh" , OfficialLanguage = "Arabic" },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.es, Name = "Arabia Saudita"  ,Capital = "Riad" , OfficialLanguage = "Árabe"   },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.fr, Name = "Arabie saoudite" ,Capital = "Riyad" , OfficialLanguage = "Arabe"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.it, Name = "Arabia Saudita"  ,Capital = "Riad" , OfficialLanguage = "Arabo"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.pt, Name = "Arábia Saudita"  ,Capital = "Riade" , OfficialLanguage = "Árabe"  },
                    new() { Id = Guid.NewGuid(), Language = SupportedLanguage.br, Name = "Arábia Saudita"  ,Capital = "Riade" , OfficialLanguage = "Árabe"  }
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
                        Language = SupportedLanguage.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sterlina",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Livre sterling",
                        Language = SupportedLanguage.fr
                    },
                      
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina",
                        Language = SupportedLanguage.pt
                    },
                      
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Libra esterlina britânica",
                        Language = SupportedLanguage.br
                    },
            }
        };

        var Euro = new Currency
        {
            Id = Guid.NewGuid(),
            CurrencyCode = CurrencyCode.EUR,
            Icon = "https://res.cloudinary.com/dxxiuvnko/image/upload/v1776216203/euro_ngyshv.png",
            Translations =
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "EUR",
                        Language = SupportedLanguage.br
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
                        Language = SupportedLanguage.en
                    },  
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasiliano",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileño",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Réal brésilien",
                        Language = SupportedLanguage.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileiro",
                        Language = SupportedLanguage.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Real brasileiro",
                        Language = SupportedLanguage.br
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
                        Language = SupportedLanguage.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruviano",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol péruvien",
                        Language = SupportedLanguage.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = SupportedLanguage.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sol peruano",
                        Language = SupportedLanguage.br
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
                        Language = SupportedLanguage.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentin",
                        Language = SupportedLanguage.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = SupportedLanguage.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso argentino",
                        Language = SupportedLanguage.br
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
                        Language = SupportedLanguage.en
                    },
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombiano",
                        Language = SupportedLanguage.it
                    },
                     new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombiano",
                        Language = SupportedLanguage.es
                    },
                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso colombien",
                        Language = SupportedLanguage.fr
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso Colombiano",
                        Language = SupportedLanguage.pt
                    },

                      new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peso Colombiano",
                        Language = SupportedLanguage.br
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
            Language = SupportedLanguage.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicano",
            Language = SupportedLanguage.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicano",
            Language = SupportedLanguage.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso mexicain",
            Language = SupportedLanguage.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Mexicano",
            Language = SupportedLanguage.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Mexicano",
            Language = SupportedLanguage.br
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
            Language = SupportedLanguage.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dollaro statunitense",
            Language = SupportedLanguage.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar estadounidense",
            Language = SupportedLanguage.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dollar américain",
            Language = SupportedLanguage.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar dos Estados Unidos",
            Language = SupportedLanguage.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Dólar dos Estados Unidos",
            Language = SupportedLanguage.br
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
            Language = SupportedLanguage.en
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso cileno",
            Language = SupportedLanguage.it
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso chileno",
            Language = SupportedLanguage.es
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso chilien",
            Language = SupportedLanguage.fr
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Chileno",
            Language = SupportedLanguage.pt
        },
        new CurrencyTranslation
        {
            Id = Guid.NewGuid(),
            Name = "Peso Chileno",
            Language = SupportedLanguage.br
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