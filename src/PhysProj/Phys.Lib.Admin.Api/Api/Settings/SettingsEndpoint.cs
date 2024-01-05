using Microsoft.AspNetCore.Mvc;
using Phys.Shared;
using Phys.Shared.Settings;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Phys.Lib.Admin.Api.Api.Settings
{
    public static class SettingsEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([FromServices] ISettings settings) =>
            {
                return settings.List();
            }).ProducesResponse<List<string>>("ListSettings");

            builder.MapGet("/{code}", (string code, [FromServices] ISettings settings) =>
            {
                return settings.Get(code);
            }).ProducesResponse<object>("GetSettings");

            builder.MapPost("/{code}", (string code, [FromBody]JsonValue value, [FromServices] ISettings settings) =>
            {
                var type = settings.GetType(code);
                var obj = value.Deserialize(type);
                settings.Set(code, obj ?? throw new PhysException());
                return obj;
            }).ProducesResponse<object>("UpdateSettings");
        }
    }
}
