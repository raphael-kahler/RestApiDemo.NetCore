using System.ComponentModel.DataAnnotations;

namespace RestApiDemo.Api.DTO.Update
{
    /// <summary>
    /// The ingredient details that should be updated.
    /// </summary>
    public class IngredientUpdate
    {
        /// <summary>
        /// The new description of the ingredient.
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
