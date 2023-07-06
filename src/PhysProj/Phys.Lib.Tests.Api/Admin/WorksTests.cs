using Phys.Lib.Admin.Client;

namespace Phys.Lib.Tests.Api.Admin
{
    public partial class AdminTests
    {
        private class WorksTests
        {
            private readonly AdminApiClient api;

            public WorksTests(AdminApiClient api)
            {
                this.api = api;
            }

            public void List(params string[] works)
            {
                var result = api.ListWorksAsync().Result;
                works.Should().HaveCount(result.Count);
            }

            public void NotFound(string code, ErrorCode errorCode)
            {
                AdminAssert.ShouldFail(() => api.GetWorkAsync(code), errorCode);
            }

            public void Found(string code)
            {
                var result = api.GetWorkAsync(code).Result;
                result.Code.Should().Be(code);
            }

            public void Create(string code)
            {
                var result = api.CreateWorkAsync(new WorkCreateModel { Code = code }).Result;
                result.Code.Should().Be(code);
            }

            public void CreateFailed(string code, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.CreateWorkAsync(new WorkCreateModel { Code = code }), errorCode);
            }

            public void Delete(string code)
            {
                var result = api.DeleteWorkAsync(code).Result;
                var works = api.ListWorksAsync().Result;
                works.Select(w => w.Code).Should().NotContain(code);
            }

            public void UpdateFailed(string code, WorkUpdateModel update, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.UpdateWorkAsync(code, update), errorCode);
            }

            public void Update(string code, WorkUpdateModel update)
            {
                var result = api.UpdateWorkAsync(code, update).Result;
                result.Code.Should().Be(code);

                result.Language.ShouldBeUpdatedWith(update.Language?.ToLowerInvariant());
                result.Publish.ShouldBeUpdatedWith(update.Date);
            }

            public void UpdateInfoFailed(string code, string language, WorkInfoUpdateModel update, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.UpdateWorkInfoAsync(code, language, update), errorCode);
            }

            public void UpdateInfo(string code, string language, WorkInfoUpdateModel update)
            {
                var result = api.UpdateWorkInfoAsync(code, language, update).Result;
                result.Code.Should().Be(code);

                var info = result.Infos.FirstOrDefault(i => i.Language == language);
                info.Should().NotBeNull();
                info.Name.Should().Be(update.Name);
                info.Description.Should().Be(update.Description);
            }

            public void DeleteInfo(string code, string language)
            {
                var author = api.DeleteWorkInfoAsync(code, language).Result;
                author.Code.Should().Be(code);
                author.Infos.Should().NotContain(i => i.Language == language);
            }

            public void LinkAuthorFailed(string code, string authorCode, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.LinkAuthorToWorkAsync(code, authorCode), errorCode);
            }

            public void LinkAuthor(string code, string authorCode)
            {
                var result = api.LinkAuthorToWorkAsync(code, authorCode).Result;
                result.Code.Should().Be(code);
                result.AuthorsCodes.Should().Contain(authorCode);
            }

            public void UnlinkAuthor(string code, string authorCode)
            {
                var result = api.UnlinkAuthorFromWorkAsync(code, authorCode).Result;
                result.Code.Should().Be(code);
                result.AuthorsCodes.Should().NotContain(authorCode);
            }

            public void LinkWorkFailed(string code, string workCode, ErrorCode errorCode = ErrorCode.InvalidArgument)
            {
                AdminAssert.ShouldFail(() => api.LinkWorkToCollectedWorkAsync(code, workCode), errorCode);
            }

            public void LinkWork(string code, string workCode)
            {
                var result = api.LinkWorkToCollectedWorkAsync(code, workCode).Result;
                result.Code.Should().Be(code);
                result.WorksCodes.Should().Contain(workCode);
            }

            public void UnlinkWork(string code, string workCode)
            {
                var result = api.UnlinkWorkFromCollectedWorkAsync(code, workCode).Result;
                result.Code.Should().Be(code);
                result.AuthorsCodes.Should().NotContain(workCode);
            }
        }
    }
}