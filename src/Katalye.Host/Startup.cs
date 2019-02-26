using System.Linq;
using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using JetBrains.Annotations;
using Katalye.Api.Controllers;
using Katalye.Api.Hubs;
using Katalye.Components.Processing;
using Katalye.Data;
using Katalye.Host.Lamar;
using Katalye.Host.Middleware;
using Katalye.Host.SignalR;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Katalye.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [UsedImplicitly]
        public void ConfigureContainer(ServiceRegistry services)
        {
            // Supports ASP.Net Core DI abstractions
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    //.AddJsonOptions(options => { options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error; })
                    .AddApplicationPart(typeof(PingController).Assembly);

            var connectionString = Configuration.GetConnectionString(nameof(KatalyeContext));
            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<KatalyeContext>(options => options.UseNpgsql(connectionString));

            services.AddSignalR()
                    .AddPostgreSql();

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(Configuration.GetConnectionString(nameof(KatalyeContext))));

            services.IncludeRegistry<MediatrRegistry>();
            services.IncludeRegistry<KatalyeRegistry>();
            services.IncludeRegistry<RestEaseRegistry>();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<AspNetCoreLoggingFilter>();
            app.EnsureMigrationOfContext<KatalyeContext>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
            app.UseSignalR(builder => builder.MapHub<EventsHub>("/hub/v1/events", options => options.Transports = HttpTransportType.WebSockets));

            var processingServers = app.ApplicationServices
                                       .GetServices<ProcessingServer>()
                                       .ToList();
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                FilterProvider = new JobFilterCollection
                {
                    app.ApplicationServices.GetService<HangfireLoggingFilter>()
                }
            }, processingServers);
            app.UseHangfireDashboard();
        }
    }
}