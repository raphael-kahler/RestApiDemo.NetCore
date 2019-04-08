using Microsoft.AspNetCore.Mvc;
using RestApiDemo.Api.DTO.Converters;
using RestApiDemo.Api.DTO.Creation;
using RestApiDemo.Api.DTO.Response;
using RestApiDemo.Framework;
using System;
using System.Linq;
using System.Net;

namespace RestApiDemo.Api.Controllers.V1
{
    /// <summary>
    /// Get information about meals.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ResponseCache(CacheProfileName = "Default")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly IMealApplicationService _mealService;
        private DtoConverter _dtoConverter;

        /// <summary>
        /// Construct a new MealController.
        /// </summary>
        /// <param name="mealService">The meal service used to access the meal data.</param>
        /// <param name="dtoConverter">The converter used to generate DTOs.</param>
        public MealsController(IMealApplicationService mealService, DtoConverter dtoConverter)
        {
            _mealService = mealService ?? throw new ArgumentNullException(nameof(mealService));
            _dtoConverter = dtoConverter ?? throw new ArgumentNullException(nameof(dtoConverter));
        }

        /// <summary>
        /// Get a collection of all meals.
        /// </summary>
        /// <param name="count">The maximum number of meals in the collection.</param>
        /// <param name="offset">The number of meals that should be skipped. Use this to page through the whole collection of meals.</param>
        /// <returns>The collection of meals.</returns>
        [HttpGet(Name = "Meals_GetAll")]
        [HttpHead(Name = "Meals_HeadAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ResponseCache(VaryByQueryKeys = new string[] {"count", "offset" }, Duration = 60)] // cache separate responses for each combination of count and offset
        public ActionResult<PagedResult<Meal>> Get(int count = 10, int offset = 0)
        {
            var meals = _dtoConverter.ToDto(_mealService.GetMeals(count, offset, out var totalCount));
            return new PagedResult<Meal>
            {
                Count = meals.Count(),
                Offset = offset,
                TotalCount = totalCount,
                Items = meals,
            };
        }

        /// <summary>
        /// Get the details of a specific meal.
        /// </summary>
        /// <param name="id">The ID of the meal.</param>
        /// <param name="peopleToFeed">Optional - Scale the ingredients to make the meal feed this number of people.</param>
        /// <returns>The details of the meal.</returns>
        [HttpGet("{id}", Name = "Meals_GetSingle")]
        [HttpHead("{id}", Name = "Meals_HeadSingle")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ResponseCache(CacheProfileName = "Never")]
        public ActionResult<Meal> Get(int id, int? peopleToFeed)
        {
            if (!_mealService.TryGetMeal(id, out Domain.Meal meal))
            {
                return NotFound($"Meal with id \"{id}\" does not exist.");
            }
            if (peopleToFeed.HasValue)
            {
                meal.ScaleTo(new Domain.Values.ServingSize(peopleToFeed.Value));
            }
            return _dtoConverter.ToDto(meal);
        }

        /// <summary>
        /// Add a new meal.
        /// </summary>
        /// <param name="meal">The details of the meal.</param>
        /// <returns>The created meal.</returns>
        [HttpPost(Name = "Meals_PostNew")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<Meal> Post([FromBody] MealCreation meal)
        {
            Domain.Meal mealModel;
            try
            {
                mealModel = _dtoConverter.ToModel(meal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            _mealService.AddMeal(ref mealModel);
            return CreatedAtRoute("Meals_GetSingle", new { id = mealModel.Id }, mealModel);
        }


        /// <summary>
        /// Change details of a meal.
        /// </summary>
        /// <param name="id">The ID of the meal to update.</param>
        /// <param name="mealUpdate">The meal details that should be updated.</param>
        /// <returns>The updated meal.</returns>
        [HttpPatch("{id}", Name = "Meals_PatchSingle")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<Meal> Patch(int id, [FromBody] MealUpdate mealUpdate)
        {
            if (!_mealService.TryGetMeal(id, out Domain.Meal meal))
            {
                return NotFound($"Meal with id \"{id}\" does not exist.");
            }

            try
            {
                _dtoConverter.ToModel(ref meal, mealUpdate);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            var updatedMeal = _mealService.SaveMeal(meal);

            return Ok(_dtoConverter.ToDto(updatedMeal));
        }

        /// <summary>
        /// Delete a meal.
        /// </summary>
        /// <param name="id">The ID of the meal to delete.</param>
        /// <returns>Http status code signalling if deletion was completed.</returns>
        [HttpDelete("{id}", Name = "Meals_DeleteSingle")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult Delete(int id)
        {
            if (_mealService.TryDeleteMeal(id))
            {
                return NoContent();
            }
            return NotFound($"Meal with id \"{id}\" does not exist.");
        }
    }
}