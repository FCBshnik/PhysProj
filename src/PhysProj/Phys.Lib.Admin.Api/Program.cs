using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Validation;
using Phys.Lib.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Authors;
using Phys.Lib.Admin.Api.Api.Health;
using Phys.Lib.Admin.Api.Api.User;
using Phys.Lib.Admin.Api.Api.Works;
using Phys.Lib.Admin.Api.OpenApi;
using Phys.Lib.Admin.Api.Api.Config;
using Phys.Lib.Admin.Api.Api.Files;
using Phys.Lib.Files.Local;
using Phys.Lib.Base.Files;

namespace Phys.Lib.Admin.Api
{
    public static class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        internal static Lazy<string> version = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty);
        internal static string Version => version.Value;

        public static void Main(string[] args)
        {
            ConsoleUtils.OnRun();

            var builder = WebApplication.CreateBuilder(args);

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

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Method != "GET")
                    log.Info($"req [{ctx.Request.Method}] {ctx.Request.Path}");
                await next();
                if (ctx.Response.StatusCode != (int)System.Net.HttpStatusCode.OK)
                    log.Info($"res [{ctx.Response.StatusCode}] to [{ctx.Request.Method}] {ctx.Request.Path}");
            });

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndpoint("health", HealthEndpoint.Map);
            app.MapEndpoint("user", UserEndpoint.Map);
            app.MapEndpoint("config", ConfigEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("authors", AuthorsEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("works", WorksEndpoint.Map).RequireAuthorization();
            app.MapEndpoint("files", FilesEndpoint.Map);//.RequireAuthorization();

            app.Lifetime.ApplicationStarted.Register(() => log.Info($"api started at {string.Join(";", app.Urls)}"));
            app.Lifetime.ApplicationStopped.Register(() => log.Info($"api stopped"));

            app.Run();
        }

        private static void ConfigureContainer(ContainerBuilder b, string mongoUrl)
        {
            b.RegisterModule(new DbModule(mongoUrl));
            b.RegisterModule(new CoreModule());
            b.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));

            b.Register(c => new SystemFileStorage("local", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files")))
                .As<IFileStorage>()
                .SingleInstance();

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
    }
}
