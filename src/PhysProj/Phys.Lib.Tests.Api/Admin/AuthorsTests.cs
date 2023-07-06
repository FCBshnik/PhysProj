using Phys.Lib.Admin.Client;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests
    {
        private class AuthorsTests
        {
            private readonly AdminApiClient api;

            public AuthorsTests(AdminApiClient api)
            {
                this.api = api;
            }

            public void List(params string[] authors)
            {
                var result = api.ListAuthorsAsync().Result;
                authors.Should().HaveCount(result.Count);
            }

            public void NotFound(string code, ErrorCode errorCode)
            {
                AdminAssert.ShouldFail(() => api.GetAuthorAsync(code), errorCode);
            }

            public void Found(string code)
            {
                var author = api.GetAuthorAsync(code).Result;
                author.Code.Should().Be(code);
            }

            public void Create(string code)
            {
                var result = api.CreateAuthorAsync(new AuthorCreateModel { Code = code }).Result;
                result.Code.Should().Be(code);
            }

            public void CreateFailed(string code, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.CreateAuthorAsync(new AuthorCreateModel { Code = code }), errorCode);
            }

            public void Delete(string code)
            {
                var result = api.DeleteAuthorAsync(code).Result;
                result.Should().NotBeNull();
            }

            public void UpdateLifetime(string code, AuthorLifetimeUpdateModel update)
            {
                var author = api.UpdateAuthorLifetimeAsync(code, update).Result;
                author.Code.Should().Be(code);
                author.Born.ShouldBeUpdatedWith(update.Born);
                author.Died.ShouldBeUpdatedWith(update.Died);
            }

            public void UpdateLifetimeFailed(string code, AuthorLifetimeUpdateModel update, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.UpdateAuthorLifetimeAsync(code, update), errorCode);
            }

            public void InfoUpdateFailed(string code, string language, AuthorInfoUpdateModel update, ErrorCode errorCode)
            {
                AdminAssert.ShouldFail(() => api.UpdateAuthorInfoAsync(code, language, update), errorCode);
            }

            public void InfoUpdate(string code, string language, AuthorInfoUpdateModel update)
            {
                var author = api.UpdateAuthorInfoAsync(code, language, update).Result;
                author.Code.Should().Be(code);

                var info = author.Infos.FirstOrDefault(i => i.Language == language);
                info.Should().NotBeNull();
                info.Name.Should().Be(update.Name);
                info.Description.Should().Be(update.Description);
            }

            public void InfoDelete(string code, string language)
            {
                var author = api.DeleteAuthorInfoAsync(code, language).Result;
                author.Code.Should().Be(code);
                author.Infos.Should().NotContain(i => i.Language == language);
            }
        }
    }
}