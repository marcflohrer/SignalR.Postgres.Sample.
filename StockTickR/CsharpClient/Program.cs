using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CsharpClient
{
    public static class Program
    {
#pragma warning disable RECS0154 // Parameter wird niemals verwendet.
        static async Task Main(string[] args)
#pragma warning restore RECS0154 // Parameter wird niemals verwendet.
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://stocktickr:8081/stocks")
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .AddMessagePackProtocol()
                .Build();

            await connection.StartAsync();

            Console.WriteLine("[Info] Starting connection. Press Ctrl-C to close.");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            connection.Closed += e =>
            {
                Console.WriteLine("[Info] Connection closed with error: {0}", e);

                cts.Cancel();
                return Task.CompletedTask;
            };


            connection.On("marketOpened", () =>
            {
                Console.WriteLine("[Info] Market opened");
            });

            connection.On("marketClosed", () =>
            {
                Console.WriteLine("[Info] Market closed");
            });

            connection.On("marketReset", () =>
            {
                // We don't care if the market rest
            });

            var channel = await connection.StreamAsChannelAsync<Stock>("StreamStocks", CancellationToken.None);
            while (await channel.WaitToReadAsync() && !cts.IsCancellationRequested)
            {
                while (channel.TryRead(out var stock))
                {
                    Console.WriteLine($"[Info] {stock.Symbol} {stock.Price}");
                }
            }
        }
    }

    public class Stock
    {
        public string Symbol { get; set; }

        public decimal DayOpen { get; private set; }

        public decimal DayLow { get; private set; }

        public decimal DayHigh { get; private set; }

        public decimal LastChange { get; private set; }

        public decimal Change { get; set; }

        public double PercentChange { get; set; }

        public decimal Price { get; set; }
    }
}
