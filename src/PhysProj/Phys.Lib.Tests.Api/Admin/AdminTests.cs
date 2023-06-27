using Autofac;
using Phys.Lib.Core.Users;
using Phys.Lib.Admin.Client;
using System.Net;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Tests.Api.Admin
{
    public class AdminTests : ApiTests
    {
        private FileInfo projectPath => new FileInfo(Path.Combine(solutionDir.FullName, "Phys.Lib.Api.Admin", "Phys.Lib.Api.Admin.csproj"));
        private const string url = "https://localhost:17188/";

        private AdminApiClient api;

        private IUsersService? users;
        private IAuthorsService? authors;

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
                authors = scope.Resolve<IAuthorsService>();
            }
        }

        private void CreateUsers()
        {
            users.Create(new UserCreateData { Name = "user", Password = "123456", Role = UserRole.User });
            users.Create(new UserCreateData { Name = "admin", Password = "123qwe", Role = UserRole.Admin });
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DbModule(GetMongoUrl()));
            builder.RegisterModule(new CoreModule());
            return builder.Build();
        }

        [Fact]
        public void AllTests()
        {
            Log("testing");

            HealthTests();
            AuthTests();
            AuthorsTests();

            Log("tested");
        }

        private void AuthTests()
        {
            CreateUsers();

            LoginFailedTest();
            LoginAsUserFailedTest();
            GetUserInfoUnauthorizedTest();
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginAsAdminSuccessTest());
            GetUserInfoAuthorizedTest();
        }

        private void HealthTests()
        {
            var check = api.HealthCheckAsync().Result;
            check.Should().NotBeNull();
        }

        private void LoginFailedTest()
        {
            api.Invoking(i => i.LoginAsync(new LoginModel
            {
                Username = "badUserName",
                Password = "badUserPassword"
            }).Result).Should().Throw<ApiException<ErrorModel>>().Where(e => e.Result.Code == ErrorCode.LoginFailed);
        }

        private void LoginAsUserFailedTest()
        {
            api.Invoking(i => i.LoginAsync(new LoginModel
            {
                Username = "user",
                Password = "123456"
            }).Result).Should().Throw<ApiException<ErrorModel>>().Which.Result.Code.Should().Be(ErrorCode.LoginFailed);
        }

        private string LoginAsAdminSuccessTest()
        {
            var result = api.LoginAsync(new LoginModel
            {
                Username = "admin",
                Password = "123qwe"
            }).Result;
            result.Token.Should().NotBeEmpty();

            return result.Token;
        }

        private void GetUserInfoUnauthorizedTest()
        {
            var result = Assert.ThrowsAsync<ApiException>(api.GetUserInfoAsync).Result;
            result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        private void GetUserInfoAuthorizedTest()
        {
            var result = api.GetUserInfoAsync().Result;
            result.Name.Should().Be("admin");
        }

        private void AuthorsTests()
        {
            AssertAuthors();

            AuthorNotFoundTest("decartes");

            authors.Create("decartes");
            AuthorFoundTest("decartes");

            authors.Create("galilei");
            AuthorFoundTest("galilei");

            AssertAuthors("decartes", "galilei");

            api.DeleteAuthorAsync("none").Wait();
            AssertAuthors("decartes", "galilei");

            api.DeleteAuthorAsync("galilei").Wait();
            AssertAuthors("decartes");

            AuthorUpdateTest("decartes", new AuthorUpdateModel { Born = "1596", Died = "1650" });
            AuthorUpdateTest("decartes", new AuthorUpdateModel { Born = string.Empty, Died = "1650" });

            AuthorUpdateInfoTest("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            AuthorUpdateInfoTest("decartes", "ru", new AuthorInfoUpdateModel { Name = "Рене́ Дека́рт", Description = "французский философ, математик и естествоиспытатель" });
            AuthorDeleteInfoTest("decartes", "en");
        }

        private void AuthorNotFoundTest(string code)
        {
            var result = Assert.ThrowsAsync<ApiException<ErrorModel>>(() => api.GetAuthorAsync(code)).Result;
            result.Result.Code.Should().Be(ErrorCode.NotFound);
        }

        private void AuthorFoundTest(string code)
        {
            var author = api.GetAuthorAsync(code).Result;
            author.Code.Should().Be(code);
        }

        private void AuthorUpdateTest(string code, AuthorUpdateModel update)
        {
            var author = api.UpdateAuthorAsync(code, update).Result;
            author.Code.Should().Be(code);
            author.Born.Should().Be(update.Born);
            author.Died.Should().Be(update.Died);
        }

        private void AuthorUpdateInfoTest(string code, string language, AuthorInfoUpdateModel update)
        {
            var author = api.UpdateAuthorInfoAsync(code, language, update).Result;
            author.Code.Should().Be(code);

            var info = author.Infos.FirstOrDefault(i => i.Language == language);
            info.Should().NotBeNull();
            info.Name.Should().Be(update.Name);
            info.Description.Should().Be(update.Description);
        }

        private void AuthorDeleteInfoTest(string code, string language)
        {
            var author = api.DeleteAuthorInfoAsync(code, language).Result;
            author.Code.Should().Be(code);
            author.Infos.Should().NotContain(i => i.Language == language);
        }

        private void AssertAuthors(params string[] authors)
        {
            var result = api.ListAuthorsAsync().Result;
            authors.Should().HaveCount(result.Count);
        }
    }
}