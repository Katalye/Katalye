using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using JetBrains.Annotations;
using Katalye.Api.Controllers;
using Katalye.Data;
using Katalye.Host.Lamar;
using Katalye.Host.Middleware;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireContext")));

            services.IncludeRegistry<MediatrRegistry>();
            services.IncludeRegistry<KatalyeRegistry>();
            services.IncludeRegistry<RestEaseRegistry>();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<AspNetCoreLoggingFilter>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                FilterProvider = new JobFilterCollection
                {
                    app.ApplicationServices.GetService<HangfireLoggingFilter>()
                }
            });
            app.UseHangfireDashboard();
        }
    }
}