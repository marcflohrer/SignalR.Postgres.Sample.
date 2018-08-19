// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockTickR.Repositories;
using StockTickR.Repositories.Core;

namespace StockTickR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build()
                                                 .Seed();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:8081");
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
