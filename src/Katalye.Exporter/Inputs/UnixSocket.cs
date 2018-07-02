using System;
using System.IO;
using System.Net.Sockets;

using NLog;

namespace Katalye.Exporter.Inputs
{
    public class UnixSocket : IInput
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Socket _server;

        public bool IsListening { get; private set; }
        public string SocketPath { get; set; }
        public bool OverrideExisting { get; set; }
        public int SocketMaxBackLog { get; set; } = 100;

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
        }

        public void Dispose()
        {
            _server?.Close();
            _server?.Dispose();
        }
    }
}