using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Server;
using Katalye.Components.Configuration;
using NLog;

namespace Katalye.Components.Processing
{
    public abstract class ProcessingServer : IBackgroundProcess
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKatalyeConfiguration _configuration;

        public abstract TimeSpan Interval { get; }

        public virtual bool RequiresDistributedLock => true;
        public virtual string Name => GetType().Name;

        public abstract Task Process(CancellationToken cancellationToken);

        public ProcessingServer(IKatalyeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Execute(BackgroundProcessContext context)
        {
            Logger.Info($"Beginning execution of processing server {Name} with a execution interval of {Interval}.");

            var rock = AcquireDistributedLock(context);
            try
            {
                while (!context.IsShutdownRequested)
                {
                    Process(context);
                }
            }
            finally
            {
                rock?.Dispose();
            }
        }

        private IDisposable AcquireDistributedLock(BackgroundProcessContext context)
        {
            if (!RequiresDistributedLock || _configuration.DisableDistributedLocks)
            {
                Logger.Debug("Distibuted lock is not reqired, none will be aquired.");
                return null;
            }

            Logger.Info("Attemping to aquire distibuted lock.");
            var connection = context.Storage.GetConnection();
            var rock = connection.AcquireDistributedLock(Name, TimeSpan.FromHours(24));
            Logger.Info("Distibuted lock achived.");
            return rock;
        }

        private void Process(BackgroundProcessContext context)
        {
            try
            {
                Logger.Info("Processing execution in-progress.");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Process(context.CancellationToken).GetAwaiter().GetResult();
                stopwatch.Stop();

                Logger.Info($"Processing executed successfully in {stopwatch.ElapsedMilliseconds}ms.");

                var localInterval = Interval;
                Logger.Info($"Processing {Name} server will now sleep for {localInterval}.");

                var endTime = DateTimeOffset.Now;
                while (DateTimeOffset.Now - endTime < localInterval)
                {
                    if (localInterval != Interval)
                    {
                        localInterval = Interval;
                        Logger.Info($"Processing server {Name} interval was changed to {localInterval}.");
                    }

                    context.Wait(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, "An error occured while processing, but was handled by the processing server. The processing server will restart.");
                throw;
            }
        }
    }
}