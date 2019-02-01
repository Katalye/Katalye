namespace Katalye.Components.Configuration.ValueParsers
{
    public class IntValueParser : IValueParser<int>
    {
        public object Parse(string value) => int.Parse(value);
    }
}