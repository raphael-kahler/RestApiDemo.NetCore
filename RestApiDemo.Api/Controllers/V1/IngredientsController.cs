using Microsoft.AspNetCore.Mvc;
using RestApiDemo.Api.DTO.Converters;
using RestApiDemo.Api.DTO.Creation;
using RestApiDemo.Api.DTO.Response;
using RestApiDemo.Api.DTO.Update;
using RestApiDemo.Framework;
using System;
using System.Linq;
using System.Net;

namespace RestApiDemo.Api.Controllers.V1
{
    /// <summary>
    /// Get information about ingredients.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ResponseCache(CacheProfileName = "Default")] // these profiles map to the options in Startup.cs created in services.AddMVC(options => ...)
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IMealApplicationService _mealService;
        private DtoConverter _dtoConverter;

        /// <summary>
        /// Construct a new IngredientsController.
        /// </summary>
        /// <param name="mealService">The meal service used to access the meal data.</param>
        /// <param name="dtoConverter">The converter used to generate DTOs.</param>
        public IngredientsController(IMealApplicationService mealService, DtoConverter dtoConverter)
        {
            _mealService = mealService ?? throw new ArgumentNullException(nameof(mealService));
            _dtoConverter = dtoConverter ?? throw new ArgumentNullException(nameof(dtoConverter));
        }

        /// <summary>
        /// Get a collection of all ingredients.
        /// </summary>
        /// <param name="count">The maximum number of ingredients to return.</param>
        /// <param name="offset">The number of ingredients to skip before the ingredients that will be returned.</param>
        /// <returns>The collection of ingredients.</returns>
        [HttpGet(Name = "Ingredients_GetAll")]
        [HttpHead(Name = "Ingredients_HeadAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ResponseCache(VaryByQueryKeys = new string[] {"count", "offset" }, Duration = 60)] // cache separate responses for each combination of count and offset for 60 seconds
        public ActionResult<PagedResult<Ingredient>> Get(int count = 10, int offset = 0)
        {
            var ingredients = _dtoConverter.ToDto(_mealService.GetIngredients(count, offset, out var totalCount));
            return new PagedResult<Ingredient>
            {
                Count = ingredients.Count(),
                Offset = offset,
                TotalCount = totalCount,
                Items = ingredients,
            };
        }

        /// <summary>
        /// Get the details of a specific ingredient.
        /// </summary>
        /// <param name="name">The ingredient id.</param>
        /// <returns>The details of the ingredient.</returns>
        [HttpHead("{name}", Name = "Ingredients_HeadsSingle")]
        [HttpGet("{name}", Name = "Ingredients_GetSingle")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<Ingredient> Get(string name)
        {
            if (!_mealService.TryGetIngredient(name, out Domain.Ingredient ingredient))
            {
                return NotFound($"Ingredient with name \"{name}\" does not exist.");
            }
            return _dtoConverter.ToDto(ingredient);
        }

        /// <summary>
        /// Add a new ingredient.
        /// </summary>
        /// <param name="newIngredient">The ingredient details.</param>
        /// <returns>The created ingredient.</returns>
        [HttpPost(Name = "Ingredients_PostNew")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<Ingredient> Post([FromBody] IngredientCreation newIngredient)
        {
            var newIngredientModel = _dtoConverter.ToModel(newIngredient);
            var added = _mealService.AddIngredient(newIngredientModel);
            if (added)
            {
                var createdIngredient = _dtoConverter.ToDto(newIngredientModel);
                return CreatedAtRoute("Ingredients_GetSingle", new { name = createdIngredient.Name }, createdIngredient);
            }
            else
            {
                return Conflict($"Ingredient with name \"{newIngredientModel.Id.Name}\" already exists.");
            }
        }

        /// <summary>
        /// Update an ingredient and replace the old ingredient information with the new provided information.
        /// </summary>
        /// <param name="name">The ingredient name.</param>
        /// <param name="ingredientUpdate">The new details of the ingredient.</param>
        /// <returns>The updated ingredient.</returns>
        [HttpPut("{name}", Name = "Ingredients_PutSingle")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<Ingredient> Put(string name, [FromBody] IngredientUpdate ingredientUpdate)
        {
            if (!_mealService.TryGetIngredient(name, out _))
            {
                return NotFound($"Ingredient with name \"{name}\" does not exist.");
            }

            Domain.Ingredient ingredient;
            try
            {
                ingredient = _dtoConverter.ToModel(name, ingredientUpdate);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            var updatedIngredient = _mealService.SaveIngredient(ingredient);

            return _dtoConverter.ToDto(updatedIngredient);
        }

        /// <summary>
        /// Delete an ingredient.
        /// </summary>
        /// <param name="name">The name of the ingredient to delete.</param>
        /// <returns>Http status code signalling if deletion was completed.</returns>
        [HttpDelete("{name}", Name = "Ingredients_DeleteSingle")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Delete(string name)
        {
            if (_mealService.TryDeleteIngredient(name))
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}