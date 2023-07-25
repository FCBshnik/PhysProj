using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace Phys.Lib.Admin.Api.OpenApi
{
    public class KebabCaseEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                schema.Type = "string";
                schema.Format = null;
                schema.Nullable = false;
                foreach (var name in Enum.GetNames(context.Type))
                    schema.Enum.Add(new OpenApiString(JsonNamingPolicy.KebabCaseLower.ConvertName(name)));
            }
        }
    }
}
