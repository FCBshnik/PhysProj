using NSwag;
using NSwag.CodeGeneration;

namespace Phys.Tools.ClientGen
{
    internal static class Program
    {
        private static readonly string adminSwaggerJsonUrl = "http://localhost:7188/swagger/v1/swagger.json";

        static void Main(string[] args)
        {
            GenerateTypeScript();
            GenerateCSharp();
        }

        private static void GenerateTypeScript()
        {
            var curDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = new DirectoryInfo(Path.Combine(curDir, "../../../../Clients/TypeScript/"));
            var outDirUi = new DirectoryInfo(Path.Combine(curDir, "../../../../Phys.Lib.Admin.Web/src/lib/api/"));
            outDir.Create();

            var clientName = "Admin";
            var className = $"{clientName}ApiClient";
            using var http = new HttpClient();
            var document = OpenApiDocument.FromJsonAsync(http.GetStringAsync(adminSwaggerJsonUrl).Result).Result;
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
            var outFile2 = new FileInfo(Path.Combine(outDirUi.FullName, $"{className}.ts"));
            var generator = new NSwag.CodeGeneration.TypeScript.TypeScriptClientGenerator(document, settings);
            var clientCode = generator.GenerateFile(ClientGeneratorOutputType.Full);
            File.WriteAllText(outFile.FullName, clientCode);
            File.WriteAllText(outFile2.FullName, clientCode);
            Console.WriteLine($"generated at '{outFile.FullName}'");
            Console.WriteLine($"generated at '{outFile2.FullName}'");
        }

        private static void GenerateCSharp()
        {
            var curDir = AppDomain.CurrentDomain.BaseDirectory;
            var outDir = new DirectoryInfo(Path.Combine(curDir, "../../../../Clients/CSharp/"));
            outDir.Create();

            var clientName = "Admin";
            var className = $"{clientName}ApiClient";
            using var http = new HttpClient();
            var document = OpenApiDocument.FromJsonAsync(http.GetStringAsync(adminSwaggerJsonUrl).Result).Result;
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