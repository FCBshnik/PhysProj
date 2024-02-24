using Microsoft.Extensions.Logging;
using Phys.Shared.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phys.Lib.Core.Migration
{
    internal class EventsMigrator : IMigrator
    {
        private readonly ILogger<EventsMigrator> log;

        private readonly IDictionary<string, IEvent> events;
        private readonly IEventBus eventBus;

        public EventsMigrator(IEnumerable<IEvent> events, IEventBus eventBus, ILogger<EventsMigrator> log)
        {
            this.events = events.ToDictionary(e => e.Name);
            this.eventBus = eventBus;
            this.log = log;
        }

        public IEnumerable<string> Sources => events.Keys;

        public IEnumerable<string> Destinations => new[] { "bus" };

        public string Name => MigratorName.Events;

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            log.LogInformation($"publish event '{migration.Source}'");

            eventBus.Publish(events[migration.Source]);
        }
    }
}
