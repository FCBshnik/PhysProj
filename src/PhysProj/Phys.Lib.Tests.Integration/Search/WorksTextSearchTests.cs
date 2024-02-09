using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Core;
using Phys.Lib.Search;
using Shouldly;

namespace Phys.Lib.Tests.Integration.Search
{
    public class WorksTextSearchTests : BaseTests
    {
        public WorksTextSearchTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override void BuildContainer(ContainerBuilder builder)
        {
            base.BuildContainer(builder);
            builder.RegisterModule(new MeilisearchModule("http://192.168.2.67:7700/", "123456", "phys-lib-tests", loggerFactory));
        }

        [Fact]
        public async Task Tests()
        {
            var search = Container.Resolve<ITextSearch<WorkTso>>();

            await search.Reset(Language.AllAsStrings);
            await search.Index(new[]
            {
                new WorkTso
                {
                    Code = "1",
                    Names = new Dictionary<string, string?> { ["en"] = "On the Nature of Things", ["ru"] = "О природе вещей" },
                    Authors = new Dictionary<string, List<string?>> { ["ru"] = new List<string?> { "Лукреций" } },
                    Info = new WorkInfoTso { Code = "1" },
                },
                new WorkTso { Code = "2", Names = new Dictionary<string, string?> { ["en"] = "On The Heavens" }, Info = new WorkInfoTso { Code = "2" }, },
                new WorkTso { Code = "3", Names = new Dictionary<string, string?> { ["ru"] = "Клеомед - Учение о круговращении небесных тел" }, Info = new WorkInfoTso { Code = "3" }, },
                new WorkTso
                {
                    Code = "4",
                    Names = new Dictionary<string, string?> { ["en"] = "introduction-to-the-phenomena", ["ru"] = "Гемин - Введение в явления" },
                    Authors = new Dictionary<string, List<string?>> { ["ru"] = new List<string?> { "Щетников" } },
                    Info = new WorkInfoTso { Code = "4" },
                },
            });

            search.Search("учение").Result.Count.ShouldBe(1);
            search.Search("the").Result.Count.ShouldBe(3);
            search.Search("4he").Result.Count.ShouldBe(0);
            search.Search("Лукреций").Result.Count.ShouldBe(1);
            search.Search("Щетников").Result.Count.ShouldBe(1);
        }
    }
}
