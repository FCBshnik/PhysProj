using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.NLog;
using Phys.Utils;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Phys.Lib.App
{
    internal static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

        static void Main(string[] args)
        {
            NLogConfig.Configure(loggerFactory);
            AppUtils.OnRun(loggerFactory);

            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureAppConfiguration(c => AppUtils.AddJsonConfigFromArgs(c));

            builder.ConfigureLogging((c, b) =>
            {
                var elasticUrl = c.Configuration.GetConnectionString("logs_elastic");
                if (elasticUrl != null)
                    NLogConfig.AddElastic(loggerFactory, "lib-app", elasticUrl);
            });

            builder.UseServiceProviderFactory(ctx => new AutofacServiceProviderFactory(c => c.RegisterModule(new AppModule(loggerFactory, ctx.Configuration))));
            using var host = builder.Build();
            host.Run();
        }
    }
}