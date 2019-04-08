using System.Collections.Generic;

namespace RestApiDemo.Api.DTO.Creation
{
    /// <summary>
    /// The details of how a meal should be updated.
    /// Parts that are not set will be ignored. Parts that are set will be updated in the meal.
    /// </summary>
    public class MealUpdate
    {
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
        public IEnumerable<MealIngredientCreation> MealIngredients { get; set; }
    }
}
