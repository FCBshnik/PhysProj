using Autofac;
using Microsoft.Extensions.Logging;
using Phys.Lib.Autofac;
using Phys.NLog;
using Phys.Shared;

namespace Phys.Lib.Tests.Integration
{
    public abstract class BaseTests : IDisposable
    {
        protected readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected readonly ITestOutputHelper output;

        protected IContainer container;

        public BaseTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory);
                PhysAppContext.Init(loggerFactory);
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

        public void Dispose()
        {
            Log("releasing");
            Release().Wait();
            Log("released");
        }

        protected virtual void BuildContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));
        }

        protected virtual Task Init()
        {
            var builder = new ContainerBuilder();
            BuildContainer(builder);
            container = builder.Build();

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
