using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace Katalye.Exporter.Inputs
{
    public class UnixSocket : IInput
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Socket _server;

        public bool IsListening { get; private set; }
        public string SocketPath { get; }
        public bool OverrideExisting { get; set; } = true;
        public int SocketMaxBackLog { get; set; } = 100;

        public UnixSocket(string socketPath)
        {
            SocketPath = socketPath;
        }

        public void StartListening()
        {
            if (IsListening)
            {
                throw new InvalidOperationException($"Cannot call {nameof(StartListening)} multiple times.");
            }

            Logger.Debug($"Creating a new unix socket at the path {SocketPath}.");
            _server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            if (OverrideExisting && File.Exists(SocketPath))
            {
                Logger.Info($"Socket or file at path {SocketPath} exists and override is enabled, will delete.");
                File.Delete(SocketPath);
            }

            Logger.Debug("Binding socket endpoint.");
            var endPoint = new UnixDomainSocketEndPoint(SocketPath);
            _server.Bind(endPoint);

            Logger.Debug($"Socket entering listening mode with a max backlog of {SocketMaxBackLog}.");
            _server.Listen(SocketMaxBackLog);

            Logger.Debug("Unix socket is ready for connections.");
            IsListening = true;

            Task.Factory.StartNew(() => AcceptLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task AcceptLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Logger.Debug("Accepting new sockets.");
                var socket = await _server.AcceptAsync();

                Logger.Debug("New socket accepted, will create a new task to handle it.");
#pragma warning disable 4014
                Task.Run(() => RecieveLoop(socket, token), token);
#pragma warning restore 4014
            }
        }

        private async Task RecieveLoop(Socket socket, CancellationToken token)
        {
            var stream = new NetworkStream(socket, true);
            var reader = new StreamReader(stream);
            while (socket.Connected && !token.IsCancellationRequested)
            {
                var output = await reader.ReadLineAsync();
                Logger.Info($"GOT: [{output}].");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _server?.Close();
            _server?.Dispose();
        }
    }
}