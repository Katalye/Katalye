namespace Katalye.Components.Configuration.ValueParsers
{
    public interface IValueParser
    {
        object Parse(string value);
    }

    // ReSharper disable once UnusedTypeParameter
    public interface IValueParser<out T> : IValueParser
    {
    }
}