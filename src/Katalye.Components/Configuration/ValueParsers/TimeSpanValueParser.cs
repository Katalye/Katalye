using System;

namespace Katalye.Components.Configuration.ValueParsers
{
    public class TimeSpanValueParser : IValueParser<TimeSpan>
    {
        public object Parse(string value) => TimeSpan.Parse(value);
    }
}