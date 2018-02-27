using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Frontend.Data;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;

namespace Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var redis = ConnectionMultiplexer.Connect(Configuration.GetSection("cache").GetValue<string>("redis"));

            services.AddDataProtection()
                .SetApplicationName("frontend-test")
                .PersistKeysToRedis(redis);

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "SampleInstance";
                options.Configuration = Configuration.GetSection("cache").GetValue<string>("redis");
            });

            services.AddSingleton<IPeopleService>(x => new PeopleService(
                Configuration.GetSection("services").GetValue<string>("people"),
                x.GetService<IDistributedCache>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
