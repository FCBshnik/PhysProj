using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Shared.Logging;
using Phys.Shared.Utils;
using System.Diagnostics;
using System.Reflection;

namespace Phys.Utils
{
    public static class AppUtils
    {
        /// <summary>
        /// Adds json config from path specified by <paramref name="argName"/> command line argument if such argument is present
        /// </summary>
        public static IConfigurationBuilder AddJsonConfigFromArgs(this IConfigurationBuilder builder, string argName = "appsettings", string[]? args = null)
        {
            // skip first argument which is name of executable file
            args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
            var argsDic = CliArgsUtils.Parse(args);
            if (argsDic.ContainsKey(argName))
                builder = builder.AddJsonFile(argsDic[argName]!, optional: false);

            return builder;
        }

        public static void OnRun(ILoggerFactory loggerFactory)
        {
            var args = Environment.GetCommandLineArgs();
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly!.GetName().Name;
            var version = assembly.GetName().Version;
            var log = loggerFactory.CreateLogger(name!);

            log.LogInformation($"run {version} with args: {string.Join(" ", args)}");

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                log.LogError(e.ExceptionObject as Exception, "UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                log.LogError(e.Exception, "UnobservedTaskException");
                e.SetObserved();
            };

            DiagnosticListener.AllListeners.Subscribe(new HttpRequestsObserver(loggerFactory));
        }
    }
}
