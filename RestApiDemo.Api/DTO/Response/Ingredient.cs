namespace RestApiDemo.Api.DTO.Response
{
    /// <summary>
    /// An ingredient.
    /// </summary>
    public class Ingredient
    {
        /// <summary>
        /// The ingredient ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The ingredient name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the ingredient.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A link to an inmage of the ingredient.
        /// </summary>
        public string Image { get; set; }
    }
}