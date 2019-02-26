using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Katalye.Host.SignalR
{
    public struct RedisInvocation
    {
        /// <summary>
        /// Gets a list of connections that should be excluded from this invocation.
        /// May be null to indicate that no connections are to be excluded.
        /// </summary>
        public IReadOnlyList<string> ExcludedConnectionIds { get; }

        /// <summary>
        /// Gets the message serialization cache containing serialized payloads for the message.
        /// </summary>
        public SerializedHubMessage Message { get; }

        public RedisInvocation(SerializedHubMessage message, IReadOnlyList<string> excludedConnectionIds)
        {
            Message = message;
            ExcludedConnectionIds = excludedConnectionIds;
        }

        public static RedisInvocation Create(string target, object[] arguments, IReadOnlyList<string> excludedConnectionIds = null)
        {
            return new RedisInvocation(
                new SerializedHubMessage(new InvocationMessage(target, null, arguments)),
                excludedConnectionIds);
        }
    }
}