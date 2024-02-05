using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Phys.Lib.Core.Migration;
using Phys.HistoryDb;
using Phys.Queue;

namespace Phys.Lib.Admin.Api.Api.Migrations
{
    public static class MigrationsEndpont
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/migrators", ([FromServices] IMigrationService migrationService) =>
            {
                var migrators = migrationService.ListMigrators();
                return migrators.Select(MigratorModel.Map).ToList();
            }).ProducesResponse<List<MigratorModel>>("ListMigrators");

            builder.MapPost("/", ([FromBody]MigrationTaskModel model,
                [FromServices]IMigrationService migrationService, [FromServices] IQueue queue) =>
            {
                var migration = migrationService.Create(model.Map());
                queue.Publish("migrations", migration);
                return Results.Ok(MigrationModel.Map(migration));
            }).ProducesResponse<MigrationModel>("StartMigration");

            builder.MapGet("/{id}", (string id, [FromServices] IMigrationService migrationService) =>
            {
                var migration = migrationService.Get(id);
                return MigrationModel.Map(migration);
            }).ProducesResponse<MigrationModel>("GetMigration");

            builder.MapGet("/", ([FromServices] IMigrationService migrationService) =>
            {
                var end = SystemClock.Instance.GetCurrentInstant();
                var start = end.Minus(Duration.FromDays(30));
                var migrations = migrationService.ListHistory(new HistoryDbQuery(new Interval(start, end), 0, 10));
                return migrations.Select(MigrationModel.Map).ToList();
            }).ProducesResponse<List<MigrationModel>>("ListMigrations");
        }
    }
}
