using NLog.Config;
using NLog.Filters;
using NLog.Layouts;
using NLog.Targets;
using NLog;
using NLog.Common;
using NLog.Targets.ElasticSearch;
using NLog.Targets.Wrappers;
using NLog.Extensions.Logging;

namespace Phys.Shared.NLog
{
    public static class NLogConfig
    {
        public static void Configure(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory, string appName)
        {
            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true,
                ReplaceLoggerFactory = true,
            });

            LogManager.Setup().SetupExtensions(s =>
                s.RegisterLayoutRenderer("longdate_utc", (logEvent) => logEvent.TimeStamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff"))
            );

            var config = new LoggingConfiguration();

            var layout = new JsonLayout { IncludeEventProperties = true };
            layout.Attributes.Add(new JsonAttribute { Name = "src", Layout = "${logger}" });
            layout.Attributes.Add(new JsonAttribute { Name = "time", Layout = "${longdate_utc}" });
            layout.Attributes.Add(new JsonAttribute { Name = "lvl", Layout = "${level}" });
            layout.Attributes.Add(new JsonAttribute { Name = "thr", Layout = "${threadid}" });
            layout.Attributes.Add(new JsonAttribute { Name = "msg", Layout = "${message}" });
            layout.Attributes.Add(new JsonAttribute
            {
                Name = "err",
                Encode = false,
                Layout = new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("type", "${exception:format=type}"),
                        new JsonAttribute("message", "${exception:format=message}"),
                        new JsonAttribute("stacktrace", "${exception:format=tostring}"),
                    }
                }
            });

            AddTarget(config, new ColoredConsoleTarget("console")
            {
                Layout = layout,
            });
            AddTarget(config, new FileTarget
            {
                FileName = "./data/logs/${shortdate}.txt",
                Layout = layout,
                KeepFileOpen = true,
                ConcurrentWrites = false,
            });

            var elasticTarget = new ElasticSearchTarget
            {
                Uri = "http://192.168.1.107:9200/",
                Index = $"physproj-{appName}-" + "${date:universalTime=true:format=yyyy-MM-dd}",
                DocumentType = null, // deprecated since 8 version of elasticsearch, if not set to null than elasticsearch respond with error
                IncludeAllProperties = true,
                IncludeDefaultFields = true,
                EnableJsonLayout = true,
                Layout = layout,
                DisableCertificateValidation = true,
            };
            AddTarget(config, new BufferingTargetWrapper("buffer", elasticTarget)
            {
                BufferSize = 50,
                OverflowAction = BufferingTargetWrapperOverflowAction.Discard,
                FlushTimeout = 1000,
            });

            InternalLogger.LogLevel = LogLevel.Info;
            InternalLogger.LogFile = "nlog.internal.txt";
            LogManager.Configuration = config;
        }

        private static void AddTarget(LoggingConfiguration config, Target target)
        {
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

            var rule = new LoggingRule("*", LogLevel.Info, LogLevel.Fatal, target);
            rule.FilterDefaultAction = FilterResult.Log;
            rule.Filters.Add(microsoftFilter);
            rule.Filters.Add(npgsqlFilter);
            config.LoggingRules.Add(rule);
        }
    }
}
