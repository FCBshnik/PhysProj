
using Autofac;
using Autofac.Extensions.DependencyInjection;
using NLog.Web;
using Phys.Lib.Api.Admin.Api.Auth;
using Phys.Lib.Api.Admin.Filters;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Validation;
using Phys.Lib.Data;
using System.Reflection;

namespace Phys.Lib.Api.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsoleUtils.OnRun();

            var builder = WebApplication.CreateBuilder(args);
            
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new DbModule());
                builder.RegisterModule(new CoreModule());
                builder.RegisterModule(new ValidationModule(Assembly.GetExecutingAssembly()));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            AuthEndpoint.Map(app.MapGroup("api/auth").AddEndpointFilter<ValidationErrorFilter>());

            app.Run();
        }
    }
}