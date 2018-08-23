using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using StockDatabase.Models;

namespace StockTickR.Clients
{
    public class StockClient
    {
        private HttpClient _client;
        MediaTypeWithQualityHeaderValue _mediaType = new MediaTypeWithQualityHeaderValue("application/json");
        public StockClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://stockdatabase:8082/")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(_mediaType);
        }

        public IEnumerable<Stock> Get()
        {
            var response = _client.GetAsync("stocks/").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsAsync<List<Stock>>().GetAwaiter().GetResult();
        }

        public HttpStatusCode Add(Stock stock)
        {
            var response = _client.PostAsJsonAsync("stocks/" + stock.Id, stock).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public HttpStatusCode AddRange(IEnumerable<Stock> stocks)
        {
            var response = _client.PostAsJsonAsync("stocks/", stocks).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
    }
}
