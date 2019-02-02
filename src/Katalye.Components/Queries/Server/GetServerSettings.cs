using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Katalye.Components.Queries.Server
{
    public static class GetServerSettings
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public Dictionary<string, Setting> Settings { get; set; }
        }

        public class Setting
        {
            public string Key { get; set; }

            public string Value { get; set; }

            public int? Version { get; set; }

            public bool Overridden { get; set; }

            public DateTimeOffset? LastUpdated { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly KatalyeContext _context;
            private readonly IConfiguration _configuration;

            public Handler(KatalyeContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var dbSettings = await (from setting in _context.ServerSettings
                                        select new
                                        {
                                            setting.Value,
                                            setting.Key,
                                            setting.Version,
                                            setting.LastUpdated
                                        }).ToListAsync(cancellationToken);

                var settings = dbSettings.ToDictionary(x => x.Key, x => new Setting
                {
                    Key = x.Key,
                    Value = x.Value,
                    Version = x.Version,
                    LastUpdated = x.LastUpdated,
                    Overridden = false
                });

                var overriddenSettings = _configuration.AsEnumerable()
                                                       .Select(x => new {Key = x.Key.ToLower(), x.Value})
                                                       .Where(x => x.Key.StartsWith("katalye:"));
                foreach (var overriddenSetting in overriddenSettings)
                {
                    var existing = settings.ContainsKey(overriddenSetting.Key);
                    if (existing)
                    {
                        var setting = settings[overriddenSetting.Key];
                        setting.Value = overriddenSetting.Value;
                        setting.Overridden = true;
                    }
                    else
                    {
                        settings.Add(overriddenSetting.Key, new Setting
                        {
                            Key = overriddenSetting.Key,
                            Value = overriddenSetting.Value,
                            Overridden = true
                        });
                    }
                }

                return new Result
                {
                    Settings = settings
                };
            }
        }
    }
}