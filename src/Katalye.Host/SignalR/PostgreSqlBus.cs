using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Npgsql;

namespace Katalye.Host.SignalR
{
    public class PostgreSqlBus : IDisposable
    {
        private readonly Func<NpgsqlConnection> _connectionFactory;

        private readonly ConcurrentDictionary<string, PostgreSqlSubscription> _subscriptions = new ConcurrentDictionary<string, PostgreSqlSubscription>();

        public PostgreSqlBus(Func<NpgsqlConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task PublishAsync(string channel, byte[] payload)
        {
            var payloadStr = Convert.ToBase64String(payload);
            using (var connection = _connectionFactory.Invoke())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"NOTIFY \"{nameof(PostgreSqlBus)}_{channel}\", '{payloadStr}'";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<PostgreSqlSubscription> SubscribeAsync(string channelsAll)
        {
            var subscription = _subscriptions.GetOrAdd(channelsAll, s =>
            {
                var connection = _connectionFactory.Invoke();
                return new PostgreSqlSubscription(connection, s);
            });
            await subscription.Monitor();
            return subscription;
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Value.Dispose();
            }
        }
    }
}