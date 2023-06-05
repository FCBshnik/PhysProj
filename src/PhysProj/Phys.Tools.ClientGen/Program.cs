using NJsonSchema.CodeGeneration;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;

namespace Phys.Tools.ClientGen
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var curDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = new DirectoryInfo(Path.Combine(curDir, "../../../../Clients/CSharp/"));
            outDir.Create();

            var clientName = "Admin";
            var className = $"{clientName}ApiClient";
            using var http = new HttpClient();
            var document = OpenApiDocument.FromJsonAsync(http.GetStringAsync("https://localhost:7188/swagger/v1/swagger.json").Result).Result;
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = className,
                CSharpGeneratorSettings =
                {
                    Namespace = $"PhysLib.{clientName}.Client",
                }
            };

            var outFile = new FileInfo(Path.Combine(outDir.FullName, $"{className}.cs"));
            var generator = new CSharpClientGenerator(document, settings);
            var clientCode = generator.GenerateFile(ClientGeneratorOutputType.Full);
            File.WriteAllText(outFile.FullName, clientCode);
            Console.WriteLine($"generated at '{outFile.FullName}'");
        }
    }
}