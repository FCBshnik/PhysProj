using Microsoft.Extensions.Configuration;

namespace Phys.Lib.Tests.Integration
{
    internal static class ConfigurationFactory
    {
        public static IConfiguration CreateTestConfiguration()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:db", "mongo" }
                })
                .Build();
        }
    }
}
