using Phys.Lib.Core.Users;

namespace Phys.Lib.Tests.Unit.Users
{
    public class UsersTests
    {
        [Theory]
        [InlineData("ya")]
        [InlineData("8lalala")]
        [InlineData(" admin")]
        [InlineData("admin ")]
        [InlineData("ad$min")]
        public void InvalidNamesTests(string name)
        {
            var result = UserValidators.Name.Validate(name);
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("user-1")]
        public void ValidNamesTests(string name)
        {
            var result = UserValidators.Name.Validate(name);
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("password")]
        [InlineData("123456")]
        public void ValidPasswordTests(string password)
        {
            var result = UserValidators.Password.Validate(password);
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123 456")]
        public void InvalidPasswordTests(string password)
        {
            var result = UserValidators.Password.Validate(password);
            result.IsValid.Should().BeFalse();
        }
    }
}