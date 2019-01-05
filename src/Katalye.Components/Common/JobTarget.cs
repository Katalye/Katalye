using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Katalye.Components.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobTarget
    {
        [EnumMember(Value = "list")]
        List,

        [EnumMember(Value = "glob")]
        Glob,
    }
}