namespace Katalye.Components.Configuration.ValueParsers
{
    public class BooleanValueParser : IValueParser<bool>
    {
        public object Parse(string value) => bool.Parse(value);
    }
}