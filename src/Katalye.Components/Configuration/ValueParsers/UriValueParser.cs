using System;

namespace Katalye.Components.Configuration.ValueParsers
{
    public class UriValueParser : IValueParser<Uri>
    {
        public object Parse(string value) => new Uri(value);
    }
}