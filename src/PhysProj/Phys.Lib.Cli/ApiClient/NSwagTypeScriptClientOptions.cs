using CommandLine;

namespace Phys.Lib.Cli.ApiClient
{
    [Verb("nswag-typescript-client")]
    internal class NSwagTypeScriptClientOptions
    {
        [Option("name", Required = true)]
        public required string Name { get; set; }

        [Option("swagger-json-url", Required = true)]
        public required string SwaggerJsonUrl { get; set; }

        [Option("out-directory", Required = true)]
        public required string OutDirectory { get; set; }
    }
}
