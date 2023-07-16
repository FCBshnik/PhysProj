namespace Phys.Lib.Tests.Unit
{
    public class CodeTests
    {
        private static readonly Code.Validator codeValidator = new Code.Validator();

        [Theory]
        [InlineData("ya")]
        [InlineData("8decartes")]
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
        public void ValidCodeTests(string code)
        {
            var result = codeValidator.Validate(code);
            result.IsValid.Should().BeTrue();
        }
    }
}
