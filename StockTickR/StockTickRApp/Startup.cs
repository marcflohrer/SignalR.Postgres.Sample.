using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTickR.Hubs;
using StockTickR.Repositories;
using StockTickR.Repositories.Core;

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
                .Build();

            HostingEnvironment = env;
        }

        public static IConfigurationRoot Configuration { get; private set; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var connectionString = $"Server=stocktickr_database_1:db;Port=5432;Database={Configuration["POSTGRES_DB"]};User Id={Configuration["POSTGRES_USER"]};Password={Configuration["POSTGRES_PASSWORD"]};";
            var sqlConnectionString = $"User ID=damienbod;Password=damienbod;Host=database;Port=5432;Database=damienbod;Pooling=true;";
            
            //Add PostgreSQL support
            services.AddEntityFrameworkNpgsql()
            .AddDbContext<StockDbContext>(
                options => options.UseNpgsql(sqlConnectionString)
            );

            services.AddScoped<IStockRepository, StockRepository>();

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

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (!serviceScope.ServiceProvider.GetService<StockDbContext>().AllMigrationsApplied())
                {
                    serviceScope.ServiceProvider.GetService<StockDbContext>().Database.Migrate();
                }
            }

            app.UseFileServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<StockTickerHub>("/stocks");
            });
        }
    }
}