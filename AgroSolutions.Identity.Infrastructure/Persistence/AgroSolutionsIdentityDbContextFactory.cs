using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AgroSolutions.Identity.Infrastructure.Persistence;

public class AgroSolutionsIdentityDbContextFactory : IDesignTimeDbContextFactory<AgroSolutionsIdentityDbContext>
{
    public AgroSolutionsIdentityDbContext CreateDbContext(string[] args)
    {
        string basePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "AgroSolutions.Identity.API"
        );

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        DbContextOptions<AgroSolutionsIdentityDbContext> options = new DbContextOptionsBuilder<AgroSolutionsIdentityDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AgroSolutionsIdentityDbContext(options);
    }
}
