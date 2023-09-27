using CommandLine;

namespace Phys.Lib.Cli.ApiClient
{
    [Verb("nswag-csharp-client")]
    internal class NSwagCSharpClientOptions
    {
        [Option("swagger-json-url")]
        public string SwaggerJsonUrl { get; set; } = "http://localhost:7188/swagger/v1/swagger.json";

        [Option("name")]
        public string Name { get; set; } = "Admin";
    }
}
