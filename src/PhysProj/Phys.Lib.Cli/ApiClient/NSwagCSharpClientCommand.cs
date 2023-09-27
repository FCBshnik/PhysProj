using NSwag;
using NSwag.CodeGeneration;

namespace Phys.Lib.Cli.ApiClient
{
    internal class NSwagCSharpClientCommand : ICommand<NSwagCSharpClientOptions>
    {
        public void Run(NSwagCSharpClientOptions options)
        {
            var curDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = new DirectoryInfo(Path.Combine(curDir, "../../../../Clients/CSharp/"));
            outDir.Create();

            var clientName = options.Name;
            var className = $"{clientName}ApiClient";
            using var http = new HttpClient();
            var document = OpenApiDocument.FromJsonAsync(http.GetStringAsync(options.SwaggerJsonUrl).Result).Result;
            var settings = new NSwag.CodeGeneration.CSharp.CSharpClientGeneratorSettings
            {
                ClassName = className,
                GenerateOptionalParameters = false,
                CSharpGeneratorSettings =
                {
                    Namespace = $"Phys.Lib.{clientName}.Client",
                }
            };

            var outFile = new FileInfo(Path.Combine(outDir.FullName, $"{className}.cs"));
            var generator = new NSwag.CodeGeneration.CSharp.CSharpClientGenerator(document, settings);
            var clientCode = generator.GenerateFile(ClientGeneratorOutputType.Full);
            File.WriteAllText(outFile.FullName, clientCode);
            Console.WriteLine($"generated at '{outFile.FullName}'");
        }
    }
}
