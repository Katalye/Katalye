using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Katalye.Components
{
    public class UtcTimeConverter : IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var datetime = (DateTime) base.ReadJson(reader, objectType, existingValue, serializer);
            var result = new DateTime(datetime.Ticks, DateTimeKind.Utc);
            return result;
        }
    }
}