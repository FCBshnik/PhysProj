﻿using Autofac;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Phys.Lib.Core;
using Phys.Lib.Mongo;
using System.Reflection;
using Phys.Lib.Postgres;
using Microsoft.Extensions.Logging;
using Phys.Shared.Utils;
using Phys.Shared.Logging;
using Phys.Shared.Mongo.Configuration;
using Phys.Shared.NLog;

namespace Phys.Lib.Cli
{
    internal static class Program
    {
        private static readonly LoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ILogger log = loggerFactory.CreateLogger(nameof(Program));

        private static void Main(string[] args)
        {
            NLogConfig.Configure(loggerFactory, "cli");
            ProgramUtils.OnRun(loggerFactory);

            var parser = new Parser();
            var result = parser.ParseArguments(args, LoadVerbs());
            result.WithNotParsed(e => log.LogError("parse failed: {errors}", e));
            result.WithParsed(RunCommand);
        }

        private static IContainer BuildContainer(object options)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
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