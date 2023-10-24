using Phys.Lib.Admin.Client;
using System.Diagnostics;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests
    {
        internal class MigrationTests
        {
            private readonly AdminApiClient api;

            public MigrationTests(AdminApiClient api)
            {
                this.api = api;
            }

            public void List(int expectedCount)
            {
                var result = api.ListMigrationsAsync().Result;
                result.Count.Should().Be(expectedCount);
            }

            public MigrationModel Start(MigrationTaskModel task)
            {
                var migration = api.StartMigrationAsync(task).Result;
                migration.Should().NotBeNull();
                return migration;
            }

            public MigrationModel WaitCompleted(string id, TimeSpan timeout, string expectedResult, int expectedMigratedCount)
            {
                var sw = Stopwatch.StartNew();
                while (sw.Elapsed < timeout)
                {
                    var migration = api.GetMigrationAsync(id).Result;
                    migration.Should().NotBeNull();
                    if (migration.Status == "completed")
                    {
                        migration.Result.Should().Be(expectedResult);
                        migration.Stats.Created.Should().Be(expectedMigratedCount);
                        return migration;
                    }

                    Thread.Sleep(500);
                }

                throw new Exception("migration failed");
            }
        }
    }
}
