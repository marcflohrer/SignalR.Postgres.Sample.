using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Microsoft.Extensions.Hosting;
using StockDatabase.Models;
using StockTickR.Clients;

namespace StockCollector {
    public class StockCollectionService : BackgroundService {
        protected override Task ExecuteAsync (CancellationToken stoppingToken) {
            var stockCollector = new StockCollector ();
            var stockClient = new StockClient ();
            var observable = stockCollector.StocksStream (TimeSpan.FromSeconds (3), stoppingToken)
                .Where (stocks => stocks.Any ())
                .Do (stocks => stockClient.AddRange (stocks))
                .Catch<IEnumerable<Stock>, Exception> (ex => {
                    Console.WriteLine ("[Error] " + DateTime.Now + " Catch: " + ex.Message + " : " + ex.StackTrace);
                    return Observable.Empty<IEnumerable<Stock>> ();
                });
            using (var stocks = observable.Subscribe ()) {
                Console.WriteLine (DateTime.Now + " Press any key to unsubscribe");
                observable.Wait ();
            }
            return Task.CompletedTask;
        }
    }
}