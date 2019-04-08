using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestApiDemo.Api.DTO.Creation
{
    /// <summary>
    /// The details of a new meal.
    /// </summary>
    public class MealCreation
    {
        /// <summary>
        /// The name of the meal.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The cooking instructions for the meal.
        /// </summary>
        [Required]
        public string Instructions { get; set; }

        /// <summary>
        /// How many people the meal feeds.
        /// </summary>
        [Required]
        public int FeedsNumPeople { get; set; }

        /// <summary>
        /// The ingredients needed for the meal.
        /// </summary>
        [Required]
        public IEnumerable<MealIngredientCreation> MealIngredients { get; set; }
    }
}
