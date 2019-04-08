using System.ComponentModel.DataAnnotations;

namespace RestApiDemo.Api.DTO.Creation
{
    /// <summary>
    /// The details of a new ingredient.
    /// </summary>
    public class IngredientCreation
    {
        /// <summary>
        /// The ingredient name.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the ingredient.
        /// </summary>
        [Required]
        [MaxLength(10000)]
        public string Description { get; set; }

        /// <summary>
        /// Optional - A link to an image of the ingredient.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
