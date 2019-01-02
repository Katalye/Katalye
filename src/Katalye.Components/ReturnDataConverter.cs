using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Katalye.Components
{
    public class ReturnDataConverter : JsonConverter
    {
        // https://blog.bitscry.com/2017/08/31/single-or-array-json-converter/

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //var token = JToken.Load(reader);
            //if (token.Type == JTokenType.Array)
            //{
            //    return token.ToObject<List<T>>();
            //}

            //return new List<T> { token.ToObject<T>() };

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public interface IReturnData
    {
        ReturnType Type { get; set; }

        JObject Data { get; set; }
    }

    public enum ReturnType
    {
        Boolean,
        String,
        Number,
        List,
        Dictionary
    }
}