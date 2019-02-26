namespace Katalye.Host.SignalR
{
    internal class PostgreSqlChannels
    {
        public string All { get; }

        public PostgreSqlChannels(string prefix)
        {
            All = prefix + ":all";
        }
    }
}