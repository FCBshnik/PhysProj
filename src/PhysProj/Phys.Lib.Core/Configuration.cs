using Microsoft.Extensions.Configuration;
using Phys.Shared;

namespace Phys.Lib.Core
{
    public static class Configuration
    {
        public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
        {
            return configuration.GetConnectionString(name) ?? throw new PhysException($"connection string '{name}' not found in configuration");
        }
    }
}
