using NLog.Config;
using NLog.Filters;
using NLog.Layouts;
using NLog.Targets;
using NLog;

namespace Phys.Shared.Logging
{
    internal static class NLogConfig
    {
        public static void Configure()
        {
            var config = new LoggingConfiguration();

            var layout = new JsonLayout { IncludeEventProperties = true };
            layout.Attributes.Add(new JsonAttribute { Name = "src", Layout = "${logger}" });
            layout.Attributes.Add(new JsonAttribute { Name = "time", Layout = "${longdate}" });
            layout.Attributes.Add(new JsonAttribute { Name = "lvl", Layout = "${level}" });
            layout.Attributes.Add(new JsonAttribute { Name = "thr", Layout = "${threadid}" });
            layout.Attributes.Add(new JsonAttribute { Name = "msg", Layout = "${message}" });
            layout.Attributes.Add(new JsonAttribute { Name = "err", Encode = false, Layout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("type", "${exception:format=type}"),
                    new JsonAttribute("message", "${exception:format=message}"),
                    new JsonAttribute("stacktrace", "${exception:format=tostring}"),
                }
            }
            });

            var microsoftFilter = new ConditionBasedFilter()
            {
                Condition = "level < LogLevel.Warn and starts-with('${logger}','Microsoft.')",
                Action = FilterResult.Ignore
            };
            var npgsqlFilter = new ConditionBasedFilter()
            {
                Condition = "level < LogLevel.Warn and starts-with('${logger}','Npgsql.Command')",
                Action = FilterResult.Ignore
            };

            var console = new ColoredConsoleTarget("console")
            {
                Layout = layout
            };
            var consoleRule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, console);
            consoleRule.FilterDefaultAction = FilterResult.Log;
            consoleRule.Filters.Add(microsoftFilter);
            consoleRule.Filters.Add(npgsqlFilter);
            config.LoggingRules.Add(consoleRule);

            var fileTarget = new FileTarget();
            fileTarget.FileName = "./data/logs/${shortdate}_log.txt";
            fileTarget.Layout = layout;
            fileTarget.KeepFileOpen = true;
            fileTarget.ConcurrentWrites = false;

            var fileRule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, fileTarget);
            fileRule.FilterDefaultAction = FilterResult.Log;
            fileRule.Filters.Add(microsoftFilter);
            consoleRule.Filters.Add(npgsqlFilter);
            config.LoggingRules.Add(fileRule);

            LogManager.Configuration = config;
        }
    }
}
