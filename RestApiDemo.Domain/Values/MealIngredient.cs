using RestApiDemo.Domain.Values;
using System;
using System.Collections.Generic;

namespace RestApiDemo.Domain.Values
{
    public class MealIngredient : IEquatable<MealIngredient>
    {
        public IngredientId Ingredient { get; }
        public Quantity Quantity { get; }
        public string Preparation { get; }

        public MealIngredient(IngredientId ingredient, Quantity quantity, string preparation = null)
        {
            Ingredient = ingredient ?? throw new ArgumentNullException(nameof(ingredient));
            Quantity = quantity;
            Preparation = preparation;
        }

        public MealIngredient ScaleQuantity(double ratio) => new MealIngredient(Ingredient, Quantity.ScaleBy(ratio), Preparation);
        public MealIngredient ScaleQuantity(Quantity newQuantity) => new MealIngredient(Ingredient, newQuantity, Preparation);
        public MealIngredient ConvertQuantity(Unit newUnit, IUnitConverter converter) => new MealIngredient(Ingredient, Quantity.ConvertTo(newUnit, converter), Preparation);

        #region auto generated equality overrides
        public bool Equals(MealIngredient other)
        {
            return other != null &&
                   EqualityComparer<IngredientId>.Default.Equals(Ingredient, other.Ingredient) &&
                   EqualityComparer<Quantity>.Default.Equals(Quantity, other.Quantity) &&
                   Preparation == other.Preparation;
        }

        public override bool Equals(object obj) => Equals(obj as MealIngredient);
        public override int GetHashCode() => HashCode.Combine(Ingredient, Quantity, Preparation);
        public static bool operator ==(MealIngredient ingredient1, MealIngredient ingredient2) => EqualityComparer<MealIngredient>.Default.Equals(ingredient1, ingredient2);
        public static bool operator !=(MealIngredient ingredient1, MealIngredient ingredient2) => !(ingredient1 == ingredient2);
        #endregion
    }
}
