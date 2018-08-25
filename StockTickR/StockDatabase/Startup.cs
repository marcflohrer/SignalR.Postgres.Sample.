using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Enrichers.AspNetCore.HttpContext;
using Serilog.Events;
using StockDatabase.Repositories;
using StocksDatabase.Controllers;

namespace StockDatabase {
    public class Startup {
        public Startup (IHostingEnvironment env) {
            Configuration = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", true, true)
                .AddJsonFile ($"appsettings.{env.EnvironmentName}.json")
                .AddEnvironmentVariables ()
                .Build ();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Log.Logger = new LoggerConfiguration ()
                .ReadFrom.Configuration (Configuration.GetSection ("Logging"))
                .Enrich.FromLogContext ()
                .Enrich.WithProperty ("Environment", HostingEnvironment.EnvironmentName)
                .WriteTo.Console (outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {Properties}{NewLine}{Exception}{NewLine}")
                .CreateLogger ();

            services.AddSingleton (Log.Logger);
            services.AddLogging (loggingBuilder => loggingBuilder.AddSerilog (dispose: true));

            services.AddEntityFrameworkNpgsql ().AddDbContext<StockDbContext> ();
            services.AddScoped<IStockRepository, StockRepository> ();
            services.AddScoped<IUnitOfWork, UnitOfWork> ();
            services.AddScoped<StocksController> ();
            services.AddSingleton (Configuration);

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions (o => {
                    o.Conventions.ConfigureFilter (new IgnoreAntiforgeryTokenAttribute ());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerfactory,
            IApplicationLifetime appLifetime) {
            loggerfactory.AddConsole (Configuration.GetSection ("Logging"))
                .AddDebug ()
                .AddSerilog ();

            app.UseSerilogLogContext (options => {
                options.EnrichersForContextFactory = context => new [] {
                // TraceIdentifier property will be available in all chained middlewares. And yes - it is HttpContext specific
                new PropertyEnricher ("TraceIdentifier", context.TraceIdentifier)
                };
            });

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register (Log.CloseAndFlush);

            app.UseStatusCodePagesWithReExecute ("/Home/Error", "?statusCode={0}");

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Error");
                //app.UseHsts();
            }

            app.UseStaticFiles ();
            //app.UseCookiePolicy();
            app.UseMvc ();
        }
    }
}