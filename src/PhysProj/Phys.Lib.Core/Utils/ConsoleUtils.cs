using NLog;
using NLog.Layouts;
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

            var console = new NLog.Targets.ColoredConsoleTarget("console");
            var layout = new JsonLayout { IncludeEventProperties = true };
            layout.Attributes.Add(new JsonAttribute { Name = "t", Layout = "${longdate}" });
            layout.Attributes.Add(new JsonAttribute { Name = "l", Layout = "${level}" });
            layout.Attributes.Add(new JsonAttribute { Name = "m", Layout = "${message}" });
            layout.Attributes.Add(new JsonAttribute { Name = "error", Layout = "${exception:format=tostring,StackTrace}" });

            console.Layout = layout;
            config.AddRule(LogLevel.Info, LogLevel.Fatal, console);

            LogManager.Configuration = config;
        }
    }
}
