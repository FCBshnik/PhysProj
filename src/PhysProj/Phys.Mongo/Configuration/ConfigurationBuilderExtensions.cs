using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Phys.Mongo.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddMongo(this IConfigurationBuilder builder, ILoggerFactory loggerFactory, string connectionStringName = "config-mongo")
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            var baseConfig = builder.Build();
            var connectionString = baseConfig.GetConnectionString(connectionStringName);
            if (connectionString == null)
                throw new Exception($"connection string '{connectionStringName}' not found in base configuration");

            return builder.Add(new MongoConfigurationSource(connectionString, loggerFactory));
        }
    }
}
