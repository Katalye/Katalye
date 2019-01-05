using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;

namespace Katalye.Components.Common
{
    [Header("User-Agent", "Katalye")]
    public interface ISaltApiClient
    {
        [Post("run")]
        Task<CreateJobResult> CreateJob([Body] CreateJob createJob);
    }

    public class CreateJob
    {
        [JsonProperty("client")]
        public JobClient Client { get; set; }

        [JsonProperty("tgt")]
        public string Target { get; set; }

        [JsonProperty("tgt_type")]
        public JobTarget TargetType { get; set; }

        [JsonProperty("fun")]
        public string Function { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("eauth")]
        public string ExternalAuth { get; set; }
    }

    public class CreateJobResult
    {
        public ICollection<CreateJobReturnResult> Return { get; set; }
    }

    public class CreateJobReturnResult
    {
        public ICollection<string> Minions { get; set; }

        public string Jid { get; set; }
    }
}