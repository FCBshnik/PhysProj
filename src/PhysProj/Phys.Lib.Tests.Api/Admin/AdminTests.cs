using Autofac;
using Phys.Lib.Core;
using Phys.Lib.Core.Users;
using Phys.Lib.Data;
using Phys.Lib.Admin.Client;
using System.Net;
using Xunit.Abstractions;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Tests.Api.Admin
{
    public class AdminTests : ApiTests
    {
        private const string url = "https://localhost:17188/";

        private AdminApiClient? api;

        private FileInfo projectPath => new FileInfo(Path.Combine(solutionDir.FullName, "Phys.Lib.Api.Admin", "Phys.Lib.Api.Admin.csproj"));

        private IUsers users;
        private IAuthors authors;

        public AdminTests(ITestOutputHelper output) : base(output)
        {
        }

        public override async Task Init()
        {
            await base.Init();

            StartApp(url, projectPath);

            var container = BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                users = scope.Resolve<IUsers>();
                authors = scope.Resolve<IAuthors>();
            }

            api = new AdminApiClient(url, http);
        }

        private void InitUsers()
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
        public async void AllTests()
        {
            Log("testing");

            await HealthCheck();

            InitUsers();

            await LoginFailed();
            await LoginAsUserFailed();
            var token = await LoginAsAdminSuccess();
            await GetUserInfoUnauthorized();
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await GetUserInfoAuthorized();

            await AuthorsTests();

            Log("tested");
        }

        private async Task HealthCheck()
        {
            var check = await api.HealthCheckAsync();
            Assert.True(check != null, "Health check");
        }

        private async Task LoginFailed()
        {
            var result = await Assert.ThrowsAsync<ApiException<ErrorModel>>(async () => await api.LoginAsync(new LoginModel
            {
                Username = "badUserName",
                Password = "badUserPassword"
            }));

            Assert.Equal(ErrorCode.LoginFailed, result.Result.Code);
        }

        private async Task LoginAsUserFailed()
        {
            var result = await Assert.ThrowsAsync<ApiException<ErrorModel>>(async () => await api.LoginAsync(new LoginModel
            {
                Username = "user",
                Password = "123456"
            }));

            Assert.Equal(ErrorCode.LoginFailed, result.Result.Code);
        }

        private async Task<string> LoginAsAdminSuccess()
        {
            var result = await api.LoginAsync(new LoginModel
            {
                Username = "admin",
                Password = "123qwe"
            });

            Assert.NotEmpty(result.Token);

            return result.Token;
        }

        private async Task GetUserInfoUnauthorized()
        {
            var result = await Assert.ThrowsAsync<ApiException>(api.GetUserInfoAsync);

            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        private async Task GetUserInfoAuthorized()
        {
            var result = await api.GetUserInfoAsync();

            Assert.Equal("admin", result.Name);
        }

        private async Task AuthorsTests()
        {
            await AssertAuthors();

            await TestAuthorNotFound("decartes");
            
            authors.Create("decartes");
            await TestAuthorFound("decartes");

            authors.Create("galilei");
            await TestAuthorFound("galilei");

            await AssertAuthors("decartes", "galilei");

            await api.DeleteAuthorAsync("none");
            await AssertAuthors("decartes", "galilei");

            await api.DeleteAuthorAsync("galilei");
            await AssertAuthors("decartes");

            await TestAuthorUpdate("decartes", new AuthorUpdateModel { Born = "1596", Died = "1650" });
            await TestAuthorUpdate("decartes", new AuthorUpdateModel { Born = string.Empty, Died = "1650" });

            await TestAuthorUpdateInfo("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            await TestAuthorDeleteInfo("decartes", "en");
        }

        private async Task TestAuthorNotFound(string code)
        {
            var result = await Assert.ThrowsAsync<ApiException<ErrorModel>>(async () => await api.GetAuthorAsync(code));
            Assert.Equal(ErrorCode.NotFound, result.Result.Code);
        }

        private async Task TestAuthorFound(string code)
        {
            var author = await api.GetAuthorAsync(code);
            Assert.Equal(code, author.Code);
        }

        private async Task TestAuthorUpdate(string code, AuthorUpdateModel update)
        {
            var author = await api.UpdateAuthorAsync(code, update);
            Assert.Equal(code, author.Code);
            Assert.Equal(update.Born, author.Born);
            Assert.Equal(update.Died, author.Died);
        }

        private async Task TestAuthorUpdateInfo(string code, string language, AuthorInfoUpdateModel update)
        {
            var author = await api.UpdateAuthorInfoAsync(code, language, update);
            Assert.Equal(code, author.Code);

            var info = author.Infos.FirstOrDefault(i => i.Language == language);
            Assert.NotNull(info);
            Assert.Equal(update.Name, info.Name);
            Assert.Equal(update.Description, info.Description);
        }

        private async Task TestAuthorDeleteInfo(string code, string language)
        {
            var author = await api.DeleteAuthorInfoAsync(code, language);
            Assert.Equal(code, author.Code);

            var info = author.Infos.FirstOrDefault(i => i.Language == language);
            Assert.Null(info);
        }

        private async Task AssertAuthors(params string[] authors)
        {
            var result = await api.ListAuthorsAsync();
            Assert.Equal(result.Count, authors.Length);
        }
    }
}