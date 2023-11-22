using Autofac;
using CommandLine;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Phys.Utils;
using Phys.NLog;

namespace Phys.Lib.Cli
{
    internal static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

        private static void Main(string[] args)
        {
            NLogConfig.Configure(loggerFactory, "lib-cli");
            AppUtils.OnRun(loggerFactory);

            var parser = new Parser(s => s.IgnoreUnknownArguments = true);
            var result = parser.ParseArguments(args, LoadVerbs());
            result.WithNotParsed(e => log.LogError("parse failed: {errors}", e));
            result.WithParsed(RunCommand);
        }

        private static IContainer BuildContainer(object options)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                // skip first argument which is name of executable file and command name
                .AddJsonConfigFromArgs(args: Environment.GetCommandLineArgs().Skip(2).ToArray())
                .Build();

            var builder = new ContainerBuilder();
            builder.Register(_ => config).AsImplementedInterfaces().SingleInstance();

            builder.RegisterModule(new CliModule(config, loggerFactory));

            builder.RegisterInstance(options).AsSelf().SingleInstance();

            return builder.Build();
        }

        private static void RunCommand(object options)
        {
            using (var scope = BuildContainer(options).BeginLifetimeScope())
                try
                {
                    var commandType = typeof(ICommand<>).MakeGenericType(options.GetType());
                    var command = scope.Resolve(commandType);
                    var run = commandType.GetMethods()[0];
                    run.Invoke(command, new[] { options });
                    log.LogInformation("command completed");
                }
                catch (Exception e)
                {
                    log.LogError(e, "failed command");
                }
        }

        private static Type[] LoadVerbs()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        }
    }
}