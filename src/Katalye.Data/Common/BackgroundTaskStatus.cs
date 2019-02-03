using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Katalye.Data.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BackgroundTaskStatus
    {
        Unknown,
        Queued,
        Processing,
        Failed,
        Succeeded
    }
}