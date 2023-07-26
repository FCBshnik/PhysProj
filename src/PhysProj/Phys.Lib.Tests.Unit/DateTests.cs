using FluentValidation;

namespace Phys.Lib.Tests.Unit
{
    public class DateTests
    {
        [Theory]
        [InlineData("1YYYY")]
        [InlineData("YYBCE")]
        [InlineData("BCE")]
        [InlineData("-12")]
        [InlineData("-12BCE")]
        [InlineData("BCE12")]
        [InlineData("1BCE2")]
        [InlineData("+123")]
        [InlineData("123!")]
        [InlineData("2024")]
        [InlineData("Y55")]
        [InlineData("1987Y")]
        [InlineData("204Y")]
        public void InvalidDateTests(string date)
        {
            var result = Date.Parse(date);
            result.Ok.Should().BeFalse();
        }

        [Theory]
        [InlineData("5000BCE", -5000, -5000)]
        [InlineData("35YBCE", -360, -340)]
        [InlineData("300BCE", -300, -300)]
        [InlineData("20BCE", -20, -20)]
        [InlineData("1BCE", -1, -1)]
        [InlineData("10", 10, 10)]
        [InlineData("30", 30, 30)]
        [InlineData("399", 399, 399)]
        [InlineData("1546", 1546, 1546)]
        [InlineData("2023", 2023, 2023)]
        [InlineData("0YYBCE", -100, 100)]
        [InlineData("0YY", -100, 100)]
        [InlineData("3YYBCE", -400, -200)]
        [InlineData("33Y2BCE", -350, -310)]
        [InlineData("6YBCE", -70, -50)]
        [InlineData("32YBCE", -330, -310)]
        [InlineData("0Y", -10, 10)]
        [InlineData("3Y", 20, 40)]
        [InlineData("4YY", 300, 500)]
        [InlineData("155Y", 1540, 1560)]
        [InlineData("150Y", 1490, 1510)]
        [InlineData("150Y5", 1450, 1550)]
        [InlineData("145Y5", 1400, 1500)]
        [InlineData("15YY", 1400, 1600)]
        [InlineData("18Y5", 130, 230)]
        [InlineData("198Y", 1970, 1990)]
        [InlineData("202Y", 2010, 2030)]
        public void ValidDateTests(string date, int min, int max)
        {
            var result = Date.Parse(date);
            result.Ok.Should().BeTrue();
            result.Value.Min.Should().Be(min);
            result.Value.Max.Should().Be(max);
        }

        [Theory]
        [InlineData("1650", "1690")]
        [InlineData("1600", "1700")]
        [InlineData("3YY", "4YY")]
        [InlineData("3YYBCE", "2YYBCE")]
        public void ValidLifetimeTests(string born, string died)
        {
            Date.ValidateLifetime(Date.Parse(born).Value, Date.Parse(died).Value);
        }

        [Theory]
        [InlineData("1695", "1690")]
        [InlineData("1600", "1800")]
        [InlineData("3YY", "5YY")]
        [InlineData("25YBCE", "26YBCE")]
        [InlineData("3YYBCE", "30YBCE")]
        public void InvalidLifetimeTests(string born, string died)
        {
            Assert.Throws<ValidationException>(() => Date.ValidateLifetime(Date.Parse(born).Value, Date.Parse(died).Value));
        }

        [Theory]
        [InlineData("1650", "1670")]
        public void ValidBornAndPublishTests(string born, string publish)
        {
            Date.ValidateBornAndPublish(Date.Parse(born).Value, Date.Parse(publish).Value);
        }

        [Theory]
        [InlineData("1695", "1590")]
        public void InvalidBornAndPublishTests(string born, string publish)
        {
            Assert.Throws<ValidationException>(() => Date.ValidateBornAndPublish(Date.Parse(born).Value, Date.Parse(publish).Value));
        }
    }
}
