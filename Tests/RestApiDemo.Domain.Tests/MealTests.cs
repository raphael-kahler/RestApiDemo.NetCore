using RestApiDemo.Domain.Values;
using System.Collections.Generic;
using Xunit;

namespace RestApiDemo.Domain.Tests
{
    public class MealTests
    {
        [Fact]
        public void AddIngredient_OneIngredientAdded_MealContainsOneIngredient()
        {
            var sut = new Meal(0, new MealName("Bread"), new List<MealIngredient>(), new CookingInstructions("bake"), new ServingSize(2));

            sut.SetIngredients(new List<MealIngredient> { new MealIngredient(new IngredientId("flour"), new Quantity(Unit.Grams, 500)) });

            Assert.Single(sut.Ingredients);
        }
    }
}
