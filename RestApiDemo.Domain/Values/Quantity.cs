using System;
using System.Collections.Generic;

namespace RestApiDemo.Domain.Values
{
    public class Quantity : IEquatable<Quantity>
    {
        public Unit Unit { get; }
        public double Value { get; }

        public Quantity(Unit unit, double value)
        {
            Unit = unit ?? throw new ArgumentNullException(nameof(unit));

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Quantity requires a non-negative value.");
            }
            Value = value;
        }

        /// <summary>
        /// Scale a quantity by a factor.
        /// </summary>
        /// <param name="scaleRatio">The factor to scale the quantity by,</param>
        /// <returns>The scaled quantity.</returns>
        public Quantity ScaleBy(double scaleRatio)
        {
            if (scaleRatio < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaleRatio), scaleRatio, $"Quantity can only be scaled by a non-negative ratio.");
            }

            return new Quantity(Unit, Value * scaleRatio);
        }

        /// <summary>
        /// Convert the current quantity to a different unit.
        /// If the conversion is not possible an ArgumentException will be thrown.
        /// </summary>
        /// <remarks>
        /// IUnitConverter is a domain service and is used to keep the domain logic clean and simple.
        /// Adding all conversion details to the Quantity class directly would make the class very complex and make it much more difficult to maintain and test.
        /// By splitting the logic, both the Quantity and the converter are easy to test in isolation.
        /// Note that there are no implementations of the converter in the domain project. The implementations live in other parts such as the framework that interacts with the domain project.
        /// </remarks>
        /// <param name="newUnit">The unit to convert to.</param>
        /// <param name="converter">The converter used to perform the unit conversion.</param>
        /// <returns>The converted quantity.</returns>
        public Quantity ConvertTo(Unit newUnit, IUnitConverter converter)
        {
            if (null == newUnit) { throw new ArgumentNullException(nameof(newUnit)); }

            if (!converter.CanConvert(Unit, newUnit, out var conversionRatio))
            {
                throw new ArgumentException($"Unit '{Unit.Name}' is not convertable to '{newUnit.Name}'.", nameof(newUnit));
            }

            return new Quantity(newUnit, Value * conversionRatio);
        }

        // equals and hashcode overrides
        public override bool Equals(object obj) => Equals(obj as Quantity);
        public bool Equals(Quantity other) => other != null && EqualityComparer<Unit>.Default.Equals(Unit, other.Unit) && Value == other.Value;
        public override int GetHashCode() => HashCode.Combine(Unit, Value);
        public static bool operator ==(Quantity quantity1, Quantity quantity2) => EqualityComparer<Quantity>.Default.Equals(quantity1, quantity2);
        public static bool operator !=(Quantity quantity1, Quantity quantity2) => !(quantity1 == quantity2);
    }
}
