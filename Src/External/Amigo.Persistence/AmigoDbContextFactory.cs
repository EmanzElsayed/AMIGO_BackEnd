using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Amigo.Persistence;

public class AmigoDbContextFactory : IDesignTimeDbContextFactory<AmigoDbContext>
{
    public AmigoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AmigoDbContext>();

        var connectionString =
           "Host=ep-proud-wind-adnbu4xz-pooler.c-2.us-east-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_ndD2RI6wNayg; SSL Mode=VerifyFull; Channel Binding=Require;";

        optionsBuilder.UseNpgsql(connectionString);

        return new AmigoDbContext(optionsBuilder.Options);
    }
}
