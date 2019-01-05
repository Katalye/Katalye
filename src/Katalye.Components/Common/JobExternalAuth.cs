using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Katalye.Components.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobExternalAuth
    {
        [EnumMember(Value = "pam")]
        Pam,

        [EnumMember(Value = "auto")]
        Auto
    }
}