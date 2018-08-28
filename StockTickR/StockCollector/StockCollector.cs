using System;
using System.Collections.Generic;
using System.Globalization;
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
using StockDatabase.Models;

namespace StockCollector {
    public class StockCollector {
        private static short selectorStockFirstIndex = 2;
        private static short selectorStockLastIndex = 31;

        readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim (1, 1);
        private bool _updatingStockPrices = false;

        public IObservable<IEnumerable<Stock>> StocksStream (TimeSpan pauseBetweenUpdates, CancellationToken token) => Observable.Generate (StockPrices,
            stock => !token.IsCancellationRequested,
            stock => StockPrices,
            stock => stock,
            stock => pauseBetweenUpdates);

        private IEnumerable<Stock> GetStockPrices () => StockPrices;

        private IEnumerable<Stock> StockPrices {
            get {
                IEnumerable<Stock> stocks = new List<Stock> ();
                // This function must be re-entrant as it's running as a timer interval handler
                _updateStockPricesLock.WaitAsync ().GetAwaiter ().GetResult (); {
                    try {
                        if (!_updatingStockPrices) {
                            _updatingStockPrices = true;
                            stocks = UpdatedStockValues;
                            _updatingStockPrices = false;
                        }
                    } catch (Exception ex) {
                        Console.WriteLine ("[Error] " + DateTime.Now + " " + ex.Message + " :: " + ex.StackTrace);
                    } finally {
                        _updateStockPricesLock.Release ();
                    }
                }
                return stocks;
            }
        }

        public IEnumerable<Stock> UpdatedStockValues {
            get {
                string html;
                using (var httpClient = new HttpClient {
                    BaseAddress = financialUrl
                }) {
                    html = httpClient.GetStringAsync (string.Empty).GetAwaiter ().GetResult ();
                }
                var parser = new HtmlParser ();
                if (html == null) {
                    Console.WriteLine ("[Error] " + DateTime.Now + " html is null using url as baseaddress: " + financialUrl);
                }
                var document = parser.Parse (html);
                List<Stock> stocks = new List<Stock> ();
                for (int i = selectorStockFirstIndex; i <= selectorStockLastIndex; ++i) {
                    if (document == null) {
                        Console.WriteLine ("[Error] " + DateTime.Now + " document null using html " + html);
                    }
                    var stockName = document.QuerySelectorAll (string.Format (dowJonesStockNameSelector, i)).FirstOrDefault ()?.Text () ?? string.Empty;
                    if (stockName == null) {
                        Console.WriteLine ("[Error] " + DateTime.Now + " Stock name is null using css selector " + dowJonesStockNameSelector + " for url " + financialUrl);
                    }
                    double bidPrice = 0;
                    var rawBidPrice = document.QuerySelectorAll (string.Format (dowJonesBidPriceSelector, i)).FirstOrDefault ()?.Text ();
                    if (rawBidPrice == null) {
                        Console.WriteLine ("[Error] " + DateTime.Now + " Stock Price is null using css selector " + dowJonesStockNameSelector + " for url " + financialUrl);
                    } else {
                        try {
                            bidPrice = Convert.ToDouble (rawBidPrice.Trim (), CultureInfo);
                            stocks.Add (new Stock { Symbol = stockName, Price = Convert.ToDecimal (bidPrice, CultureInfo) });
                        } catch (Exception ex) {
                            Console.WriteLine ("[Error] " + DateTime.Now + " Cannot parse " + rawBidPrice.Trim () + " to double " + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
                return stocks;
            }
        }

        private Uri financialUrl;
        private static string dowJonesStockNameSelector;
        private static string dowJonesBidPriceSelector;

        public CultureInfo CultureInfo {
            get {
                string cultureIdentifier;
                if (!TryGetEnvironmentVariable ("CULTURE_IDENTIFIER", out cultureIdentifier) || string.IsNullOrEmpty (cultureIdentifier)) {
                    return CultureInfo.InvariantCulture;
                }
                return new CultureInfo (cultureIdentifier);
            }
        }

        public StockCollector () {
            var envVars = Environment.GetEnvironmentVariables ();
            foreach (string key in envVars.Keys) {
                Console.WriteLine ("[Info] " + DateTime.Now + " environment variable>" + key + " : " + (string) envVars[key]);
            }
            string raw = string.Empty;
            if (TryGetEnvironmentVariable ("DOWJONES_LISTING_URL", out raw)) {
                financialUrl = new Uri (raw);
            }
            if (TryGetEnvironmentVariable ("DOWJONES_STOCKNAME_SELECTOR", out raw)) {
                dowJonesStockNameSelector = raw;
            }
            if (TryGetEnvironmentVariable ("DOWJONES_BIDPRICE_SELECTOR", out raw)) {
                dowJonesBidPriceSelector = raw;
            }
        }

        private List<string> blackList = new List<string> ();

        private bool TryGetEnvironmentVariable (string key, out string value) {
            value = null;
            var envVars = Environment.GetEnvironmentVariables ();
            var valueRaw = (string) envVars[key];
            if (blackList.Contains (key)) {
                return false;
            }
            if (string.IsNullOrEmpty (valueRaw)) {
                Console.WriteLine (string.Format ("[Error] " + DateTime.Now + " value for {0} is missing. Please add it to the .env file.", key));
                blackList.Add (key);
                return false;
            }
            value = valueRaw;
            return true;
        }
    }
}