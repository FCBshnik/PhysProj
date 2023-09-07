using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Phys.Shared.Utils
{
    public static class ProgramUtils
    {
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
        }
    }
}
