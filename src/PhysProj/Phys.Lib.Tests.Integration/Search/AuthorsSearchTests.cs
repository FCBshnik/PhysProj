using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Search;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phys.Lib.Tests.Integration.Search
{
    public class AuthorsSearchTests : BaseTests
    {
        public AuthorsSearchTests(ITestOutputHelper output) : base(output)
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
            var search = container.Resolve<ITextSearch<AuthorTso>>();

            search.Reset();
            search.Index(new[]
            {
                new AuthorTso { Code = "1", Names = new Dictionary<string, string?> { ["en"] = "On the Nature of Things", ["ru"] = "О природе вещей" } },
                new AuthorTso { Code = "2", Names = new Dictionary<string, string?> { ["en"] = "On The Heavens" } },
                new AuthorTso { Code = "3", Names = new Dictionary<string, string?> { ["ru"] = "Клеомед - Учение о круговращении небесных тел" } },
                new AuthorTso { Code = "4", Names = new Dictionary<string, string?> { ["en"] = "introduction-to-the-phenomena", ["ru"] = "Гемин - Введение в явления" } },
            });

            search.Search("учение").Count.ShouldBe(1);
            search.Search("the").Count.ShouldBe(3);
            search.Search("4he").Count.ShouldBe(0);
        }
    }
}
