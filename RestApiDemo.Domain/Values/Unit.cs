using System;
using System.Collections.Generic;

namespace RestApiDemo.Domain.Values
{
    public class Unit : IEquatable<Unit>
    {
        private Unit(string name) => Name = name;

        public string Name { get; }

        public static Unit Grams       { get; } = new Unit("grams");
        public static Unit Kilograms   { get; } = new Unit("kg");
        public static Unit Pounds      { get; } = new Unit("lbs");
        public static Unit Milliliters { get; } = new Unit("ml");
        public static Unit Liters      { get; } = new Unit("l");
        public static Unit Cups        { get; } = new Unit("cups");
        public static Unit Pieces      { get; } = new Unit("pieces");

        public static Unit Parse(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "grams":  return Grams;
                case "kg":     return Kilograms;
                case "lbs":    return Pounds;
                case "ml":     return Milliliters;
                case "l":      return Liters;
                case "cups":   return Cups;
                case "pieces": return Pieces;
                default: throw new ArgumentException($"\"{name}\" is not a valid unit.", nameof(name));
            }
        }

        public override bool Equals(object obj) => Equals(obj as Unit);
        public bool Equals(Unit other) => other != null && Name == other.Name;
        public override int GetHashCode() => HashCode.Combine(Name);
        public static bool operator ==(Unit unit1, Unit unit2) => EqualityComparer<Unit>.Default.Equals(unit1, unit2);
        public static bool operator !=(Unit unit1, Unit unit2) => !(unit1 == unit2);
    }
}