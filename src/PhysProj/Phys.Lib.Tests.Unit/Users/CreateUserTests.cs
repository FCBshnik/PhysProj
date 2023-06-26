using FluentAssertions;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Tests.Unit.Users
{
    public class CreateUserTests
    {
        private static readonly UserCreateDataValidator.UserNameValidator userNameValidator = new UserCreateDataValidator.UserNameValidator();

        [Theory]
        [InlineData("ya", false)]
        [InlineData("8lalala", false)]
        [InlineData(" admin", false)]
        [InlineData("admin ", false)]
        [InlineData("ad$min", false)]
        [InlineData("admin", true)]
        public void TestUserName(string userName, bool isValid)
        {
            var result = userNameValidator.Validate(userName);
            result.IsValid.Should().Be(isValid);
        }
    }
}