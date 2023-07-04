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
            var tests = new AuthorsTests(api);
            tests.List();

            tests.NotFound("decartes", ErrorCode.NotFound);

            tests.CreateFailed("decartes-");
            tests.CreateFailed("-decartes");
            tests.CreateFailed("deca--rtes");

            tests.Create("decartes");
            tests.Found("decartes");

            tests.Create("galilei");
            tests.Found("galilei");
            tests.List("decartes", "galilei");

            tests.Delete("non-existent");
            tests.List("decartes", "galilei");

            tests.Delete("galilei");
            tests.List("decartes");

            tests.UpdateFailed("decartes", new AuthorUpdateModel { Born = nonExistentCode });
            tests.UpdateFailed("decartes", new AuthorUpdateModel { Born = "165o" });
            tests.UpdateFailed("decartes", new AuthorUpdateModel { Born = "1696", Died = "1650" });
            tests.Update("decartes", new AuthorUpdateModel { Died = "1650" });
            tests.UpdateFailed("decartes", new AuthorUpdateModel { Born = "1696" });
            tests.Update("decartes", new AuthorUpdateModel { Born = string.Empty, Died = "1650" });
            tests.Update("decartes", new AuthorUpdateModel { Born = "1596", Died = "1650" });

            tests.InfoUpdateFailed(nonExistentCode, "en", new AuthorInfoUpdateModel(), ErrorCode.NotFound);
            tests.InfoUpdateFailed("decartes", nonExistentCode, new AuthorInfoUpdateModel(), ErrorCode.InvalidArgument);
            tests.InfoUpdate("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            tests.InfoUpdate("decartes", "ru", new AuthorInfoUpdateModel { Name = "Рене́ Дека́рт", Description = "французский философ, математик и естествоиспытатель" });

            tests.InfoDelete("decartes", "en");
        }

        private void TestWorks()
        {
            var tests = new WorksTests(api);

            tests.List();

            tests.NotFound("discourse-on-method", ErrorCode.NotFound);

            tests.Create("discourse-on-method");
            tests.Found("discourse-on-method");
            tests.List("discourse-on-method");

            tests.UpdateFailed(nonExistentCode, new WorkUpdateModel(), ErrorCode.NotFound);
            tests.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = nonExistentCode });
            tests.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = nonExistentCode });
            // can not be published before author's lifetime
            tests.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1537" });
            // but can be published after author's lifetime
            tests.Update("discourse-on-method", new WorkUpdateModel { Date = "1737" });
            tests.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "FR" });
            tests.Update("discourse-on-method", new WorkUpdateModel { Date = string.Empty, Language = string.Empty });
            tests.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "fr" });

            tests.UpdateInfoFailed(nonExistentCode, "ru", new WorkInfoUpdateModel(), ErrorCode.NotFound);
            tests.UpdateInfoFailed("discourse-on-method", nonExistentCode, new WorkInfoUpdateModel());
            tests.UpdateInfo("discourse-on-method", "en", new WorkInfoUpdateModel { Name = "Discourse on the Method", Description = "one of the most influential works in the history of modern philosophy" });
            tests.UpdateInfo("discourse-on-method", "ru", new WorkInfoUpdateModel { Name = "Рассуждение о методе", Description = "Считается переломной работой, ознаменовавшей переход от философии Ренессанса и начавшей эпоху философии Нового времени" });

            tests.DeleteInfo("discourse-on-method", "ru");
            tests.DeleteInfo("discourse-on-method", "es");

            tests.LinkAuthorFailed("discourse-on-method", nonExistentCode, ErrorCode.NotFound);
            tests.LinkAuthor("discourse-on-method", "decartes");
            tests.UnlinkAuthor("discourse-on-method", "decartes");

            tests.Create("discourse-on-method-chapter-one");
            tests.Create("discourse-on-method-chapter-two");
            tests.Create("discourse-on-method-chapter-three");
            tests.LinkWorkFailed("discourse-on-method", nonExistentCode, ErrorCode.NotFound);
            tests.LinkWork("discourse-on-method", "discourse-on-method-chapter-one");
            tests.LinkWork("discourse-on-method", "discourse-on-method-chapter-two");
            tests.LinkWork("discourse-on-method", "discourse-on-method-chapter-three");
            tests.UnlinkWork("discourse-on-method", "discourse-on-method-chapter-one");
            tests.UnlinkWork("discourse-on-method", "discourse-on-method-chapter-three");
        }
    }
}