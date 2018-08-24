using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.HttpContextData;
using StockTickR.Clients;
using StockTickR.Hubs;

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
            Log.Logger = new LoggerConfiguration()
                                    .ReadFrom.Configuration(Configuration.GetSection("Logging"))
                                    .Enrich.FromLogContext()
                                    .Enrich.WithProperty("Environment", HostingEnvironment.EnvironmentName)
                                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {Properties}{NewLine}{Exception}{NewLine}")
                                    .Enrich.FromLogContext()
                                    .Enrich.WithHttpContextData()
                                    .WriteTo.Console()
                                    .CreateLogger();
                                    
            services.AddSingleton(Configuration);
            services.AddSingleton<StockClient>();

            // Add framework services.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddRazorPagesOptions(o =>
                    {
                        o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                    });

            services.AddSignalR()
                    .AddMessagePackProtocol();

            services.AddSingleton<StockTicker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

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