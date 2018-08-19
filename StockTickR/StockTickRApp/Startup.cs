using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTickR.Hubs;
using StockTickR.Repositories;
using StockTickR.Repositories.Core;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace StockTickR
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                .AddEnvironmentVariables()
                .Build();

            HostingEnvironment = env;
        }

        public static IConfigurationRoot Configuration { get; private set; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            //Add PostgreSQL support
            services.AddEntityFrameworkNpgsql().AddDbContext<StockDbContext>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddSingleton<IConfigurationRoot>(Configuration);

            // Add framework services.
            services.AddMvc();

            services.AddSignalR()
                    .AddMessagePackProtocol();

            services.AddSingleton<StockTicker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<StockTickerHub>("/stocks");
            });
        }
    }
}