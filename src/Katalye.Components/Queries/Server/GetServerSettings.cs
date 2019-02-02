using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Configuration;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

            public string EffectiveValue { get; set; }

            public string DbValue { get; set; }

            public string Provider { get; set; }

            public int? Version { get; set; }

            public DateTimeOffset? LastUpdated { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly KatalyeContext _context;
            private readonly ConfigurationRenderer _configurationRenderer;

            public Handler(KatalyeContext context, ConfigurationRenderer configurationRenderer)
            {
                _context = context;
                _configurationRenderer = configurationRenderer;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var effectiveSettings = _configurationRenderer.RenderSettings();

                var dbSettings = await (from setting in _context.ServerSettings
                                        select new
                                        {
                                            setting.Value,
                                            setting.Key,
                                            setting.Version,
                                            setting.LastUpdated
                                        }).ToListAsync(cancellationToken);

                var settings = from effectiveSetting in effectiveSettings
                               from dbSetting in dbSettings.Where(x => x.Key == effectiveSetting.Key).DefaultIfEmpty()
                               select new Setting
                               {
                                   Key = effectiveSetting.Key,
                                   DbValue = dbSetting?.Value,
                                   EffectiveValue = effectiveSetting.Value,
                                   Version = dbSetting?.Version,
                                   LastUpdated = dbSetting?.LastUpdated,
                                   Provider = effectiveSetting.Provider
                               };

                return new Result
                {
                    Settings = settings.ToDictionary(x => x.Key, x => x)
                };
            }
        }
    }
}