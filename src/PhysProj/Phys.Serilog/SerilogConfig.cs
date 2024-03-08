using Serilog;
using Serilog.Filters;
using Serilog.Templates.Themes;
using Serilog.Templates;

namespace Phys.Serilog
{
    public static class SerilogConfig
    {
        public static void Configure(Microsoft.Extensions.Logging.LoggerFactory loggerFactory)
        {
            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/logs");

            global::Serilog.Debugging.SelfLog.Enable(Console.Error);

            var consoleFormatter = new ExpressionTemplate("{ {t: UtcDateTime(@t), l: if @l = 'Information' then 'Info' else @l, th: ThreadId, s: SourceContext, m: @m, @x } }\n",
                theme: TemplateTheme.Code);

            var formatter = new ExpressionTemplate("{ {t: UtcDateTime(@t), l: if @l = 'Information' then 'Info' else @l, th: ThreadId, s: SourceContext, m: @mt, @x," +
                " ..@p, SourceContext: undefined(), ThreadId: undefined() } }\n");

            Log.Logger = new LoggerConfiguration()
                .Enrich.With<ThreadIdEnricher>()
                .MinimumLevel.Information()
                .Filter.ByExcluding(Matching.WithProperty<string>("SourceContext", sc => sc.StartsWith("Microsoft.AspNetCore")))
                .Filter.ByExcluding(Matching.WithProperty<string>("SourceContext", sc => sc.StartsWith("Microsoft.Hosting")))
                .WriteTo.Console(consoleFormatter)
                .WriteTo.File(formatter, $"{logsDir}/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            loggerFactory.AddSerilog();
        }
    }
}
