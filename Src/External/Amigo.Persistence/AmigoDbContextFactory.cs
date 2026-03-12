using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Amigo.Persistence;

public class AmigoDbContextFactory : IDesignTimeDbContextFactory<AmigoDbContext>
{

    public AmigoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AmigoDbContext>();

        //IConfigurationRoot configuration = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddUserSecrets<AmigoDbContextFactory>()
        //        .AddEnvironmentVariables() // fallback
        //        .Build();

        //var connectionString = configuration.GetConnectionString("DefaultConnection");

        //if (string.IsNullOrEmpty(connectionString))
        //{
        //    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //}
        //2️⃣ Get the connection string
        var connectionString = "Host=ep-dark-sound-a4d200n1-pooler.us-east-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_ja9N7nKldrXo; SSL Mode=VerifyFull; Channel Binding=Require;";
        optionsBuilder.UseNpgsql(connectionString);

        return new AmigoDbContext(optionsBuilder.Options);
    }
}
