using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Phys.Lib.Core.Migration;
using Phys.Shared.HistoryDb;

namespace Phys.Lib.Admin.Api.Api.Migration
{
    public static class MigrationEndpont
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("/", ([FromBody]MigrationTaskModel model, [FromServices]IMigrationService migrationService) =>
            {
                var migration = migrationService.Create(model.Map());
                // TODO: move task to service app
                Task.Factory.StartNew(() => migrationService.Execute(migration));
                return Results.Ok(MigrationModel.Map(migration));
            }).ProducesResponse<MigrationModel>("StartMigration");

            builder.MapGet("/", ([FromServices] IMigrationService migrationService) =>
            {
                var end = SystemClock.Instance.GetCurrentInstant();
                var start = end.Minus(Duration.FromDays(7));
                var migrations = migrationService.List(new HistoryDbQuery(new Interval(start, end), 0, 10));
                return migrations.Select(MigrationModel.Map).ToList();
            }).ProducesResponse<List<MigrationModel>>("GetMigration");
        }
    }
}
