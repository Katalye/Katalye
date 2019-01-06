using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Components.Common;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Katalye.Components.Notifications.Handlers
{
    [UsedImplicitly]
    public class GrainsRefreshed : INotificationHandler<JobReturnSaved.Notification>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly KatalyeContext _context;
        private readonly IBackgroundJobClient _jobClient;
        private readonly JsonFlattener _jsonFlattener;

        public GrainsRefreshed(KatalyeContext context, IBackgroundJobClient jobClient, JsonFlattener jsonFlattener)
        {
            _context = context;
            _jobClient = jobClient;
            _jsonFlattener = jsonFlattener;
        }

        public Task Handle(JobReturnSaved.Notification notification, CancellationToken cancellationToken)
        {
            if (notification.Function == "grains.items")
            {
                Logger.Info($"Found grains.item job return for {notification.MinionReturnEventId}, will queue job to process this event.");
                _jobClient.Enqueue<GrainsRefreshed>(x => x.Process(notification));
            }

            return Task.CompletedTask;
        }

        [UsedImplicitly]
        public async Task Process(JobReturnSaved.Notification notification)
        {
            Logger.Info($"Processing grains return for {notification.MinionReturnEventId}.");

            using (var unit = await _context.Database.BeginTransactionAsync())
            {
                var bulk = await (from returnEvent in _context.MinionReturnEvents.Where(x => x.Id == notification.MinionReturnEventId)
                                  let m = returnEvent.Minion
                                  select new
                                  {
                                      ReturnEvent = returnEvent,
                                      MinionId = m.Id,
                                      MinionVersion = m.Version,
                                      MinionLastGrainRefresh = m.LastGrainRefresh
                                  }).SingleOrDefaultAsync();

                var grainsAreStale = bulk.MinionLastGrainRefresh >= bulk.ReturnEvent.Timestamp;
                if (grainsAreStale)
                {
                    Logger.Info("Grain data is more stale than existing grain data, will ignore return event.");
                    return;
                }

                var minion = new Minion
                {
                    Id = bulk.MinionId
                };
                _context.Attach(minion);

                var generation = Guid.NewGuid();
                var grainsJson = bulk.ReturnEvent.ReturnData;
                var grains = _jsonFlattener.Flatten(grainsJson);

                var minionGrains = grains.Select(x => new MinionGrain
                {
                    MinionId = bulk.MinionId,
                    Generation = generation,
                    Path = x.Key,
                    Values = x.Value.ToList(),
                    Timestamp = bulk.ReturnEvent.Timestamp
                });

                _context.MinionGrains.AddRange(minionGrains);
                minion.Version = bulk.MinionVersion;
                minion.GrainGeneration = generation;
                minion.LastGrainRefresh = bulk.ReturnEvent.Timestamp;

                await _context.SaveChangesAsync();

                unit.Commit();

                Logger.Debug($"Commited {grains.Count} grains under generation {generation}");
            }
        }
    }
}