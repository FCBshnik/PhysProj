using Autofac;
using Phys.Lib.Core.Users;
using Phys.Lib.Admin.Client;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests : ApiTests
    {
        private FileInfo projectPath => new FileInfo(Path.Combine(solutionDir.FullName, "Phys.Lib.Api.Admin", "Phys.Lib.Api.Admin.csproj"));
        private const string url = "https://localhost:17188/";

        private AdminApiClient api;

        private IUsersService? users;

        public AdminTests(ITestOutputHelper output) : base(output)
        {
            api = new AdminApiClient(url, http);
        }

        public override async Task Init()
        {
            await base.Init();

            StartApp(url, projectPath);

            var container = BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                users = scope.Resolve<IUsersService>();
            }
        }

        private void CreateUsers()
        {
            users.Create(new UserCreate { Name = "user", Password = "123456", Role = UserRole.User });
            users.Create(new UserCreate { Name = "admin", Password = "123qwe", Role = UserRole.Admin });
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DbModule(GetMongoUrl()));
            builder.RegisterModule(new CoreModule());
            return builder.Build();
        }

        [Fact]
        public void TestAll()
        {
            Log("testing");

            TestHealth();
            TestUsers();
            TestAuthors();
            TestWorks();

            Log("tested");
        }

        private void TestUsers()
        {
            CreateUsers();

            var tests = new UsersTests(api);

            tests.LoginFailed(new LoginModel { Username = "non-existent", Password = "123456" });
            tests.LoginFailed(new LoginModel { Username = "user", Password = "123456" });
            tests.LoginFailed(new LoginModel { Username = "admin", Password = "123456" });

            tests.GetUserInfoUnauthorized();
            var token = tests.LoginSuccess(new LoginModel { Username = "admin", Password = "123qwe" });
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            tests.GetUserInfoAuthorized("admin");
        }

        private void TestHealth()
        {
            var check = api.HealthCheckAsync().Result;
            check.Should().NotBeNull();
        }

        private void TestAuthors()
        {
            var tests = new AuthorsTests(api);
            tests.List();

            tests.NotFound("decartes");

            tests.Create("decartes");
            tests.Found("decartes");

            tests.Create("galilei");
            tests.Found("galilei");
            tests.List("decartes", "galilei");

            tests.Delete("non-existent");
            tests.List("decartes", "galilei");

            tests.Delete("galilei");
            tests.List("decartes");

            tests.Update("decartes", new AuthorUpdateModel { Born = "1596", Died = "1650" });
            tests.Update("decartes", new AuthorUpdateModel { Born = string.Empty, Died = "1650" });

            tests.InfoUpdate("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            tests.InfoUpdate("decartes", "ru", new AuthorInfoUpdateModel { Name = "Рене́ Дека́рт", Description = "французский философ, математик и естествоиспытатель" });

            tests.InfoDelete("decartes", "en");
        }

        private void TestWorks()
        {
            var tests = new WorksTests(api);

            tests.List();

            tests.NotFound("discourse-on-method");

            tests.Create("discourse-on-method");
            tests.Found("discourse-on-method");
            tests.List("discourse-on-method");

            tests.UpdateFailed("non-existent", new WorkUpdateModel(), ErrorCode.NotFound);

            tests.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "fr" });
            tests.Update("discourse-on-method", new WorkUpdateModel { Date = string.Empty, Language = string.Empty });
        }
    }
}