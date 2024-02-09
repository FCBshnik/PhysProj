using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.NLog;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Phys.Shared;
using Phys.Shared.Configuration;

namespace Phys.Lib.App
{
    internal static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

        static void Main(string[] args)
        {
            NLogConfig.Configure(loggerFactory);
            PhysAppContext.Init(loggerFactory);

            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureAppConfiguration(c => c.AddJsonConfigFromArgs());

            builder.ConfigureLogging((c, b) =>
            {
                var elasticUrl = c.Configuration.GetConnectionString("logs_elastic");
                if (elasticUrl != null)
                {
                    NLogConfig.AddElastic(loggerFactory, "lib-app", elasticUrl);
                    PhysAppContext.HttpObserver?.IgnoreUri(new Uri(elasticUrl));
                }
            });

            builder.UseServiceProviderFactory(ctx => new AutofacServiceProviderFactory(c => c.RegisterModule(new AppModule(loggerFactory, ctx.Configuration))));
            using var host = builder.Build();
            host.Run();
        }
    }
}