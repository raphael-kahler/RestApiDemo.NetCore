using RestApiDemo.Domain.Values;
using System.Collections.Generic;

namespace RestApiDemo.Domain
{
    public class Meal
    {
        public int Id { get; }

        // instead of using base types like int or string, encapsulate properties into value classes/structs that do validation checking.
        public MealName Name { get; private set; }
        public CookingInstructions Instructions { get; private set; }
        public IList<MealIngredient> Ingredients { get; private set; }
        public ServingSize ServingSize { get; private set; }

        public Meal(int id, MealName mealName, IList<MealIngredient> ingredients, CookingInstructions instructions, ServingSize servingSize)
        {
            Id = id;
            Name = mealName;
            Instructions = instructions;
            ServingSize = servingSize;
            Ingredients = ingredients;
        }

        public void ChangeName(string name)
        {
            Name = new MealName(name);
        }

        public void ChangeInstructions(string instructions)
        {
            Instructions = new CookingInstructions(instructions);
        }

        public void SetIngredients(IList<MealIngredient> ingredients)
        {
            Ingredients = ingredients ?? throw new System.ArgumentNullException(nameof(ingredients));
        }

        public void ChangeServingSize(int servingSize)
        {
            ServingSize = new ServingSize(servingSize);
        }

        public void ScaleTo(ServingSize newServingSize)
        {
            var scaleRatio = newServingSize.SizeRatioVersus(ServingSize);
            for (int i = 0; i < Ingredients.Count; ++i)
            {
                Ingredients[i] = Ingredients[i].ScaleQuantity(scaleRatio);
            }
            ServingSize = newServingSize;
        }
    }
}
