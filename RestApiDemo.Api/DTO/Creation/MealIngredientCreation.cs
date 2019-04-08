namespace RestApiDemo.Api.DTO.Creation
{
    /// <summary>
    /// The details of a new meal ingredient.
    /// </summary>
    public class MealIngredientCreation
    {
        /// <summary>
        /// The ingredient name.
        /// </summary>
        public string Ingredient { get; set; }

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
