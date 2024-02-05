using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IHostedService, IConsumer<MigrationDto>
    {
        private readonly ILogger<MigrationsExecutor> log;
        private readonly IQueue queue;
        private readonly IMigrationService migrationService;

        private IDisposable? sub;

        public MigrationsExecutor(ILogger<MigrationsExecutor> log, IQueue queue, IMigrationService migrationService)
        {
            this.log = log;
            this.queue = queue;
            this.migrationService = migrationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            sub = queue.Consume("migrations", this);
            log.LogInformation("started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            sub?.Dispose();
            log.LogInformation("stopped");
            return Task.CompletedTask;
        }

        void IConsumer<MigrationDto>.Consume(MigrationDto message)
        {
            migrationService.Execute(message);
        }
    }
}
