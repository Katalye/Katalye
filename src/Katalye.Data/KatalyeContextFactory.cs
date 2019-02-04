using System.IO;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Katalye.Data
{
    [UsedImplicitly]
    public class KatalyeContextFactory : IDesignTimeDbContextFactory<KatalyeContext>
    {
        public KatalyeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KatalyeContext>();

            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

            var connectionString = configuration.GetConnectionString(nameof(KatalyeContext));
            optionsBuilder.UseNpgsql(connectionString);

            return new KatalyeContext(optionsBuilder.Options);
        }
    }
}