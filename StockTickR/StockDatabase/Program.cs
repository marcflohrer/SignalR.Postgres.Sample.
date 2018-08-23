using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockDatabase.Repositories;
using StockDatabase.Repositories.Core;

namespace StockDatabase
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args).Build().Seed().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                    })
                    .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                    .UseStartup<Startup>();
    }

    public static class WebHostExtension
    {
        public static IWebHost Seed(this IWebHost webhost)
        {
            // alternatively resolve UserManager instead and pass that if only think you want to seed are the users     
            var stockDbContext = webhost.Services.GetService<IServiceScopeFactory>()
                                        .CreateScope()
                                        .ServiceProvider
                                        .GetRequiredService<StockDbContext>();
            if (!stockDbContext.AllMigrationsApplied())
            {
                stockDbContext.Database.Migrate();
            }
            return webhost;
        }
    }
}
