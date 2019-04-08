namespace RestApiDemo.Api.DTO.Response
{
    /// <summary>
    /// An ingredient for a meal.
    /// </summary>
    public class MealIngredient
    {
        /// <summary>
        /// The details of the ingredient.
        /// </summary>
        public Link Ingredient { get; set; }

        /// <summary>
        /// The unit of the ingredient.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// The quantity of the ingredient.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// How the ingredient should be prepared.
        /// </summary>
        public string Preparation { get; set; }
    }
}
