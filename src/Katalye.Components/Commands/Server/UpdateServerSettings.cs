using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using NLog;

namespace Katalye.Components.Commands.Server
{
    public static class UpdateServerSettings
    {
        public class Command : IRequest<Result>
        {
            [Required]
            public Dictionary<string, Setting> Settings { get; set; }
        }

        public class Setting
        {
            [Required]
            public string Key { get; set; }

            [Required]
            public string Value { get; set; }

            public int? Version { get; set; }
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly KatalyeContext _context;

            public Handler(KatalyeContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Attempting to update server settings with {message.Settings.Count} values.");

                var existingSettings
                    = message.Settings.Values
                             .Where(x => x.Version.HasValue)
                             .Select(x => new ServerSetting
                             {
                                 Key = x.Key.ToLower(),
                                 Value = x.Value,
                                 Version = x.Version ?? 0,
                                 LastUpdated = DateTimeOffset.Now
                             })
                             .ToList();

                var newSettings
                    = message.Settings.Values
                             .Where(x => !x.Version.HasValue)
                             .Select(x => new ServerSetting
                             {
                                 Key = x.Key.ToLower(),
                                 Value = x.Value,
                                 LastUpdated = DateTimeOffset.Now
                             })
                             .ToList();

                _context.ServerSettings.AttachRange(existingSettings);
                _context.ServerSettings.AddRange(newSettings);

                await _context.SaveChangesAsync(cancellationToken);

                Logger.Info($"Successfully updated server settings, {existingSettings.Count} values updated, {newSettings.Count} values added.");

                return new Result();
            }
        }
    }
}