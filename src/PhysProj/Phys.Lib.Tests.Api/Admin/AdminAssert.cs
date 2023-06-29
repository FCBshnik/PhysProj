using Phys.Lib.Admin.Client;
using Phys.Lib.Core;

namespace Phys.Lib.Tests.Api.Admin
{
    internal static class AdminAssert
    {
        public static void ShouldFail(this Func<Task> endpoint, ErrorCode errorCode)
        {
            var result = Assert.ThrowsAsync<ApiException<ErrorModel>>(endpoint).Result;
            result.Result.Code.Should().Be(errorCode);
        }

        public static void ShouldBeUpdatedWith(this string resultValue, string updateValue)
        {
            if (updateValue != null)
                if (updateValue == string.Empty)
                    resultValue.Should().BeNull();
                else
                    resultValue.Should().Be(updateValue);
        }
    }
}
