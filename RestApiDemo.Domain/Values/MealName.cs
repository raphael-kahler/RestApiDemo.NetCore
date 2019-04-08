using System;

namespace RestApiDemo.Domain.Values
{
    public class MealName
    {
        public string Name { get; }

        public MealName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (name.Length > 100)
            {
                throw new ArgumentException("Meal name can be at most 100 characters long.", nameof(name));
            }
        }
    }
}
