using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ericsson.ReCapProject.Persistence.DbContexts.Factories
{
    public class ReCapProjectDbContextFactory : IDesignTimeDbContextFactory<ReCapProjectDbContext>
    {
        public ReCapProjectDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ReCapProjectDbContext>();
            string connectionString = GetConnectionStringFromKeyVault(configuration);
            optionsBuilder.UseSqlServer(connectionString);
            return new ReCapProjectDbContext(optionsBuilder.Options);
        }

        private static string GetConnectionStringFromKeyVault(IConfigurationRoot configuration)
        {
            var secretUri = configuration["AzureKeyVault:VaultUri"];
            var secretClient = new SecretClient(new Uri(secretUri), new DefaultAzureCredential());
            var secret = secretClient.GetSecret("ReCapProjectDbConnectionString");
            var connectionString = secret.Value.Value;
            return connectionString;
        }
    }
}
