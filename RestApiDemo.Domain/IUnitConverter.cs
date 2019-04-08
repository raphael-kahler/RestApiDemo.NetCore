using RestApiDemo.Domain.Values;

namespace RestApiDemo.Domain
{
    public interface IUnitConverter
    {
        bool CanConvert(Unit from, Unit to, out double conversionRatio);
    }
}