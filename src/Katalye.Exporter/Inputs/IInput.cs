using System;

namespace Katalye.Exporter.Inputs
{
    public interface IInput : IDisposable
    {
        bool IsListening { get; }

        void StartListening();
    }
}