using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
using Swashbuckle.AspNetCore.SwaggerGen;
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

            var config = builder.Configuration;
            var urls = config.GetConnectionString("urls") ?? throw new ApplicationException();
            var mongoUrl = config.GetConnectionString("mongo") ?? throw new ApplicationException();

            builder.WebHost.UseUrls(urls);
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
            builder.Host.ConfigureContainer((ContainerBuilder b) => ConfigureContainer(b, mongoUrl));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(ConfigureSwagger);

            builder.Services.Configure<JsonOptions>(o =>
            {
                o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoint("api/auth", AuthEndpoint.Map);
            app.MapEndpoint("api/health", HealthEndpoint.Map);

            app.Lifetime.ApplicationStarted.Register(() => log.Info($"api started at {string.Join(";", app.Urls)}"));
            app.Lifetime.ApplicationStopped.Register(() => log.Info($"api stopped"));

            app.Run();
        }

        private static void ConfigureContainer(ContainerBuilder b, string mongoUrl)
        {
            b.RegisterModule(new DbModule(mongoUrl));
            b.RegisterModule(new CoreModule());
            b.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));

            b.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            b.RegisterType<UserResolver>().InstancePerDependency();
            b.Register(c => c.Resolve<UserResolver>().GetUser()).InstancePerDependency();
        }

        private static void ConfigureSwagger(SwaggerGenOptions o)
        {
            o.SchemaFilter<KebabCaseEnumSchemaFilter>();
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[]{}
                    }
                });
        }

        private class Options
        {
            [Option("appsettings", Required = false)]
            public string? AppSettingsFile { get; set; }
        }
    }
}