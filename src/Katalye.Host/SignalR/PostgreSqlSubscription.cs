using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Katalye.Host.SignalR
{
    public class PostgreSqlSubscription : IDisposable
    {
        private readonly ConcurrentBag<Action<PostgreSqlSubscriptionMessage>> _actions = new ConcurrentBag<Action<PostgreSqlSubscriptionMessage>>();

        private readonly NpgsqlConnection _connection;
        private readonly string _channel;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Task _task;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1);

        public PostgreSqlSubscription(NpgsqlConnection connection, string channel)
        {
            _connection = connection;
            _channel = channel;
        }

        public void OnMessage(Action<PostgreSqlSubscriptionMessage> action)
        {
            _actions.Add(action);
        }

        public void Invoke(PostgreSqlSubscriptionMessage message)
        {
            foreach (var action in _actions)
            {
                action.Invoke(message);
            }
        }

        public async Task Monitor()
        {
            if (_task == null)
            {
                await _connectionLock.WaitAsync();
                try
                {
                    if (_task == null)
                    {
                        var token = _tokenSource.Token;

                        await _connection.OpenAsync(token);

                        _task = Task.Factory.StartNew(() =>
                        {
                            _connection.Notification += (o, e) =>
                            {
                                var payload = Convert.FromBase64String(e.AdditionalInformation);
                                Invoke(new PostgreSqlSubscriptionMessage
                                {
                                    Message = payload
                                });
                            };

                            using (var cmd = new NpgsqlCommand($"LISTEN \"{nameof(PostgreSqlBus)}_{_channel}\"", _connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            token.Register(() => _connection.Close());

                            while (!token.IsCancellationRequested)
                            {
                                _connection.Wait();
                            }
                        }, TaskCreationOptions.LongRunning);
                    }
                }
                finally
                {
                    _connectionLock.Release();
                }
            }
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            _task.Wait(TimeSpan.FromSeconds(5));
            _connection?.Dispose();
        }
    }
}