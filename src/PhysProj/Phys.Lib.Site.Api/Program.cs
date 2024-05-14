using Autofac;
using Autofac.Extensions.DependencyInjection;
using Phys.Lib.Site.Api.Pipeline;
using Phys.Serilog;
using Phys.Shared;
using Phys.Shared.Configuration;
using Serilog;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;
using Phys.Lib.Site.Api.Controllers;

namespace Phys.Lib.Site.Api
{
    public static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly Microsoft.Extensions.Logging.ILogger log = loggerFactory.CreateLogger(nameof(Program));

        internal static readonly Lazy<string> version = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty);
        internal static string Version => version.Value;

        public static void Main(string[] args)
        {
            SerilogConfig.Configure(loggerFactory);
            PhysAppContext.Init(loggerFactory);

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders().AddSerilog(Log.Logger);

            builder.Configuration.AddJsonConfigFromArgs();
            IConfiguration config = builder.Configuration;

            var urls = config.GetConnectionStringOrThrow("urls");
            var elasticUrl = config.GetConnectionString("logs_elastic");
            if (elasticUrl != null)
            {
                PhysAppContext.HttpObserver?.IgnoreUri(new Uri(elasticUrl));
            }

            builder.WebHost.UseUrls(urls);

            builder.Services.AddCors();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new SiteApiModule(loggerFactory, config)));

            builder.Services.AddControllers(c =>
            {
                c.Filters.Add(new InternalErrorFilter(loggerFactory.CreateLogger("api-internal-err")));
                c.Filters.Add(new ExternalErrorFilter(loggerFactory.CreateLogger("api-external-err")));
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddRateLimiter(o =>
            {
                o.OnRejected = (ctx, _) =>
                {
                    ctx.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    ctx.HttpContext.Response.WriteAsJsonAsync(new ErrorModel(ErrorCode.InvalidRequest, "Too many requests. Please try again in few seconds."));
                    return new ValueTask();
                };

                o.AddFixedWindowLimiter(policyName: "search", options =>
                {
                    options.PermitLimit = 3;
                    options.Window = TimeSpan.FromSeconds(10);
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRateLimiter();

            app.UseMiddleware<StatusCodeLoggingMiddlware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(() => log.LogInformation("{event} at {urls}", "start", string.Join(";", app.Urls)));
            app.Lifetime.ApplicationStopped.Register(() => log.LogInformation("{event}", "stop"));

            app.Run();
        }
    }
}