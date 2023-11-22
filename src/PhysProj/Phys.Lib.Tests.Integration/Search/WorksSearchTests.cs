using Autofac;
using Microsoft.Extensions.Logging;
using Phys.Lib.Autofac;
using Phys.Lib.Db.Search;
using Phys.NLog;
using Phys.Shared.Search;
using Phys.Utils;
using Shouldly;

namespace Phys.Lib.Tests.Db.Search
{
    public class WorksSearchTests : IDisposable
    {
        protected readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected readonly ITestOutputHelper output;

        public WorksSearchTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory, "tests-search");
                AppUtils.OnRun(loggerFactory);
                Log("initializing");
                Init().Wait();
                Log("initialized");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        [Fact]
        public void Tests()
        {
            var container = BuildContainer();

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

        public void Dispose()
        {
            Log("releasing");
            Release().Wait();
            Log("released");
        }

        protected IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggerModule(loggerFactory));
            builder.RegisterModule(new MeiliSearchModule("http://192.168.2.67:7700/", "phys-lib-tests", loggerFactory));
            return builder.Build();
        }

        protected virtual Task Init()
        {
            return Task.CompletedTask;
        }

        protected virtual Task Release()
        {
            return Task.CompletedTask;
        }

        protected void Log(string message)
        {
            output.WriteLine($"{DateTime.UtcNow}: {message}");
        }
    }
}
