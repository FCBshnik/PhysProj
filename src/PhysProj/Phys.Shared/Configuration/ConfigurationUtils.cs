using Microsoft.Extensions.Configuration;

namespace Phys.Shared.Configuration
{
    public static class ConfigurationUtils
    {
        public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
        {
            return configuration.GetConnectionString(name) ?? throw new PhysException($"connection string '{name}' not found in configuration");
        }
    }
}
