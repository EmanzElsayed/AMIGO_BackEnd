using System.CommandLine;

namespace Amigo.Persistence;

public class AmigoDbContextFactory : IDesignTimeDbContextFactory<AmigoDbContext>
{

    public AmigoDbContext CreateDbContext(string[] args)
    {
        var connectionOption = new Option<string>
        (
            name: "--connection",
            description: "The connection string to use for the database"
        );

        var rootCommand = new RootCommand { connectionOption };

        var result = rootCommand.Parse(args);
        var connectionString = result.GetValueForOption(connectionOption);

        if (string.IsNullOrEmpty(connectionString))
        {
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../api/Amigo.API"));

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            connectionString = config.GetConnectionString("DefaultConnection");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("No connection string provided.");

        var optionsBuilder = new DbContextOptionsBuilder<AmigoDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AmigoDbContext(optionsBuilder.Options);
    }
}
