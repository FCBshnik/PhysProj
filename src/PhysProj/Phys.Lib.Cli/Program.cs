using Autofac;
using CommandLine;
using NLog;
using Phys.Lib.Cli;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Data;
using System.Reflection;

internal class Program
{
    private static readonly ILogger log = LogManager.GetCurrentClassLogger();

    private static void Main(string[] args)
    {
        ConsoleUtils.OnRun();

        var parser = new Parser();
        var result = parser.ParseArguments(args, LoadVerbs());
        result.WithNotParsed(e => { log.Error("parse failed: {errors}", e); });
        result.WithParsed(RunCommand);
    }

    private static IContainer BuildContainer(object options)
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule(new DbModule());
        builder.RegisterModule(new CoreModule());
        builder.RegisterModule(new CliModule());
        builder.RegisterInstance(options).AsSelf().SingleInstance();

        return builder.Build();
    }

    private static void RunCommand(object options)
    {
        using (var scope = BuildContainer(options).BeginLifetimeScope())
        {
            try
            {
                var commandType = typeof(ICommand<>).MakeGenericType(options.GetType());
                var command = scope.Resolve(commandType);
                var run = commandType.GetMethods().First();
                run.Invoke(command, new[] { options });
            }
            catch (Exception e)
            {
                log.Error(e, "failed command");
            }
        }
    }

    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}