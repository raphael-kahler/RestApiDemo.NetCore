using RestApiDemo.Domain.Values;
using System;
using Xunit;

namespace RestApiDemo.Domain.Tests.Values
{
    public class QuantityTests
    {
        private class FakeUnitConverter : IUnitConverter
        {
            public bool CanConvert(Unit from, Unit to, out double conversionRatio)
            {
                conversionRatio = 0;
                if (from == Unit.Liters && to == Unit.Milliliters)
                {
                    conversionRatio = 1000;
                    return true;
                }
                return false;
            }
        }

        private static readonly IUnitConverter fakeConverter = new FakeUnitConverter();

        [Fact]
        public void ConvertTo_ConvertableUnits_ConversionIsCorrect()
        {
            var sut = new Quantity(Unit.Liters, 5);
            var expectedMilliliters = 5000;

            var converted = sut.ConvertTo(Unit.Milliliters, fakeConverter);

            Assert.Equal(Unit.Milliliters, converted.Unit);
            Assert.Equal(expectedMilliliters, converted.Value);
        }

        [Fact]
        public void ConvertTo_IncompatibleUnits_ThrowsException()
        {
            var sut = new Quantity(Unit.Pieces, 3);

            Assert.Throws<ArgumentException>(() => sut.ConvertTo(Unit.Grams, fakeConverter));
        }
    }
}
