using RestApiDemo.Domain;
using RestApiDemo.Domain.Values;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RestApiDemo.Framework
{
    public class MealApplicationService : IMealApplicationService
    {
        private static ConcurrentDictionary<int, Meal> _meals;
        private static ConcurrentDictionary<string, Ingredient> _ingredients;

        private static int _mealIdCounter;
        private static int _numMeals;
        private static int _numIngredients;

        static MealApplicationService()
        {
            _meals = new ConcurrentDictionary<int, Meal>();
            _ingredients = new ConcurrentDictionary<string, Ingredient>();

            _ingredients.TryAdd("flour", new Ingredient(new IngredientId("flour"), "bake it", new ImageUri("https://upload.wikimedia.org/wikipedia/commons/6/64/All-Purpose_Flour_%284107895947%29.jpg")));
            _ingredients.TryAdd("water", new Ingredient(new IngredientId("water"), "drink it", new ImageUri("https://upload.wikimedia.org/wikipedia/commons/2/24/Cat_drinking_water_%28ubt%29.jpeg")));
            _ingredients.TryAdd("yeast", new Ingredient(new IngredientId("yeast"), "bake it"));
            _ingredients.TryAdd("chili peppers", new Ingredient(new IngredientId("chili peppers"), "cook it"));
            _ingredients.TryAdd("beans", new Ingredient(new IngredientId("beans"), "cook it"));
            _ingredients.TryAdd("tomatoes", new Ingredient(new IngredientId("tomatoes"), "cook it"));
            _ingredients.TryAdd("ground beef", new Ingredient(new IngredientId("ground beef"), "85% lean"));

            _numIngredients = 7;

            var breadIngredients = new List<MealIngredient>
            {
                new MealIngredient(new IngredientId("flour"), new Quantity(Unit.Grams, 500), "fluffed"),
                new MealIngredient(new IngredientId("water"), new Quantity(Unit.Milliliters, 100)),
                new MealIngredient(new IngredientId("yeast"), new Quantity(Unit.Grams, 10))
            };
            var bread = new Meal(1, new MealName("Bread"), breadIngredients, new CookingInstructions("bake it"), new ServingSize(4));

            var chiliIngredients = new List<MealIngredient>
            {
                new MealIngredient(new IngredientId("chili peppers"), new Quantity(Unit.Kilograms, 10), "chopped"),
                new MealIngredient(new IngredientId("beans"), new Quantity(Unit.Kilograms, 10), "rinsed and drained"),
                new MealIngredient(new IngredientId("tomatoes"), new Quantity(Unit.Kilograms, 10), "chopped"),
                new MealIngredient(new IngredientId("ground beef"), new Quantity(Unit.Kilograms, 10)),
                new MealIngredient(new IngredientId("water"), new Quantity(Unit.Liters, 10))
            };
            var chili = new Meal(2, new MealName("Chili"), chiliIngredients, new CookingInstructions("cook it"), new ServingSize(60));

            _meals.TryAdd(1, bread);
            _meals.TryAdd(2, chili);

            _numMeals = 2;
            _mealIdCounter = _numMeals;
        }

        public IEnumerable<Meal> GetMeals(int count, int offset, out int totalCount)
        {
            totalCount = _numMeals;
            return _meals.Values.Skip(offset).Take(count);
        }

        public bool TryGetMeal(int id, out Meal meal)
        {
            return _meals.TryGetValue(id, out meal);
        }

        public bool AddMeal(ref Meal meal)
        {
            lock (_meals)
            {
                var nextId = _mealIdCounter + 1;
                var mealToAdd = new Meal(nextId, meal.Name, meal.Ingredients, meal.Instructions, meal.ServingSize);
                var added = _meals.TryAdd(nextId, mealToAdd);
                if (added)
                {
                    Interlocked.Increment(ref _numMeals);
                    Interlocked.Increment(ref _mealIdCounter);
                    meal = mealToAdd;
                    return true;
                }
                return false;
            }
        }

        public Meal SaveMeal(Meal meal)
        {
            return _meals.AddOrUpdate(meal.Id, meal, (key, value) => meal);
        }

        public bool TryDeleteMeal(int id)
        {
            var removed = _meals.TryRemove(id, out _);
            if (removed)
            {
                Interlocked.Decrement(ref _numMeals);
            }
            return removed;
        }

        public IEnumerable<Ingredient> GetIngredients(int count, int offset, out int totalCount)
        {
            totalCount = _numIngredients;
            return _ingredients.Values.Skip(offset).Take(count);
        }

        public bool TryGetIngredient(string name, out Ingredient ingredient)
        {
            return _ingredients.TryGetValue(name, out ingredient);
        }


        public bool AddIngredient(Ingredient ingredient)
        {
            var added = _ingredients.TryAdd(ingredient.Id.Name, ingredient);
            if (added)
            {
                Interlocked.Increment(ref _numIngredients);
                return true;
            }
            return false;
        }

        public Ingredient SaveIngredient(Ingredient ingredient)
        {
            return _ingredients.AddOrUpdate(ingredient.Id.Name, ingredient, (key, value) => ingredient);
        }

        public bool TryDeleteIngredient(string name)
        {
            var removed = _ingredients.TryRemove(name, out _);
            if (removed)
            {
                Interlocked.Decrement(ref _numIngredients);
            }
            return removed;
        }
    }
}
