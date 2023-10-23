using Microsoft.Extensions.Configuration;
using Phys.Shared;

namespace Phys.Lib.Core
{
    public static class Configuration
    {
        public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder builder)
        {
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.lib-development.json"), optional: true);
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.tests.json"), optional: true);
            return builder;
        }

        public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
        {
            return configuration.GetConnectionString(name) ?? throw new PhysException($"connection string '{name}' not found in configuration");
        }
    }
}
