using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog;
using StockTickR.Clients;
using StockTickR.Hubs;
using StockTickR.Models;

namespace StockTickR {
    public class StockTicker {
        readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim (1, 1);
        readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim (1, 1);

        readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock> ();

        readonly Subject<Stock> _subject = new Subject<Stock> ();

        readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds (2000);

        Timer _timer;
        volatile bool _updatingStockPrices;
        volatile MarketState _marketState;
        private Serilog.ILogger _logger;

        public StockTicker (IHubContext<StockTickerHub> hub, StockClient stockClient, Serilog.ILogger logger) {
            Hub = hub ??
                throw new ArgumentNullException (nameof (hub));
            _stockClient = stockClient ??
                throw new ArgumentNullException (nameof (stockClient));
            _logger = logger ??
                throw new ArgumentNullException (nameof (logger));
            UpdateStockValues ();
        }

        IHubContext<StockTickerHub> Hub {
            get;
            set;
        }
        private StockClient _stockClient { get; }

        public MarketState MarketState {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public IEnumerable<Stock> GetAllStocks () {
            return _stocks.Values;
        }

        public IObservable<Stock> StreamStocks () {
            return _subject;
        }

        public async Task OpenMarket () {
            _logger.Debug ("[ENTER] [OpenMarket]");
            await _marketStateLock.WaitAsync ();
            try {
                if (MarketState != MarketState.Open) {
                    _logger.Debug ("[DGB] [OpenMarket]: Opening MarketState.");
                    _timer = new Timer (UpdateStockPrices, null, _updateInterval, _updateInterval);

                    MarketState = MarketState.Open;

                    await BroadcastMarketStateChange (MarketState.Open);
                } else {
                    _logger.Debug ("[DGB] [OpenMarket]: MarketState already open.");
                }
            } finally {
                _marketStateLock.Release ();
            }
        }

        public async Task CloseMarket () {
            _logger.Debug ("[ENTER] [CloseMarket]");
            await _marketStateLock.WaitAsync ();
            try {
                if (MarketState == MarketState.Open) {
                    if (_timer != null) {
                        _timer.Dispose ();
                    }

                    MarketState = MarketState.Closed;

                    await BroadcastMarketStateChange (MarketState.Closed);
                }
            } finally {
                _marketStateLock.Release ();
            }
        }

        public async Task Reset () {
            _logger.Debug ("[ENTER] [Reset]");
            await _marketStateLock.WaitAsync ();
            try {
                if (MarketState != MarketState.Closed) {
                    throw new InvalidOperationException ("Market must be closed before it can be reset.");
                }

                UpdateStockValues ();
                await BroadcastMarketReset ();
            } finally {
                _marketStateLock.Release ();
            }
        }

        void UpdateStockValues () {
            _logger.Debug ("[ENTER] [UpdateStockValues]");
            _stocks.Clear ();
            List<Stock> stocks = (List<Stock>) _stockClient.Get ();
            stocks.ForEach (stock => _stocks.TryAdd (stock.Symbol, stock));
        }

        async void UpdateStockPrices (object state) {
            _logger.Debug ("[ENTER] [UpdateStockPrices]");
            // This function must be re-entrant as it's running as a timer interval handler
            await _updateStockPricesLock.WaitAsync ();
            try {
                if (!_updatingStockPrices) {
                    _updatingStockPrices = true;

                    UpdateStockValues ();
                    foreach (var stock in _stocks.Values) {
                        _logger.Debug ("Updating stock: " + stock.Symbol + " = " + stock.Price);
                        _subject.OnNext (stock);
                    }
                    _updatingStockPrices = false;
                }
            } finally {
                _updateStockPricesLock.Release ();
            }
        }

        async Task BroadcastMarketStateChange (MarketState marketState) {
            _logger.Debug ("[ENTER] [BroadcastMarketStateChange]: new state " + marketState + ", old state: " + MarketState);
            switch (marketState) {
                case MarketState.Open:
                    await Hub.Clients.All.SendAsync ("marketOpened");
                    break;
                case MarketState.Closed:
                    await Hub.Clients.All.SendAsync ("marketClosed");
                    break;
                default:
                    _logger.Error ("[BroadcastMarketStateChange]: Unknown market state: " + marketState);
                    throw new Exception ("Unknown market state");
            }
        }

        async Task BroadcastMarketReset () {
            await Hub.Clients.All.SendAsync ("marketReset");
        }
    }

    public enum MarketState {
        Closed,
        Open
    }
}