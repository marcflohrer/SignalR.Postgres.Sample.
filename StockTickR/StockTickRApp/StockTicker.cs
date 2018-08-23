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
using StockTickR.Clients;
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

        public StockTicker(IHubContext<StockTickerHub> hub, StockClient stockClient)
        {
            Hub = hub;
            _stockClient = stockClient;
            UpdateStockValues();
        }

        IHubContext<StockTickerHub> Hub
        {
            get;
            set;
        }
        private StockClient _stockClient { get; }

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

                UpdateStockValues();
                await BroadcastMarketReset();
            }
            finally
            {
                _marketStateLock.Release();
            }
        }

        void UpdateStockValues()
        {
            _stocks.Clear();
            List<Stock> stocks = (List<Stock>)_stockClient.Get();
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

                    UpdateStockValues();
                    foreach (var stock in _stocks.Values)
                    {
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