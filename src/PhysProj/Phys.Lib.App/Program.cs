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
            NLogConfig.Configure(loggerFactory, "lib-app");
            ProgramUtils.OnRun(loggerFactory);

            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureAppConfiguration((ctx, c) =>
            {
                var envSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.lib-{ctx.HostingEnvironment.EnvironmentName.ToLowerInvariant()}.json");
                c.AddJsonFile(envSettingsPath, optional: true);
            });

            builder.UseServiceProviderFactory(ctx => new AutofacServiceProviderFactory(c => c.RegisterModule(new AppModule(loggerFactory, ctx.Configuration))));
            using var host = builder.Build();
            host.Run();
        }
    }
}