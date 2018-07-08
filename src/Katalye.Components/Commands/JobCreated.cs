using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MediatR;

using Newtonsoft.Json;

using NLog;

namespace Katalye.Components.Commands
{
    public static class JobCreated
    {
        public class Command : IRequest<Result>
        {
            public string Jid { get; set; }

            public CreationData Data { get; set; }
        }

        public class CreationData
        {
            [JsonProperty("tgt_type")]
            public string TargetType { get; set; }

            [JsonProperty("jid")]
            public string Jid { get; set; }

            [JsonProperty("tgt")]
            public string Target { get; set; }

            [JsonProperty("_stamp")]
            public DateTimeOffset TimeStamp { get; set; }

            [JsonProperty("user")]
            public string User { get; set; }

            [JsonProperty("arg")]
            public List<object> Arg { get; set; }

            [JsonProperty("fun")]
            public string Fun { get; set; }

            [JsonProperty("minions")]
            public List<string> Minions { get; set; }

            [JsonProperty("missing")]
            public List<string> Missing { get; set; }
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"A new job {message.Jid} was created.");

                throw new NotImplementedException();
            }
        }
    }
}