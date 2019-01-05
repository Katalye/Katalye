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
            var token = JToken.Load(reader);

            var returnType = ReturnType.Unknown;
            switch (token.Type)
            {
                case JTokenType.Object:
                    returnType = ReturnType.Dictionary;
                    break;
                case JTokenType.Array:
                    returnType = ReturnType.List;
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                    returnType = ReturnType.Number;
                    break;
                case JTokenType.String:
                    returnType = ReturnType.String;
                    break;
                case JTokenType.Boolean:
                    returnType = ReturnType.Boolean;
                    break;
            }

            if (token.Type == JTokenType.Array)
            {
                //return token.ToObject<List<T>>();
            }

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public class ReturnData
    {
        public ReturnType Type { get; set; }

        public JObject Data { get; set; }
    }

    public enum ReturnType
    {
        Unknown,
        Boolean,
        String,
        Number,
        List,
        Dictionary
    }
}