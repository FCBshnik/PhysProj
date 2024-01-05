using NSwag.CodeGeneration;
using NSwag;

namespace Phys.Lib.Cli.ApiClient
{
    internal class NSwagTypeScriptClientCommand : ICommand<NSwagTypeScriptClientOptions>
    {
        public void Run(NSwagTypeScriptClientOptions options)
        {
            var curDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = new DirectoryInfo(Path.Combine(curDir, "../../../../Clients/TypeScript/"));
            outDir.Create();

            var clientName = options.Name;
            var className = $"{clientName}ApiClient";
            using var http = new HttpClient();
            var document = OpenApiDocument.FromJsonAsync(http.GetStringAsync(options.SwaggerJsonUrl).Result).Result;
            var settings = new NSwag.CodeGeneration.TypeScript.TypeScriptClientGeneratorSettings
            {
                ClassName = className,
                UseTransformResultMethod = true,
                CodeGeneratorSettings =
                {
                },
                TypeScriptGeneratorSettings =
                {
                }
            };

            var outFile = new FileInfo(Path.Combine(outDir.FullName, $"{className}.ts"));
            var outFile2 = new FileInfo(Path.Combine(new DirectoryInfo(options.OutDirectory).FullName, $"{className}.ts"));
            var generator = new NSwag.CodeGeneration.TypeScript.TypeScriptClientGenerator(document, settings);
            var clientCode = generator.GenerateFile(ClientGeneratorOutputType.Full);
            File.WriteAllText(outFile.FullName, clientCode);
            File.WriteAllText(outFile2.FullName, clientCode);
            Console.WriteLine($"generated at '{outFile.FullName}'");
            Console.WriteLine($"generated at '{outFile2.FullName}'");
        }
    }
}
