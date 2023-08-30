using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Phys.Lib.Admin.Api.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Phys.Lib.Admin.Api
{
    internal static class SwaggerConfig
    {
        public static void Configure(SwaggerGenOptions o)
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
