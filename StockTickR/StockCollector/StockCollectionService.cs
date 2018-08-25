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
            var observable = stockCollector.StocksStream (TimeSpan.FromSeconds (10), stoppingToken)
                .Where (stocks => stocks.Count () > 0)
                .Do (stocks => new StockClient ().AddRange (stocks))
                .Do (stocks => Console.WriteLine ("Apple: " + stocks.FirstOrDefault (stock => stock.Symbol == "Apple").Price))
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