using Autofac;
using Phys.Lib.Core.Users;
using Phys.Lib.Admin.Client;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests : ApiTests
    {
        private const string nonExistentCode = "non-existent";

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
            var authors = new AuthorsTests(api);
            var works = new WorksTests(api);
            authors.List();

            authors.NotFound("decartes", ErrorCode.NotFound);

            authors.CreateFailed("decartes-");
            authors.CreateFailed("-decartes");
            authors.CreateFailed("deca--rtes");

            authors.Create("decartes");
            authors.Found("decartes");

            authors.Create("galilei");
            authors.Found("galilei");
            authors.List("decartes", "galilei");

            authors.Delete("non-existent");
            authors.List("decartes", "galilei");

            authors.Delete("galilei");
            authors.List("decartes");

            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = nonExistentCode });
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "165o" });
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1696", Died = "1650" });
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Died = "1650" });
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1696" });
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Born = string.Empty, Died = "1650" });
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Born = "1596", Died = "1650" });

            authors.InfoUpdateFailed(nonExistentCode, "en", new AuthorInfoUpdateModel(), ErrorCode.NotFound);
            authors.InfoUpdateFailed("decartes", nonExistentCode, new AuthorInfoUpdateModel(), ErrorCode.InvalidArgument);
            authors.InfoUpdate("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            authors.InfoUpdate("decartes", "ru", new AuthorInfoUpdateModel { Name = "Рене́ Дека́рт", Description = "французский философ, математик и естествоиспытатель" });

            authors.InfoDelete("decartes", "en");
        }

        private void TestWorks()
        {
            var authors = new AuthorsTests(api);
            var works = new WorksTests(api);

            works.List();

            works.NotFound("discourse-on-method", ErrorCode.NotFound);

            works.Create("discourse-on-method");
            works.Found("discourse-on-method");
            works.List("discourse-on-method");

            works.UpdateFailed(nonExistentCode, new WorkUpdateModel(), ErrorCode.NotFound);
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = nonExistentCode });
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = nonExistentCode });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1737" });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "FR" });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = string.Empty, Language = string.Empty });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "fr" });

            works.UpdateInfoFailed(nonExistentCode, "ru", new WorkInfoUpdateModel(), ErrorCode.NotFound);
            works.UpdateInfoFailed("discourse-on-method", nonExistentCode, new WorkInfoUpdateModel());
            works.UpdateInfo("discourse-on-method", "en", new WorkInfoUpdateModel { Name = "Discourse on the Method", Description = "one of the most influential works in the history of modern philosophy" });
            works.UpdateInfo("discourse-on-method", "ru", new WorkInfoUpdateModel { Name = "Рассуждение о методе", Description = "Считается переломной работой, ознаменовавшей переход от философии Ренессанса и начавшей эпоху философии Нового времени" });

            works.DeleteInfo("discourse-on-method", "ru");
            works.DeleteInfo("discourse-on-method", "es");

            works.LinkAuthorFailed("discourse-on-method", nonExistentCode);
            works.LinkAuthor("discourse-on-method", "decartes");

            // can not be published before author's lifetime
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1537" });
            // can not change author born less than existing publish work
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1650" });

            works.UnlinkAuthor("discourse-on-method", "decartes");

            works.Create("discourse-on-method-chapter-one");
            works.Create("discourse-on-method-chapter-two");
            works.Create("discourse-on-method-chapter-three");
            works.LinkWorkFailed("discourse-on-method", nonExistentCode);
            works.LinkWork("discourse-on-method", "discourse-on-method-chapter-one");
            works.LinkWork("discourse-on-method", "discourse-on-method-chapter-two");
            works.LinkWork("discourse-on-method", "discourse-on-method-chapter-three");
            works.UnlinkWork("discourse-on-method", "discourse-on-method-chapter-one");
            works.UnlinkWork("discourse-on-method", "discourse-on-method-chapter-three");
        }
    }
}