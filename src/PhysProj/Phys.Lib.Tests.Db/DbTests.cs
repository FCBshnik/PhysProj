using Autofac;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Users;

namespace Phys.Lib.Tests.Db
{
    public abstract class DbTests : IDisposable
    {
        protected readonly ITestOutputHelper output;

        public DbTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                ConsoleUtils.OnRun();
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
        public void AllTests()
        {
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();
            new UsersTests(lifetimeScope.Resolve<IUsersDb>()).Run();
            new AuthorsTests(lifetimeScope.Resolve<IAuthorsDb>()).Run();
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new CoreModule());
            Register(builder);
            return builder.Build();
        }

        public void Dispose()
        {
            Log("releasing");
            Release().Wait();
            Log("released");
        }

        protected virtual void Register(ContainerBuilder builder)
        {
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