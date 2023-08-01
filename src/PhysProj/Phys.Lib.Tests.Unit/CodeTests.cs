namespace Phys.Lib.Tests.Unit
{
    public class CodeTests
    {
        private static readonly Code.Validator codeValidator = new Code.Validator();

        [Theory]
        [InlineData("ya")]
        [InlineData(" decartes")]
        [InlineData("decartes ")]
        [InlineData("decartes-")]
        [InlineData("Decartes")]
        [InlineData("DECARTES")]
        public void InvalidCodeTests(string code)
        {
            var result = codeValidator.Validate(code);
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("decartes8")]
        [InlineData("decartes-8")]
        [InlineData("decartes")]
        [InlineData("deca-rtes")]
        [InlineData("dec-car-tes")]
        [InlineData("dec-8-tes")]
        [InlineData("8decartes")]
        public void ValidCodeTests(string code)
        {
            var result = codeValidator.Validate(code);
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Decartes", "decartes")]
        [InlineData("Decartes Rene", "decartes-rene")]
        [InlineData("Decartes@#Rene))", "decartes-rene")]
        [InlineData("@Decartes+++Rene))", "decartes-rene")]
        [InlineData("Decartes__Rene", "decartes-rene")]
        [InlineData("Decartes____Rene", "decartes-rene")]
        [InlineData("Петровский, Фёдор Александрович", "petrovskij-fyodor-aleksandrovich")]
        public void NormalizeTests(string code, string expected)
        {
            var actual = Code.NormalizeAndValidate(code);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
