using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    public class MigrationsExecutor : IHostedService, IObjectConsumer<MigrationDto>
    {
        private readonly ILogger<MigrationsExecutor> log;
        private readonly IObjectQueue objectQueue;
        private readonly IMigrationService migrationService;

        private IDisposable? sub;

        public MigrationsExecutor(ILogger<MigrationsExecutor> log, IObjectQueue objectQueue, IMigrationService migrationService)
        {
            this.log = log;
            this.objectQueue = objectQueue;
            this.migrationService = migrationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            sub = objectQueue.Consume("migrations", this);
            log.LogInformation("started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            sub?.Dispose();
            log.LogInformation("stopped");
            return Task.CompletedTask;
        }

        void IObjectConsumer<MigrationDto>.Consume(MigrationDto message)
        {
            migrationService.Execute(message);
        }
    }
}
