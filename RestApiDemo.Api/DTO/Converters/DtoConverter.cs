using Microsoft.AspNetCore.Mvc;
using RestApiDemo.Api.DTO.Creation;
using RestApiDemo.Api.DTO.Response;
using RestApiDemo.Api.DTO.Update;
using System;
using System.Collections.Generic;

namespace RestApiDemo.Api.DTO.Converters
{
    /// <summary>
    /// A converter used to generate Data Transfer Objects (DTOs).
    /// </summary>
    public class DtoConverter
    {
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Create a new DTO convert
        /// </summary>
        /// <param name="urlHelper">The url helper to generate urls to API endpoints.</param>
        public DtoConverter(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        /// <summary>
        /// Convert a meal model to a response meal DTO.
        /// </summary>
        /// <param name="meal">The meal model.</param>
        /// <returns>The response meal DTO.</returns>
        public Meal ToDto(Domain.Meal meal)
        {
            return new Meal
            {
                Id = meal.Id,
                Name = meal.Name.Name,
                Instructions = meal.Instructions.Text,
                FeedsNumPeople = meal.ServingSize.FeedsNumPeople,
                MealIngredients = ToDto(meal.Ingredients),
            };
        }

        /// <summary>
        /// Convert a collection of meal models to their response meal DTOs.
        /// </summary>
        /// <param name="meals">The meal models.</param>
        /// <returns>The response meal DTOs.</returns>
        public IEnumerable<Meal> ToDto(IEnumerable<Domain.Meal> meals)
        {
            foreach (var meal in meals)
            {
                yield return ToDto(meal);
            }
        }

        /// <summary>
        /// Convert a meal ingredient model to a response DTO.
        /// </summary>
        /// <param name="mealIngredient">The meal ingredient model./</param>
        /// <returns>The meal ingredient response DTO.</returns>
        public MealIngredient ToDto(Domain.Values.MealIngredient mealIngredient)
        {
            return new MealIngredient
            {
                Ingredient = ToLink(mealIngredient.Ingredient),
                Quantity = Math.Round(mealIngredient.Quantity.Value, 2),
                Unit = mealIngredient.Quantity.Unit.Name,
                Preparation = mealIngredient.Preparation
            };
        }

        /// <summary>
        /// Convert a collection of meal ingredient models to their response meal ingredient DTOs.
        /// </summary>
        /// <param name="mealIngredients">The meal ingredient models.</param>
        /// <returns>The response meal ingredient DTOs.</returns>
        public IEnumerable<MealIngredient> ToDto(IEnumerable<Domain.Values.MealIngredient> mealIngredients)
        {
            foreach (var mealIngredient in mealIngredients)
            {
                yield return ToDto(mealIngredient);
            }
        }

        /// <summary>
        /// Convert an ingredient model to a response DTO.
        /// </summary>
        /// <param name="ingredient">The ingredient model./</param>
        /// <returns>The ingredient response DTO.</returns>
        public Ingredient ToDto(Domain.Ingredient ingredient)
        {
            return new Ingredient
            {
                Id = CreateUrl("ingredients", ingredient.Id.Name),
                Name = ingredient.Id.Name,
                Description = ingredient.Description,
                Image = ingredient.Image?.Uri?.ToString()
            };
        }

        /// <summary>
        /// Convert a collection of ingredient models to their response ingredient DTOs.
        /// </summary>
        /// <param name="ingredients">The ingredient models.</param>
        /// <returns>The response ingredient DTOs.</returns>
        public IEnumerable<Ingredient> ToDto(IEnumerable<Domain.Ingredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                yield return ToDto(ingredient);
            }
        }

        /// <summary>
        /// Convert an ingredient ID model to a link.
        /// </summary>
        /// <param name="id">The ingredient ID.</param>
        /// <returns>A link to the ingredient.</returns>
        public Link ToLink(Domain.IngredientId id)
        {
            return new Link
            {
                Name = id.Name,
                Href = CreateUrl("ingredients", id.Name)
            };
        }

        /// <summary>
        /// Create a url to a given API endpoint.
        /// </summary>
        /// <param name="controller">The API controller.</param>
        /// <param name="id">The item ID for the API controller route.</param>
        /// <returns>The absolute uri to the API endpoint.</returns>
        private string CreateUrl(string controller, string id)
        {
            var host = _urlHelper.ActionContext?.HttpContext?.Request?.Host;
            if (null == host)
            {
                return string.Empty;
            }

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = host.Value.Host,
                Path = $"api/v1/{controller}/{id}"
            };
            if (host.Value.Port.HasValue)
            {
                builder.Port = host.Value.Port.Value;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert an ingredient creation DTO to an ingredient model.
        /// </summary>
        /// <param name="ingredientCreation">The ingredient creation DTO.</param>
        /// <returns>The ingredient model.</returns>
        public Domain.Ingredient ToModel(IngredientCreation ingredientCreation)
        {
            var uri = null != ingredientCreation.ImageUrl ? new Domain.Values.ImageUri(ingredientCreation.ImageUrl) : null;
            return new Domain.Ingredient(new Domain.IngredientId(ingredientCreation.Name), ingredientCreation.Description, uri);
        }

        /// <summary>
        /// Convert an ingredient update DTO to an ingredient model.
        /// </summary>
        /// <param name="ingredientName">The ingredient name.</param>
        /// <param name="ingredientUpdate">The ingredient update DTO.</param>
        /// <returns>The ingredient model.</returns>
        public Domain.Ingredient ToModel(string ingredientName, IngredientUpdate ingredientUpdate)
        {
            var uri = null != ingredientUpdate.ImageUrl ? new Domain.Values.ImageUri(ingredientUpdate.ImageUrl) : null;
            return new Domain.Ingredient(new Domain.IngredientId(ingredientName), ingredientUpdate.Description, uri);
        }

        /// <summary>
        /// Convert a meal creation DTO to a meal model.
        /// </summary>
        /// <param name="mealCreation">The meal creation DTO.</param>
        /// <returns>The meal model.</returns>
        public Domain.Meal ToModel(MealCreation mealCreation)
        {
            return new Domain.Meal
            (
                id: 0,
                mealName: new Domain.Values.MealName(mealCreation.Name),
                ingredients: ToModel(mealCreation.MealIngredients),
                instructions: new Domain.Values.CookingInstructions(mealCreation.Instructions),
                servingSize: new Domain.Values.ServingSize(mealCreation.FeedsNumPeople)
            );
        }

        /// <summary>
        /// Convert a meal ingredient creation DTO to a meal ingredient model.
        /// </summary>
        /// <param name="mealIngredients">The meal ingredient DTO.</param>
        /// <returns>The meal ingredient model.</returns>
        public List<Domain.Values.MealIngredient> ToModel(IEnumerable<MealIngredientCreation> mealIngredients)
        {
            var models = new List<Domain.Values.MealIngredient>();

            foreach (var mealIngredient in mealIngredients)
            {
                models.Add(new Domain.Values.MealIngredient
                    (
                        ingredient: new Domain.IngredientId(mealIngredient.Ingredient),
                        quantity: new Domain.Values.Quantity(Domain.Values.Unit.Parse(mealIngredient.Unit), mealIngredient.Quantity),
                        preparation: mealIngredient.Preparation
                    )
                );
            }

            return models;
        }

        /// <summary>
        /// Update a meal model with a meal update DTO.
        /// </summary>
        /// <param name="meal">The meal to update.</param>
        /// <param name="mealUpdate">The meal update DTO.</param>
        public void ToModel(ref Domain.Meal meal, MealUpdate mealUpdate)
        {
            if (null != mealUpdate.Name)
            {
                meal.ChangeName(mealUpdate.Name);
            }
            if (null != mealUpdate.Instructions)
            {
                meal.ChangeInstructions(mealUpdate.Instructions);
            }
            if (null != mealUpdate.MealIngredients)
            {
                var ingredients = ToModel(mealUpdate.MealIngredients);
                meal.SetIngredients(ingredients);
            }
            if (default(int) != mealUpdate.FeedsNumPeople)
            {
                meal.ChangeServingSize(mealUpdate.FeedsNumPeople);
            }
        }
    }
}
