using NLog;
using Phys.Shared.Logging;
using System.Reflection;

namespace Phys.Shared.Utils
{
    public static class ProgramUtils
    {
        public static void OnRun()
        {
            NLogConfig.Configure();

            var args = Environment.GetCommandLineArgs();
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetName().Name;
            var version = assembly.GetName().Version;
            var log = LogManager.GetLogger(name);

            log.Info($"run {version} with args: {string.Join(" ", args)}");

            try { Console.Title = name; } catch { }

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
    }
}
