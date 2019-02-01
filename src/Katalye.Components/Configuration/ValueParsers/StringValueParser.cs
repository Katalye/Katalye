namespace Katalye.Components.Configuration.ValueParsers
{
    public class StringValueParser : IValueParser<string>
    {
        public object Parse(string value) => value;
    }
}