using Phys.Lib.Core;

namespace Phys.Lib.Admin.Api.Api.Config
{
    public static class ConfigEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("langs", () => {
                var langs = Language.All.Select(l => new LanguageModel { Code = l.Code, Name = l.Name });
                return Results.Ok(langs);
            })
            .ProducesResponse<List<LanguageModel>>("ListLanguages");
        }
    }
}
