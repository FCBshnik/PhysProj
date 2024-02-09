using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Authors;
using Phys.Lib.Admin.Api.Api.Health;
using Phys.Lib.Admin.Api.Api.User;
using Phys.Lib.Admin.Api.Api.Works;
using Phys.Lib.Admin.Api.Api.Config;
using Phys.Lib.Admin.Api.Api.Files;
using Phys.Lib.Admin.Api.Filters;
using Phys.Lib.Admin.Api.Api.Migrations;
using Phys.NLog;
using Phys.Lib.Admin.Api.Api.Settings;
using Phys.Lib.Admin.Api.Api.Stats;
using Phys.Lib.Admin.Api.Api.System;
using Phys.Shared.Configuration;
using Phys.Shared;

namespace Phys.Lib.Admin.Api
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
            PhysAppContext.Init(loggerFactory);

            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration.AddJsonConfigFromArgs().Build();

            var elasticUrl = config.GetConnectionString("logs_elastic");
            if (elasticUrl != null)
            {
                NLogConfig.AddElastic(loggerFactory, "lib-admin", elasticUrl);
                PhysAppContext.HttpObserver?.IgnoreUri(new Uri(elasticUrl));
            }

            builder.WebHost.UseUrls(config.GetConnectionStringOrThrow("urls"));
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddCors();
            builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(TokenGenerator.SignKey),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });
            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("admin", policy => policy.RequireRole("admin"));
            });

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new AdminApiModule(loggerFactory, config)));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(SwaggerConfig.Configure);

            builder.Services.Configure<JsonOptions>(o =>
            {
                o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<StatusCodeLoggingMiddlware>();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoint("health", HealthEndpoint.Map);
            app.MapEndpoint("user", UserEndpoint.Map);
            app.MapEndpoint("config", ConfigEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("authors", AuthorsEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("works", WorksEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("files", FilesEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("migrations", MigrationsEndpont.Map).RequireAuthorization();
            app.MapEndpoint("settings", SettingsEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("stats", StatsEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("system", SystemEndpoint.Map).RequireAuthorization();

            app.Lifetime.ApplicationStarted.Register(() => log.LogInformation("{event} at {urls}", "start", string.Join(";", app.Urls)));
            app.Lifetime.ApplicationStopped.Register(() => log.LogInformation("{event}", "stop"));

            app.Run();
        }
    }
}
