namespace Phys.Lib.Tests.Unit
{
    public class CodeTests
    {
        private static readonly Code.Validator codeValidator = new Code.Validator();

        [Theory]
        [InlineData("ya", false)]
        [InlineData("8decartes", false)]
        [InlineData("decartes8", false)]
        [InlineData(" decartes", false)]
        [InlineData("decartes ", false)]
        [InlineData("decartes-", false)]
        [InlineData("deca--rtes", false)]
        [InlineData("d-s", false)]
        [InlineData("deca&rtes", false)]
        [InlineData("Decartes", false)]
        [InlineData("DECARTES", false)]
        [InlineData("decartes", true)]
        [InlineData("deca-rtes", true)]
        [InlineData("dec-car-tes", true)]
        [InlineData("dec-8-tes", true)]
        public void ValidateCodeTests(string code, bool isValid)
        {
            var result = codeValidator.Validate(code);
            result.IsValid.Should().Be(isValid);
        }
    }
}
