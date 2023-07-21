using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Layouts;
using NLog.Targets;
using System.Reflection;

namespace Phys.Lib.Core.Utils
{
    public static class ConsoleUtils
    {
        public static void OnRun()
        {
            ConfigNLog();

            var args = Environment.GetCommandLineArgs();
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetName().Name;
            var version = assembly.GetName().Version;
            var log = LogManager.GetLogger(name);

            log.Info($"run {version} with args: {string.Join(" ", args)}");

            try { Console.Title = name; } catch {}

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                log.Error(e.ExceptionObject as Exception, "UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                log.Error(e.Exception, "UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void ConfigNLog()
        {
            var config = new LoggingConfiguration();

            var layout = new JsonLayout { IncludeEventProperties = true };
            layout.Attributes.Add(new JsonAttribute { Name = "l", Layout = "${logger}" });
            layout.Attributes.Add(new JsonAttribute { Name = "t", Layout = "${longdate}" });
            layout.Attributes.Add(new JsonAttribute { Name = "l", Layout = "${level}" });
            layout.Attributes.Add(new JsonAttribute { Name = "m", Layout = "${message}" });
            layout.Attributes.Add(new JsonAttribute { Name = "error", Layout = "${exception:format=tostring,StackTrace}" });

            var filter = new ConditionBasedFilter()
            {
                Condition = "level < LogLevel.Warn and starts-with('${logger}','Microsoft.')",
                Action = FilterResult.Ignore
            };

            var console = new ColoredConsoleTarget("console")
            {
                Layout = layout
            };
            var consoleRule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, console);
            consoleRule.FilterDefaultAction = FilterResult.Log;
            consoleRule.Filters.Add(filter);
            config.LoggingRules.Add(consoleRule);

            var fileTarget = new FileTarget();
            fileTarget.FileName = "./data/logs/${shortdate}_log.txt";
            fileTarget.Layout = layout;
            fileTarget.KeepFileOpen = true;
            fileTarget.ConcurrentWrites = false;

            var fileRule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, fileTarget);
            fileRule.FilterDefaultAction = FilterResult.Log;
            fileRule.Filters.Add(filter);
            config.LoggingRules.Add(fileRule);

            LogManager.Configuration = config;
        }
    }
}
