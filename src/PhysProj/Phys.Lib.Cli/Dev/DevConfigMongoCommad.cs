using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Shared.Mongo.Configuration;

namespace Phys.Lib.Cli.Dev
{
    internal class DevConfigMongoCommad : ICommand<DevConfigMongoOptions>
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfiguration configuration;

        public DevConfigMongoCommad(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
        }

        public void Run(DevConfigMongoOptions options)
        {
            var provider = new MongoConfigurationProvider(configuration.GetConnectionString("config-mongo"), loggerFactory.CreateLogger<MongoConfigurationProvider>());
            var json = File.ReadAllText("dev-config.json");
            provider.SaveIfNotExists(json);
        }
    }
}
