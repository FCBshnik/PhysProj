using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Phys.Mongo.Configuration
{
    public class MongoConfigurationSource : IConfigurationSource
    {
        private string connectionString;
        private readonly ILoggerFactory loggerFactory;

        public MongoConfigurationSource(string connectionString, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            this.connectionString = connectionString;
            this.loggerFactory = loggerFactory;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MongoConfigurationProvider(connectionString, loggerFactory.CreateLogger<MongoConfigurationProvider>());
        }
    }
}