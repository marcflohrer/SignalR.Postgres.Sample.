using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StockTickR.Hubs;
using StockTickR.Models;

namespace StockTickR
{
    public class StockTicker
    {
        readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim(1, 1);
        readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim(1, 1);

        readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();

        readonly Subject<Stock> _subject = new Subject<Stock>();

        // Stock can go up or down by a percentage of this factor on each change
        readonly double _rangePercent = 0.002;

        readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        readonly Random _updateOrNotRandom = new Random();

        Timer _timer;
        volatile bool _updatingStockPrices;
        volatile MarketState _marketState;

        public StockTicker(IHubContext<StockTickerHub> hub)
        {
            Hub = hub;
            LoadDefaultStocks();
        }

        IHubContext<StockTickerHub> Hub
        {
            get;
            set;
        }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        public IObservable<Stock> StreamStocks()
        {
            return _subject;
        }

        public async Task OpenMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState != MarketState.Open)
                {
                    _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

                    MarketState = MarketState.Open;

                    await BroadcastMarketStateChange(MarketState.Open);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        public async Task CloseMarket()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState == MarketState.Open)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    MarketState = MarketState.Closed;

                    await BroadcastMarketStateChange(MarketState.Closed);
                }
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        public async Task Reset()
        {
            await _marketStateLock.WaitAsync();
            try
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }

                LoadDefaultStocks();
                await BroadcastMarketReset();
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        void LoadDefaultStocks()
        {
            _stocks.Clear();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://stockdatabase:8082/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("stocks/").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            List<Stock> stocks = response.Content.ReadAsAsync<List<Stock>>().GetAwaiter().GetResult();
            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        }

        async void UpdateStockPrices(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            await _updateStockPricesLock.WaitAsync();
            try
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        TryUpdateStockPrice(stock);

                        _subject.OnNext(stock);
                    }

                    _updatingStockPrices = false;
                }
            }
            finally
            {
                _updateStockPricesLock.Release();
            }
        }

        bool TryUpdateStockPrice(Stock stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > 0.1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > 0.51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        async Task BroadcastMarketStateChange(MarketState marketState)
        {
            switch (marketState)
            {
                case MarketState.Open:
                    await Hub.Clients.All.SendAsync("marketOpened");
                    break;
                case MarketState.Closed:
                    await Hub.Clients.All.SendAsync("marketClosed");
                    break;
                default:
                    throw new Exception("Unknown market state");
            }
        }

        async Task BroadcastMarketReset()
        {
            await Hub.Clients.All.SendAsync("marketReset");
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
}