namespace Katalye.Components.Configuration.ValueParsers
{
    public class LongValueParser : IValueParser<long>
    {
        public object Parse(string value) => long.Parse(value);
    }
}