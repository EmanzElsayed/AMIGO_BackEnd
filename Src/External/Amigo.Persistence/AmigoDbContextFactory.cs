
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Amigo.Persistence;

public class AmigoDbContextFactory : IDesignTimeDbContextFactory<AmigoDbContext>
{
    public AmigoDbContext CreateDbContext(string[] args)
    {
        // Try to get environment
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        // Build path to API project (where appsettings exists)
        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../../Api/Amigo.API")
        );

        // Load configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("No connection string provided.");

        var optionsBuilder = new DbContextOptionsBuilder<AmigoDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AmigoDbContext(optionsBuilder.Options);
    }
}
