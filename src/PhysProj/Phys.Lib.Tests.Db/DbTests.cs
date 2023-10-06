using Autofac;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core;
using Phys.Lib.Core.Utils;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Logging;
using Phys.NLog;
using Phys.Utils;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Tests.Db
{
    public abstract class DbTests : IDisposable
    {
        protected readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected readonly ITestOutputHelper output;

        public DbTests(ITestOutputHelper output)
        {
            this.output = output;

            try
            {
                NLogConfig.Configure(loggerFactory, "tests-db");
                ProgramUtils.OnRun(loggerFactory);
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
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();
            var users = lifetimeScope.Resolve<IUsersDb>();
            var usersReader = lifetimeScope.Resolve<IDbReader<UserDbo>>();
            var authors = lifetimeScope.Resolve<IAuthorsDb>();
            var works = lifetimeScope.Resolve<IWorksDb>();
            var files = lifetimeScope.Resolve<IFilesDb>();
            new UsersTests(users, usersReader).Run();
            new AuthorsTests(authors).Run();
            new FilesTests(files).Run();
            new WorksTests(works, authors, files).Run();
        }

        protected IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggerModule(loggerFactory));
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