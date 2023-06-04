using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.AspNetCore.Http.Json;
using NLog;
using NLog.Web;
using Phys.Lib.Api.Admin.Api;
using Phys.Lib.Api.Admin.Api.Auth;
using Phys.Lib.Api.Admin.Api.Health;
using Phys.Lib.Api.Admin.OpenApi;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Validation;
using Phys.Lib.Data;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Phys.Lib.Api.Admin
{
    public class Program
    {
        private static readonly NLog.ILogger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            ConsoleUtils.OnRun();

            var options = Parser.Default.ParseArguments<Options>(args).Value;

            var builder = WebApplication.CreateBuilder(args);

            if (options.AppSettingsFile != null)
                builder.Configuration.AddJsonFile(options.AppSettingsFile, false);

            builder.WebHost.UseUrls(builder.Configuration.GetConnectionString("urls"));
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(b =>
            {
                b.RegisterModule(new DbModule(builder.Configuration.GetConnectionString("mongo")));
                b.RegisterModule(new CoreModule());
                b.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SchemaFilter<KebabCaseEnumSchemaFilter>();
            });

            builder.Services.Configure<JsonOptions>(o =>
            {
                o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapEndpoint("api/auth", AuthEndpoint.Map);
            app.MapEndpoint("api/health", HealthEndpoint.Map);

            app.Lifetime.ApplicationStarted.Register(() => log.Info($"api started at {string.Join(";", app.Urls)}"));
            app.Lifetime.ApplicationStopped.Register(() => log.Info($"api stopped"));

            app.Run();
        }

        private class Options
        {
            [Option("appsettings", Required = false)]
            public string? AppSettingsFile { get; set; }
        }
    }
}