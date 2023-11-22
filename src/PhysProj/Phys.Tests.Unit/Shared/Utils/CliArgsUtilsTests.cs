using Phys.Shared.Utils;

namespace Phys.Tests.Unit.Shared.Utils
{
    public class CliArgsUtilsTests
    {
        [Theory]
        [InlineData("-o", "o")]
        [InlineData("-o 1", "o=1")]
        [InlineData("-o   1", "o=1")]
        [InlineData("--option 2", "option=2")]
        [InlineData("-o 1 -s 2", "o=1;s=2")]
        [InlineData("-o 1 -s -p 3", "o=1;s;p=3")]
        public void ValidArgsTests(string input, string expectedStr)
        {
            var parsed = CliArgsUtils.Parse(input.Split(' '));
            var expected = expectedStr.Split(";").Select(p => p.Split("=").ToList()).ToDictionary(p => p[0], p => p.Count > 1 ? p[1] : null);
            Assert.Equivalent(expected, parsed);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("-o 1 2")]
        [InlineData("-o 1 -o 2")]
        [InlineData("- 1")]
        public void InvalidArgsTests(string input)
        {
            Assert.Throws<ArgumentException>(() => CliArgsUtils.Parse(input.Split(' ')));
        }
    }
}