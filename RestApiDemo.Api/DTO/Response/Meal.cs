using System.Collections.Generic;

namespace RestApiDemo.Api.DTO.Response
{
    /// <summary>
    /// The details of a meal.
    /// </summary>
    public class Meal
    {
        /// <summary>
        /// The meal ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the meal.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cooking instructions for the meal.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// How many people the meal feeds.
        /// </summary>
        public int FeedsNumPeople { get; set; }

        /// <summary>
        /// The ingredients needed for the meal.
        /// </summary>
        public IEnumerable<MealIngredient> MealIngredients { get; set; }
    }
}
