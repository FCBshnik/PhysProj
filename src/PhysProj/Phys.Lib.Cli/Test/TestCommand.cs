using NLog;

namespace Phys.Lib.Cli.Test
{
    internal class TestCommand : ICommand<TestOptions>
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void Run(TestOptions options)
        {
            log.Info("start");
        }
    }
}
