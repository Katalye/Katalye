using System;

using Katalye.Exporter.Inputs;

using NLog;

namespace Katalye.Exporter
{
    public class ExporterServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IInput _input;

        public void Start<T>(T input) where T : IInput
        {
            _input = input;
            Logger.Debug($"Using input of type {input.GetType()}.");

            _input.StartListening();
        }

        public void Dispose()
        {
            _input.Dispose();
        }
    }
}