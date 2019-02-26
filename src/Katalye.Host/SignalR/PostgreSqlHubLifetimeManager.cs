using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NLog;
using Npgsql;

namespace Katalye.Host.SignalR
{
    public class PostgreSqlHubLifetimeManager<THub> : HubLifetimeManager<THub>, IDisposable where THub : Hub
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Func<NpgsqlConnection> _connectionFactory;
        private readonly HubConnectionStore _connections = new HubConnectionStore();
        // ReSharper disable once NotAccessedField.Local
        private readonly PostgreSqlOptions _options;
        private readonly PostgreSqlChannels _channels;
        private readonly PostgreSqlProtocol _protocol;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1);

        private PostgreSqlBus _bus;

        public PostgreSqlHubLifetimeManager(IOptions<PostgreSqlOptions> options, IHubProtocolResolver hubProtocolResolver, Func<NpgsqlConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _options = options.Value;
            _channels = new PostgreSqlChannels(typeof(THub).FullName);
            _protocol = new PostgreSqlProtocol(hubProtocolResolver.AllProtocols);

            _logger.Info("Connecting to endpoints.");
            _ = EnsureRedisServerConnection();
        }

        public override async Task OnConnectedAsync(HubConnectionContext connection)
        {
            await EnsureRedisServerConnection();

            var connectionTask = Task.CompletedTask;
            var userTask = Task.CompletedTask;

            _connections.Add(connection);

            await Task.WhenAll(connectionTask, userTask);
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            _connections.Remove(connection);

            return Task.CompletedTask;
        }

        public override async Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = _protocol.WriteInvocation(methodName, args);
            await EnsureRedisServerConnection();
            _logger.Info($"PublishToChannel {_channels.All}.");
            await _bus.PublishAsync(_channels.All, message);
        }

        public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendConnectionAsync(string connectionId, string methodName, object[] args,
                                                 CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendGroupAsync(string groupName, string methodName, object[] args,
                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds,
                                                  CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args,
                                                  CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args,
                                             CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args,
                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _bus?.Dispose();
        }

        private async Task SubscribeToAll()
        {
            _logger.Info("Subscribing All.");
            var channel = await _bus.SubscribeAsync(_channels.All);
            channel.OnMessage(async channelMessage =>
            {
                try
                {
                    _logger.Info("ReceivedFromChannel All.");

                    var invocation = _protocol.ReadInvocation(channelMessage.Message);

                    var tasks = new List<Task>(_connections.Count);

                    foreach (var connection in _connections)
                    {
                        tasks.Add(connection.WriteAsync(invocation.Message).AsTask());
                    }

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.Info(ex, "FailedWritingMessage.");
                }
            });
        }

        private async Task EnsureRedisServerConnection()
        {
            if (_bus == null)
            {
                await _connectionLock.WaitAsync();
                try
                {
                    if (_bus == null)
                    {
                        var connection = _connectionFactory.Invoke();
                        await connection.OpenAsync();

                        _bus = new PostgreSqlBus(_connectionFactory);

                        //_redisServerConnection.ConnectionRestored += (_, e) =>
                        //{
                        //    // We use the subscription connection type
                        //    // Ignore messages from the interactive connection (avoids duplicates)
                        //    if (e.ConnectionType == ConnectionType.Interactive)
                        //    {
                        //        return;
                        //    }

                        //    Logger.Info($"ConnectionRestored.");
                        //};

                        //_redisServerConnection.ConnectionFailed += (_, e) =>
                        //{
                        //    // We use the subscription connection type
                        //    // Ignore messages from the interactive connection (avoids duplicates)
                        //    if (e.ConnectionType == ConnectionType.Interactive)
                        //    {
                        //        return;
                        //    }

                        //    Logger.Info(e.Exception, $"ConnectionFailed.");
                        //};

                        //if (_redisServerConnection.IsConnected)
                        //{
                        //    Logger.Info($"Connected.");
                        //}
                        //else
                        //{
                        //    Logger.Info($"NotConnected.");
                        //}

                        await SubscribeToAll();
                    }
                }
                finally
                {
                    _connectionLock.Release();
                }
            }
        }
    }
}