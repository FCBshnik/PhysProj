using Autofac;
using Autofac.Extensions.DependencyInjection;
using NLog.Web;
using Phys.Lib.Core;
using Phys.Lib.Site.Api.Pipeline;
using Phys.NLog;
using Phys.Utils;
using System.Reflection;

namespace Phys.Lib.Site.Api
{
    public static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

        internal static readonly Lazy<string> version = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty);
        internal static string Version => version.Value;

        public static void Main(string[] args)
        {
            NLogConfig.Configure(loggerFactory);
            AppUtils.OnRun(loggerFactory);

            var builder = WebApplication.CreateBuilder(args);

            AppUtils.AddJsonConfigFromArgs(builder.Configuration);

            IConfiguration config = builder.Configuration;
            var urls = config.GetConnectionStringOrThrow("urls");
            var elasticUrl = config.GetConnectionString("logs_elastic");
            if (elasticUrl != null)
                NLogConfig.AddElastic(loggerFactory, "lib-site", elasticUrl);

            builder.WebHost.UseUrls(urls);

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddCors();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new ApiModule(loggerFactory, config)));

            builder.Services.AddControllers(c =>
            {
                c.Filters.Add(new InternalErrorFilter(loggerFactory.CreateLogger("api-internal-err")));
                c.Filters.Add(new ExternalErrorFilter(loggerFactory.CreateLogger("api-external-err")));
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<StatusCodeLoggingMiddlware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(() => log.LogInformation("{event} at {urls}", "start", string.Join(";", app.Urls)));
            app.Lifetime.ApplicationStopped.Register(() => log.LogInformation("{event}", "stop"));

            app.Run();
        }
    }
}