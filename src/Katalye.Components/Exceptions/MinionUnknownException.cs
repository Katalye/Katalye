using System;
using System.Runtime.Serialization;

namespace Katalye.Components.Exceptions
{
    [Serializable]
    public class MinionUnknownException : Exception
    {
        public string MinionSlug { get; }

        public MinionUnknownException(string minionSlug)
        {
            MinionSlug = minionSlug;
        }

        public MinionUnknownException(string message, string minionSlug) : base(message)
        {
            MinionSlug = minionSlug;
        }

        public MinionUnknownException(string message, Exception inner, string minionSlug) : base(message, inner)
        {
            MinionSlug = minionSlug;
        }

        protected MinionUnknownException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}