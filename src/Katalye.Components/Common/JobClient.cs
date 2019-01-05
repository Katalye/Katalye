using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Katalye.Components.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobClient
    {
        [EnumMember(Value = "local_async")]
        LocalAsync,

        [EnumMember(Value = "runner_async")]
        RunnerAsync,

        [EnumMember(Value = "wheel_async")]
        WheelAsync
    }
}