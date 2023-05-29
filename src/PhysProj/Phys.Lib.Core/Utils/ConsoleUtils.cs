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

            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetName().Name;
            var log = LogManager.GetLogger(name);

            log.Info("run {version}", assembly.GetName().Version);

            try { Console.Title = name; } catch {}

            AppDomain.CurrentDomain.UnhandledException += (_, e) => 
            {
                log.Error(e.ExceptionObject as Exception, "UnhandledException");
            };
        }

        private static void ConfigNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var layout = new JsonLayout { IncludeEventProperties = true };
            layout.Attributes.Add(new JsonAttribute { Name = "l", Layout = "${logger}" });
            layout.Attributes.Add(new JsonAttribute { Name = "t", Layout = "${longdate}" });
            layout.Attributes.Add(new JsonAttribute { Name = "l", Layout = "${level}" });
            layout.Attributes.Add(new JsonAttribute { Name = "m", Layout = "${message}" });
            layout.Attributes.Add(new JsonAttribute { Name = "error", Layout = "${exception:format=tostring,StackTrace}" });

            var console = new ColoredConsoleTarget("console") 
            { 
                Layout = layout 
            };

            var loggingRule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, console);
            loggingRule.FilterDefaultAction = FilterResult.Log;
            loggingRule.Filters.Add(new ConditionBasedFilter()
            {
                Condition = "level < LogLevel.Warn and starts-with('${logger}','Microsoft.')",
                Action = FilterResult.Ignore
            });
            config.LoggingRules.Add(loggingRule);
            
            LogManager.Configuration = config;
        }
    }
}
