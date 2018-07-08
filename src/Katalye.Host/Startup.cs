using System;

using JetBrains.Annotations;

using Katalye.Api.Controllers;
using Katalye.Data;
using Katalye.Host.StructureMap;

using MediatR.StructureMap;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StructureMap;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddApplicationPart(typeof(PingController).Assembly);

            var connectionString = Configuration.GetConnectionString(nameof(KatalyeContext));
            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<KatalyeContext>(options => options.UseNpgsql(connectionString));

            var container = ConfigureStructureMap();
            container.Populate(services);

            return container.GetInstance<IServiceProvider>();
        }

        private Container ConfigureStructureMap()
        {
            var container = new Container();

            container.Configure(config => { config.AddRegistry<MediatrRegistry>(); });

            container.UseMediatR();

            return container;
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
        }
    }
}