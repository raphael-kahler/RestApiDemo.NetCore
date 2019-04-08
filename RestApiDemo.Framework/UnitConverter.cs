using RestApiDemo.Domain;
using RestApiDemo.Domain.Values;
using System;
using System.Collections.Generic;

namespace RestApiDemo.Framework
{
    public class CrappyUnitConverter : IUnitConverter
    {
        // You'd want to implement a better unit conversion, but this will work for demo purposes
        // The main point is that this could be getting values from a DB, calling an external service, etc. But the domain code does not need to change because it only knows about the IUnitConverter interface.
        private static Dictionary<Tuple<Unit, Unit>, double> _conversionRatios = new Dictionary<Tuple<Unit, Unit>, double>
            {
                { new Tuple<Unit, Unit>(Unit.Grams, Unit.Pounds), 0.00220462 },
                { new Tuple<Unit, Unit>(Unit.Pounds, Unit.Grams), 453.59237 },
                { new Tuple<Unit, Unit>(Unit.Grams, Unit.Kilograms), 0.001 },
                { new Tuple<Unit, Unit>(Unit.Kilograms, Unit.Grams), 1000 },
                { new Tuple<Unit, Unit>(Unit.Milliliters, Unit.Cups), 0.00422675 },
                { new Tuple<Unit, Unit>(Unit.Cups, Unit.Milliliters), 236.588236 },
                { new Tuple<Unit, Unit>(Unit.Milliliters, Unit.Liters), 0.001 },
                { new Tuple<Unit, Unit>(Unit.Liters, Unit.Milliliters), 1000 },
            };

        public bool CanConvert(Unit from, Unit to, out double conversionRatio)
        {
            var conversion = new Tuple<Unit, Unit>(from, to);
            var canConvert = _conversionRatios.TryGetValue(conversion, out conversionRatio);
            return canConvert;
        }
    }
}
