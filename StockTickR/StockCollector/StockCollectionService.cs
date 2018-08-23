using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using StockDatabase.Models;
using AngleSharp.Parser.Html;
using AngleSharp.Extensions;
using System.Linq;
using System.Threading;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Microsoft.Extensions.Hosting;
using StockTickR.Clients;

namespace StockCollector
{
    public class StockCollectionService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stockCollector = new StockCollector();
            var observable = stockCollector.StocksStream(TimeSpan.FromSeconds(10), stoppingToken)
                                           .Where(stocks => stocks.Count() > 0)
                                           .Do(stocks => new StockClient().AddRange(stocks))
                                           .Catch<IEnumerable<Stock>, Exception>(ex =>
                                           {
                                               Console.WriteLine(DateTime.Now + " Catch: " + ex.Message + " : " + ex.StackTrace);
                                               return Observable.Empty<IEnumerable<Stock>>();
                                           });
            using (var stocks = observable.Subscribe())
            {
                Console.WriteLine(DateTime.Now + " Press any key to unsubscribe");
                observable.Wait();
            }
            return Task.CompletedTask;
        }
    }
}