using RestApiDemo.Domain.Values;
using System;

namespace RestApiDemo.Domain
{
    public class IngredientId
    {
        public string Name { get; }

        public IngredientId(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Length > 200)
            {
                throw new ArgumentException("Name can't be longer than 200 characters", nameof(name));
            }

            Name = name;
        }
    }

    public class Ingredient
    {
        public IngredientId Id { get; }

        public string Description { get; private set; }

        public ImageUri Image { get; private set; }

        public Ingredient(IngredientId id, string description, ImageUri image = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Image = image;
        }
    }
}
