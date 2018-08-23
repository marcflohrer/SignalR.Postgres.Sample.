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
using System.Globalization;

namespace StockCollector
{
    public class StockCollector
    {
        private static short selectorStockFirstIndex = 2;
        private static short selectorStockLastIndex = 31;

        readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim(1, 1);
        private bool _updatingStockPrices = false;

        public IObservable<IEnumerable<Stock>> StocksStream(TimeSpan pauseBetweenUpdates, CancellationToken token)
                                                => Observable.Generate(StockPrices,
                                                                    stock => !token.IsCancellationRequested,
                                                                    stock => StockPrices,
                                                                    stock => stock,
                                                                    stock => pauseBetweenUpdates);

        private IEnumerable<Stock> GetStockPrices() => StockPrices;

        private IEnumerable<Stock> StockPrices
        {
            get
            {
                IEnumerable<Stock> stocks = new List<Stock>();
                // This function must be re-entrant as it's running as a timer interval handler
                _updateStockPricesLock.WaitAsync().GetAwaiter().GetResult();
                {
                    try
                    {
                        if (!_updatingStockPrices)
                        {
                            _updatingStockPrices = true;
                            stocks = UpdatedStockValues;
                            _updatingStockPrices = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Error] " + ex.Message + " :: " + ex.StackTrace);
                    }
                    finally
                    {
                        _updateStockPricesLock.Release();
                    }
                }
                return stocks;
            }
        }

        public IEnumerable<Stock> UpdatedStockValues
        {
            get
            {
                string html;
                using (var httpClient = new HttpClient
                {
                    BaseAddress = financialUrl
                })
                {
                    html = httpClient.GetStringAsync(string.Empty).GetAwaiter().GetResult();
                }
                var parser = new HtmlParser();
                if(html == null){
                    Console.WriteLine("[Error] html is null using url as baseaddress: " + financialUrl );
                }
                var document = parser.Parse(html);
                List<Stock> stocks = new List<Stock>();
                for (int i = selectorStockFirstIndex; i <= selectorStockLastIndex; ++i)
                {
                    if(document == null)
                    {
                        Console.WriteLine("[Error] document null using html " + html );
                    }
                    var stockName = document.QuerySelectorAll(string.Format(dowJonesStockNameSelector, i)).FirstOrDefault()?.Text() ?? string.Empty;
                    if(stockName == null)
                    {
                        Console.WriteLine("[Error] Stock name is null using css selector " + dowJonesStockNameSelector + " for url " + financialUrl);
                    }
                    double bidPrice = 0;
                    var rawBidPrice = document.QuerySelectorAll(string.Format(dowJonesBidPriceSelector, i)).FirstOrDefault()?.Text();
                    if(rawBidPrice == null)
                    {
                        Console.WriteLine("[Error] Stock Price is null using css selector " + dowJonesStockNameSelector + " for url " + financialUrl);
                    }
                    else
                    {   
                        bidPrice = Convert.ToDouble(rawBidPrice.Trim(), CultureInfo);
                        stocks.Add(new Stock { Symbol = stockName, Price = Convert.ToDecimal(bidPrice, CultureInfo)});                                                
                    }
                }
                return stocks;
            }
        }

        private Uri financialUrl;
        private static string dowJonesStockNameSelector;// = "#realtime_chart_list > div.table-responsive.relative > table > tbody > tr:nth-child({0}) > td:nth-child(2) > a:nth-child(2)";
        private static string dowJonesBidPriceSelector;// = "#realtime_chart_list > div.table-responsive.relative > table > tbody > tr:nth-child({0}) > td:nth-child(5) > div > span";

        public CultureInfo CultureInfo
        {
            get
            {
                string cultureIdentifier;
                if (!TryGetEnvironmentVariable("CULTURE_IDENTIFIER", out cultureIdentifier) || string.IsNullOrEmpty(cultureIdentifier))
                {
                    return CultureInfo.InvariantCulture;
                }
                return new CultureInfo(cultureIdentifier);
            }
        }

        public StockCollector()
        {
            var envVars = Environment.GetEnvironmentVariables();
            foreach (string key in envVars.Keys)
            {
                Console.WriteLine("[Info] environment variable>" + key + " : " + (string)envVars[key]);
            }
            string raw = string.Empty;
            if(TryGetEnvironmentVariable("DOWJONES_LISTING_URL", out raw))
            {
                financialUrl = new Uri(raw);
            }
            if(TryGetEnvironmentVariable("DOWJONES_STOCKNAME_SELECTOR", out raw))
            {
                dowJonesStockNameSelector = raw;
            }
            if(TryGetEnvironmentVariable("DOWJONES_BIDPRICE_SELECTOR", out raw))
            {
                dowJonesBidPriceSelector = raw;
            }
        }

        private bool TryGetEnvironmentVariable(string key, out string value){
            value = null;
            var envVars = Environment.GetEnvironmentVariables();
            var valueRaw = (string)envVars[key];
            if(string.IsNullOrEmpty(valueRaw)){
                Console.WriteLine($"[Error] value for {key} is null or empty! ({valueRaw})");
                return false;
            }
            value = valueRaw;
            return true;
        }
    }
}
