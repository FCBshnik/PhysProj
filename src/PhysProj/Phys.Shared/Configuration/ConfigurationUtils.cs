using Microsoft.Extensions.Configuration;
using Phys.Shared.Utils;

namespace Phys.Shared.Configuration
{
    public static class ConfigurationUtils
    {
        public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
        {
            return configuration.GetConnectionString(name) ?? throw new PhysException($"connection string '{name}' not found in configuration");
        }

        /// <summary>
        /// Adds json config from path specified by <paramref name="argName"/> command line argument if such argument is present
        /// </summary>
        public static IConfigurationBuilder AddJsonConfigFromArgs(this IConfigurationBuilder builder, string argName = "appsettings", string[]? args = null)
        {
            // skip first argument which is name of executable file
            args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
            var argsDic = CliArgsUtils.Parse(args);
            if (argsDic.ContainsKey(argName))
                builder = builder.AddJsonFile(argsDic[argName]!, optional: false);

            return builder;
        }
    }
}
