using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using StockDatabase.Models;

namespace StockTickR.Clients {
    public class StockClient {
        private readonly HttpClient _client;
        MediaTypeWithQualityHeaderValue _mediaType = new MediaTypeWithQualityHeaderValue ("application/json");
        private Dictionary<string, decimal> cache = new Dictionary<string, decimal> ();
        public StockClient () {
            _client = new HttpClient {
                BaseAddress = new Uri ("http://stockdatabase:8082/")
            };
            _client.DefaultRequestHeaders.Accept.Clear ();
            _client.DefaultRequestHeaders.Accept.Add (_mediaType);
        }

        public IEnumerable<Stock> Get () {
            var response = _client.GetAsync ("stocks/").GetAwaiter ().GetResult ();
            response.EnsureSuccessStatusCode ();
            return response.Content.ReadAsAsync<List<Stock>> ().GetAwaiter ().GetResult ();
        }

        public HttpStatusCode Add (Stock stock) {
            if (IsChanged (stock)) {
                var response = _client.PostAsJsonAsync ("stocks/" + stock.Id, stock).GetAwaiter ().GetResult ();
                Console.WriteLine (DateTime.Now + " Post /stocks: " + stock.Symbol + ":" + stock.Price + ", " + response.Content.ReadAsStringAsync ().GetAwaiter ().GetResult ());
                response.EnsureSuccessStatusCode ();
                return response.StatusCode;
            } else {
                return HttpStatusCode.OK;
            }
        }

        public HttpStatusCode AddRange (IEnumerable<Stock> stocks) {
            List<Stock> stocksThatChanged = FindStocksThatChanged (stocks);
            if (stocksThatChanged.Any ()) {
                HttpResponseMessage response = null;
                response = PostAsJsonUntilDbIsReadyAsync (stocksThatChanged, response);
                if (stocksThatChanged.FirstOrDefault (s => s.Symbol == "Apple") != null) {
                    Console.WriteLine (DateTime.Now + " [Post] /stocks: count=" +
                        stocksThatChanged.ToList ().Count +
                        " stocks (i.e. Apple:" + stocksThatChanged.FirstOrDefault (s => s.Symbol == "Apple")?.Price + ")");
                }
                response.EnsureSuccessStatusCode ();
                UpdateCache (stocksThatChanged);
                return response.StatusCode;
            } else {
                return HttpStatusCode.OK;
            }
        }

        private HttpResponseMessage PostAsJsonUntilDbIsReadyAsync (List<Stock> stocksThatChanged, HttpResponseMessage response) {
            while (response == null || response.StatusCode != HttpStatusCode.OK) {
                try {
                    response = _client.PostAsJsonAsync ("stocks/", stocksThatChanged).GetAwaiter ().GetResult ();
                    if (response.StatusCode != HttpStatusCode.OK) {
                        Console.WriteLine (DateTime.Now + " [Error] StockClient.AddRange: " + response.Content + " : " + response.StatusCode);
                    }
                } catch (Exception ex) {
                    Console.WriteLine (DateTime.Now + " [Error] StockClient.AddRange: " + ex.Message + "\n" + ex.StackTrace);
                    WaitForMsSqlServerToBoot ();
                }
            }

            return response;
        }

        private static void WaitForMsSqlServerToBoot () {
            Thread.Sleep (20000);
        }

        private List<Stock> FindStocksThatChanged (IEnumerable<Stock> stocks) {
            var stocksThatChanged = new List<Stock> ();

            foreach (var stock in stocks) {
                if (IsChanged (stock)) {
                    stocksThatChanged.Add (stock);
                }
            }

            return stocksThatChanged;
        }

        private void UpdateCache (IEnumerable<Stock> stocks) {
            foreach (var stock in stocks) {
                while (cache.ContainsKey (stock.Symbol)) {
                    try {
                        cache.Remove (stock.Symbol);
                    } catch (Exception ex) {
                        Console.WriteLine (DateTime.Now + " [Error] StockClient.UpdateCache (Remove(key)): " + ex.Message);
                    }
                }
                try {
                    cache.Add (stock.Symbol, stock.Price);
                } catch (Exception ex) {
                    Console.WriteLine (DateTime.Now + " [Error] StockClient.UpdateCache (Add(key): " + ex.Message);
                }

            }
        }

        private bool IsChanged (Stock stock) {
            if (cache.ContainsKey (stock.Symbol)) {
                if (cache[stock.Symbol] != stock.Price) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return true;
            }
        }
    }
}