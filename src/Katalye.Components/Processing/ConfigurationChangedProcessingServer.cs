using System;
using System.Threading;
using System.Threading.Tasks;
using Katalye.Components.Commands;
using Katalye.Components.Common;
using Katalye.Data;
using MediatR;
using NLog;

namespace Katalye.Components.Processing
{
    public class ConfigurationChangedProcessingServer : ProcessingServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDistributedNotify _notify;
        private readonly IMediator _mediator;

        public override TimeSpan Interval { get; } = TimeSpan.FromSeconds(5);

        public override bool RequiresDistributedLock => false;

        public ConfigurationChangedProcessingServer(IDistributedNotify notify, IMediator mediator)
        {
            _notify = notify;
            _mediator = mediator;
        }

        public override async Task Process(CancellationToken cancellationToken)
        {
            Logger.Info("Monitoring for configuration updates from siblings.");
            await _mediator.Send(new UpdateLocalDbConfiguration.Command(), cancellationToken);

            await _notify.WaitOnce(ChannelConstants.ConfigurationChangedChannel, cancellationToken);
            Logger.Info("A sibling server published a configuration change notifcation, "
                        + "will update local configuration cache within 10 seconds.");
        }
    }
}