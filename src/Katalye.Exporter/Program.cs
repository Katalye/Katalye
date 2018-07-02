using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Katalye.Exporter.Inputs;

using McMaster.Extensions.CommandLineUtils;

using NLog;

namespace Katalye.Exporter
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        [UsedImplicitly]
        private async Task OnExecuteAsync()
        {
            Logger.Info("Preparing the exporter server.");

            try
            {
                using (var server = new ExporterServer())
                {
                    var input = new UnixSocket("/socket");
                    server.Start(input);

                    Logger.Debug("The server is ready.");
                    Wait();

                    Logger.Debug("The server is shutting down.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "An unhandled error occured during server execution.");
            }

            Logger.Debug("The server has exited.");
        }

        private void Wait()
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
        }
    }
}