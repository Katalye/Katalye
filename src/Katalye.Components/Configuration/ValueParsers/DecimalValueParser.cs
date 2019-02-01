namespace Katalye.Components.Configuration.ValueParsers
{
    public class DecimalValueParser : IValueParser<decimal>
    {
        public object Parse(string value) => decimal.Parse(value);
    }
}