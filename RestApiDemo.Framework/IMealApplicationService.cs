using System.Collections.Generic;
using RestApiDemo.Domain;

namespace RestApiDemo.Framework
{
    public interface IMealApplicationService
    {
        bool TryGetMeal(int id, out Meal meal);
        IEnumerable<Meal> GetMeals(int count, int skip, out int totalCount);
        bool AddMeal(ref Meal meal);
        Meal SaveMeal(Meal meal);
        bool TryDeleteMeal(int id);

        bool TryGetIngredient(string name, out Ingredient ingredient);
        IEnumerable<Ingredient> GetIngredients(int count, int skip, out int totalCount);
        bool AddIngredient(Ingredient ingredient);
        Ingredient SaveIngredient(Ingredient ingredient);
        bool TryDeleteIngredient(string name);
    }
}