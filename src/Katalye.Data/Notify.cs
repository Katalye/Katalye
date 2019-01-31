using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Npgsql;

namespace Katalye.Data
{
    public interface IDistributedNotify
    {
        Task Notify(string channelName, CancellationToken cancellationToken);
        Task WaitOnce(string channelName, CancellationToken cancellationToken);
    }

    public class DistributedNotify : IDistributedNotify
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Func<NpgsqlConnection> _connectionFactory;

        public DistributedNotify(Func<NpgsqlConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Notify(string channelName, CancellationToken cancellationToken)
        {
            Logger.Debug($"Notifying channel {channelName}.");
            using (var connection = _connectionFactory.Invoke())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "NOTIFY @channel";
                    command.Parameters.AddWithValue("channel", $"{nameof(DistributedNotify)}.{channelName}");
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task WaitOnce(string channelName, CancellationToken cancellationToken)
        {
            Logger.Debug($"Starting wait for notification in channel {channelName}.");
            using (var connection = _connectionFactory.Invoke())
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "LISTEN @channel";
                    command.Parameters.AddWithValue("channel", $"{nameof(DistributedNotify)}.{channelName}");
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                cancellationToken.Register(() => CancelConnection(connection));

                await connection.WaitAsync(cancellationToken);

                Logger.Debug("Notification recieved.");
            }
        }

        private void CancelConnection(IDbConnection connection)
        {
            Logger.Debug("Cancellation was requested, closing connection.");
            connection.Close();
        }
    }
}