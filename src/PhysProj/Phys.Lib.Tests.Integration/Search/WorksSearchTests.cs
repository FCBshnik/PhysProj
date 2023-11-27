using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Search;
using Shouldly;

namespace Phys.Lib.Tests.Integration.Search
{
    public class WorksSearchTests : BaseTests
    {
        public WorksSearchTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override void BuildContainer(ContainerBuilder builder)
        {
            base.BuildContainer(builder);
            builder.RegisterModule(new MeilisearchModule("http://192.168.2.67:7700/", "phys-lib-tests", loggerFactory));
        }

        [Fact]
        public void Tests()
        {
            var search = container.Resolve<ITextSearch<WorkTso>>();

            search.Reset();
            search.Index(new[]
            {
                new WorkTso
                {
                    Code = "1",
                    Names = new Dictionary<string, string?> { ["en"] = "On the Nature of Things", ["ru"] = "О природе вещей" },
                    Authors = new Dictionary<string, List<string>> { ["ru"] = new List<string> { "Лукреций" } }
                },
                new WorkTso { Code = "2", Names = new Dictionary<string, string?> { ["en"] = "On The Heavens" } },
                new WorkTso { Code = "3", Names = new Dictionary<string, string?> { ["ru"] = "Клеомед - Учение о круговращении небесных тел" } },
                new WorkTso
                {
                    Code = "4",
                    Names = new Dictionary<string, string?> { ["en"] = "introduction-to-the-phenomena", ["ru"] = "Гемин - Введение в явления" },
                    Authors = new Dictionary<string, List<string>> { ["ru"] = new List<string> { "Щетников" } }
                },
            });

            search.Search("учение").Count.ShouldBe(1);
            search.Search("the").Count.ShouldBe(3);
            search.Search("4he").Count.ShouldBe(0);
            search.Search("Лукреций").Count.ShouldBe(1);
            search.Search("Щетников").Count.ShouldBe(1);
        }
    }
}
