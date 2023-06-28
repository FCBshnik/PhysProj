using Docker.DotNet.Models;
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

            public void NotFound(string code)
            {
                var result = Assert.ThrowsAsync<ApiException<ErrorModel>>(() => api.GetWorkAsync(code)).Result;
                result.Result.Code.Should().Be(ErrorCode.NotFound);
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

            public void UpdateFailed(string code, WorkUpdateModel update, ErrorCode errorCode)
            {
                var result = Assert.ThrowsAsync<ApiException<ErrorModel>>(() => api.UpdateWorkAsync(code, update)).Result;
                result.Result.Code.Should().Be(errorCode);
            }

            public void Update(string code, WorkUpdateModel update)
            {
                var result = api.UpdateWorkAsync(code, update).Result;
                result.Code.Should().Be(code);
                result.Language.Should().Be(update.Language);
                result.Date.Should().Be(update.Date);
            }
        }
    }
}