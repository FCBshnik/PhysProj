﻿using Autofac;
using Phys.Lib.Core.Users;
using Phys.Lib.Admin.Client;

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
            var user = users.Create("user", "123456");
            users.AddRole(user, UserRole.User);
            var admin = users.Create("admin", "123qwe");
            users.AddRole(admin, UserRole.Admin);
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

            // TODO: extract complex scenarios
            // empty list at start
            authors.List();
            // not found author
            authors.NotFound("decartes", ErrorCode.NotFound);
            // create invalid author
            authors.CreateFailed("decartes-");
            authors.CreateFailed("-decartes");
            authors.CreateFailed("deca--rtes");
            // create valid author
            authors.Create("decartes");
            authors.Found("decartes");
            // create second valid author
            authors.Create("galilei");
            authors.Found("galilei");
            authors.List("decartes", "galilei");
            // create author duplicate failed
            authors.CreateFailed("galilei");
            authors.List("decartes", "galilei");
            // delete is idempotent
            authors.Delete(nonExistentCode);
            authors.List("decartes", "galilei");
            // delete author
            authors.Delete("galilei");
            authors.NotFound("galilei", ErrorCode.NotFound);
            authors.List("decartes");
            // update with invalid lifetime
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = nonExistentCode });
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "165o" });
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1696", Died = "1650" });
            // update with valid lifetime
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Died = "1650" });
            // separate update with invalid lifetime
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1696" });
            // update with valid lifetimes
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Born = string.Empty, Died = "1650" });
            authors.UpdateLifetime("decartes", new AuthorLifetimeUpdateModel { Born = "1596", Died = "1650" });
            // update not found author
            authors.InfoUpdateFailed(nonExistentCode, "en", new AuthorInfoUpdateModel(), ErrorCode.NotFound);
            // update info with invalid language
            authors.InfoUpdateFailed("decartes", nonExistentCode, new AuthorInfoUpdateModel(), ErrorCode.InvalidArgument);
            // update info with valid languages
            authors.InfoUpdate("decartes", "en", new AuthorInfoUpdateModel { Name = "René Descartes", Description = "French philosopher, scientist, and mathematician" });
            authors.InfoUpdate("decartes", "ru", new AuthorInfoUpdateModel { Name = "Рене́ Дека́рт", Description = "французский философ, математик и естествоиспытатель" });
            // delete info
            authors.InfoDelete("decartes", "en");
            // delete info is idempotent
            authors.InfoDelete("decartes", nonExistentCode);
            // can not delete author linked to work
            authors.Create("author");
            works.Create("work-of-author");
            works.LinkAuthor("work-of-author", "author");
            authors.DeleteFailed("author");
            works.Delete("work-of-author");
            authors.Delete("author");
        }

        private void TestWorks()
        {
            var authors = new AuthorsTests(api);
            var works = new WorksTests(api);

            // TODO: extract complex scenarios
            // empty list at start
            works.List();
            // not found work
            works.NotFound("discourse-on-method", ErrorCode.NotFound);
            // create invalid work
            works.CreateFailed("dm");
            works.CreateFailed("@discourse-on-method");
            works.CreateFailed("@discourse-on-");
            // create valid work
            works.Create("discourse-on-method");
            works.Found("discourse-on-method");
            works.List("discourse-on-method");
            // create duplicate failed
            works.CreateFailed("discourse-on-method");
            // update non existing failed
            works.UpdateFailed(nonExistentCode, new WorkUpdateModel(), ErrorCode.NotFound);
            // update with invalid language failed
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = nonExistentCode });
            // update with invalid date failed
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = nonExistentCode });
            // update with valid date and language
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1737" });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "FR" });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = string.Empty, Language = string.Empty });
            works.Update("discourse-on-method", new WorkUpdateModel { Date = "1637", Language = "fr" });
            // update non existing failed
            works.UpdateInfoFailed(nonExistentCode, "ru", new WorkInfoUpdateModel(), ErrorCode.NotFound);
            // update with invalid language failed
            works.UpdateInfoFailed("discourse-on-method", nonExistentCode, new WorkInfoUpdateModel());
            // update with valid info
            works.UpdateInfo("discourse-on-method", "en", new WorkInfoUpdateModel { Name = "Discourse on the Method", Description = "one of the most influential works in the history of modern philosophy" });
            works.UpdateInfo("discourse-on-method", "ru", new WorkInfoUpdateModel { Name = "Рассуждение о методе", Description = "Считается переломной работой, ознаменовавшей переход от философии Ренессанса и начавшей эпоху философии Нового времени" });
            // delete info
            works.DeleteInfo("discourse-on-method", "ru");
            // delete info is idempotent
            works.DeleteInfo("discourse-on-method", "es");
            // link invalid author failed
            works.LinkAuthorFailed("discourse-on-method", nonExistentCode);
            // link valid author
            works.LinkAuthor("discourse-on-method", "decartes");
            // update with publish before author's born failed
            works.UpdateFailed("discourse-on-method", new WorkUpdateModel { Date = "1537" });
            // update born of author which has work published before failed
            authors.UpdateLifetimeFailed("decartes", new AuthorLifetimeUpdateModel { Born = "1650" });
            // unlink author
            works.UnlinkAuthor("discourse-on-method", "decartes");
            // unlink author is idempotent
            works.UnlinkAuthor("discourse-on-method", "decartes");
            works.UnlinkAuthor("discourse-on-method", nonExistentCode);
            // link work with invalid original failed
            works.LinkOriginalFailed("discourse-on-method", nonExistentCode);
            // link invalid work with original failed
            works.LinkOriginalFailed(nonExistentCode, "discourse-on-method-original", ErrorCode.NotFound);
            // link original to self failed
            works.LinkOriginalFailed("discourse-on-method", "discourse-on-method");
            // link with valid original
            works.Create("discourse-on-method-original");
            works.LinkOriginal("discourse-on-method", "discourse-on-method-original");
            // link original with circular dependency failed
            works.Create("discourse-on-method-proxy");
            works.LinkOriginal("discourse-on-method-proxy", "discourse-on-method");
            works.LinkOriginalFailed("discourse-on-method", "discourse-on-method-proxy");
            // link invalid work to collected work failed
            works.LinkSubWorkFailed("discourse-on-method", nonExistentCode);
            // link works to collected work
            works.Create("discourse-on-method-chapter-one");
            works.Create("discourse-on-method-chapter-two");
            works.Create("discourse-on-method-chapter-three");
            works.LinkSubWork("discourse-on-method", "discourse-on-method-chapter-one");
            works.LinkSubWork("discourse-on-method", "discourse-on-method-chapter-two");
            works.LinkSubWork("discourse-on-method", "discourse-on-method-chapter-three");
            // link original which is sub-work failed
            works.LinkOriginalFailed("discourse-on-method", "discourse-on-method-chapter-one");
            // link collected work to self failed
            works.LinkSubWorkFailed("discourse-on-method", "discourse-on-method");
            // unlink works from collected work
            works.UnlinkSubWork("discourse-on-method", "discourse-on-method-chapter-one");
            works.UnlinkSubWork("discourse-on-method", "discourse-on-method-chapter-three");
            // can not delete work linked as original
            works.Create("original");
            works.Create("work");
            works.LinkOriginal("work", "original");
            works.DeleteFailed("original");
            works.Delete("work");
            works.Delete("original");
            // can not delete work linked as sub-work
            works.Create("work");
            works.Create("sub-work");
            works.LinkSubWork("work", "sub-work");
            works.DeleteFailed("sub-work");
            works.Delete("work");
            works.Delete("sub-work");
        }
    }
}