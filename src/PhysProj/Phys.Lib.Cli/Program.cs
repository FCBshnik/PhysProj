using Autofac;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Phys.Lib.Cli;
using Phys.Lib.Core;
using Phys.Lib.Mongo;
using System.Reflection;
using Phys.Lib.Postgres;
using Microsoft.Extensions.Logging;
using Phys.Shared.Utils;
using Phys.Shared.Logging;

internal static class Program
{
    private static readonly ILoggerFactory loggerFactory = new LoggerFactory();
    private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

    private static void Main(string[] args)
    {
        ProgramUtils.OnRun();

        var parser = new Parser();
        var result = parser.ParseArguments(args, LoadVerbs());
        result.WithNotParsed(e => { log.LogError("parse failed: {errors}", e); });
        result.WithParsed(RunCommand);
    }

    private static IContainer BuildContainer(object options)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new ContainerBuilder();

        var mongoUrl = config.GetConnectionString("mongo");
        if (mongoUrl != null)
            builder.RegisterModule(new MongoModule(mongoUrl, loggerFactory));
        var postgresUrl = config.GetConnectionString("postgres");
        if (postgresUrl != null)
            builder.RegisterModule(new PostgresModule(postgresUrl, loggerFactory));
        builder.RegisterModule(new NLogModule(loggerFactory));
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
                log.LogInformation("command completed");
            }
            catch (Exception e)
            {
                log.LogError(e, "failed command");
            }
        }
    }

    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}