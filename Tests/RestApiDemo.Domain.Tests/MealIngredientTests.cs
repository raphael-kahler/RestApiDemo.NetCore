using RestApiDemo.Domain.Values;
using Xunit;

namespace RestApiDemo.Domain.Tests
{
    public class MealIngredientTests
    {
        [Fact]
        public void ScaleQuantity_ByRatio_QuantityScaledCorrectly()
        {
            var sut = new MealIngredient(new IngredientId("Flour"), new Quantity(Unit.Grams, 100));

            var scaled = sut.ScaleQuantity(3);

            Assert.Equal(300, scaled.Quantity.Value);
        }

        [Fact]
        public void ScaleQuantity_ToNewQuantity_QuantityScaledCorrectly()
        {
            var sut = new MealIngredient(new IngredientId("Flour"), new Quantity(Unit.Grams, 100));
            var scaledQuantity = new Quantity(Unit.Grams, 300);

            var scaled = sut.ScaleQuantity(scaledQuantity);

            Assert.Equal(scaledQuantity, scaled.Quantity);
        }
    }
}
