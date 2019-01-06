using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Server;
using NLog;

namespace Katalye.Components.Processing
{
    public abstract class ProcessingServer : IBackgroundProcess
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public abstract TimeSpan Interval { get; }

        public abstract Task Process(CancellationToken cancellationToken);

        public void Execute(BackgroundProcessContext context)
        {
            Logger.Info($"Beginning execution of processing server {GetType()} with a execution interval of {Interval}.");

            while (!context.IsShutdownRequested)
            {
                try
                {
                    Logger.Info("Processing execution in-progress.");

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Process(context.CancellationToken).GetAwaiter().GetResult();
                    stopwatch.Stop();

                    Logger.Info($"Processing executed successfully in {stopwatch.ElapsedMilliseconds}ms.");

                    Logger.Info($"Processing server will now sleep for {Interval}.");
                    context.Wait(Interval);
                }
                catch (Exception e)
                {
                    Logger.Warn(e, "An error occured while processing, but was handled by the processing server. The processing server will restart.");
                    throw;
                }
            }
        }
    }
}