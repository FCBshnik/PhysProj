﻿using Autofac;
using Phys.Lib.Autofac;
using Phys.Lib.Db.Search;
using Phys.Shared.Search;
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
            builder.RegisterModule(new MeiliSearchModule("http://192.168.2.67:7700/", "phys-lib-tests", loggerFactory));
        }

        [Fact]
        public void Tests()
        {
            var search = container.Resolve<ITextSearch<WorkTso>>();

            search.Reset();
            search.Add(new[]
            {
                new WorkTso { Code = "1", Names = new Dictionary<string, string?> { ["en"] = "On the Nature of Things", ["ru"] = "О природе вещей" } },
                new WorkTso { Code = "2", Names = new Dictionary<string, string?> { ["en"] = "On The Heavens" } },
                new WorkTso { Code = "3", Names = new Dictionary<string, string?> { ["ru"] = "Клеомед - Учение о круговращении небесных тел" } },
                new WorkTso { Code = "4", Names = new Dictionary<string, string?> { ["en"] = "introduction-to-the-phenomena", ["ru"] = "Гемин - Введение в явления" } },
            });

            search.Search("учение").Count.ShouldBe(1);
            search.Search("the").Count.ShouldBe(3);
            search.Search("4he").Count.ShouldBe(0);
        }
    }
}