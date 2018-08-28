using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;
using StockDatabase.Models;
using StockDatabase.Repositories.Interfaces;

namespace StocksDatabase.Controllers {
    [Route ("[controller]")]
    public class StocksController : Controller {

        public IUnitOfWork UnitOfWork { get; }
        private Serilog.ILogger _logger { get; }

        public StocksController (IUnitOfWork unitOfWork, Serilog.ILogger logger) {
            UnitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /stocks/
        [HttpGet]
        public IEnumerable<Stock> Get () {
            _logger.Debug ("public IEnumerable<Stock> Get");
            return UnitOfWork.Stocks.GetAll ();
        }

        // GET: /stocks/1
        [HttpGet ("{id:int}")]
        public Stock Get (int id) {
            _logger.Debug ("public Stock Get");
            return UnitOfWork.Stocks.Get (id);
        }

        // POST: stocks/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost ("{id}")]
        public IActionResult PostCreate ([FromBody] Stock stock) {
            if (stock == null) {
                return View (stock);
            }
            var already = UnitOfWork.Stocks.GetStockBySymbol (stock.Symbol);
            if (already == null) {
                if (ModelState.IsValid) {
                    return ExecuteTransaction (() => UnitOfWork.Stocks.Insert (stock));
                }
            } else if (ModelState.IsValid) {
                return ExecuteTransaction (() => UnitOfWork.Stocks.Update (stock));
            }
            return View (stock);
        }

        // POST: stocks/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult PostCreate ([FromBody] IEnumerable<Stock> stocks) {
            var stocksFromDb = new List<Stock> ();
            stocks.ToList ().ForEach (stock => stocksFromDb.Add (UnitOfWork
                .Stocks
                .GetStockBySymbol (stock.Symbol)));
            var stocksWithChangedPrice = stocksFromDb.Where (stock => stock.Price != stocks.First (s => s.Symbol == stock.Symbol).Price)
                .ToList ();
            stocksWithChangedPrice
                .ForEach (stock => stock.Price = stocks.First (s => s.Symbol == stock.Symbol).Price);
            stocksWithChangedPrice
                .ForEach (stock => PostCreate (stock));
            return View (stocks);
        }

        private IActionResult ExecuteTransaction (Action action) {
            action.Invoke ();
            UnitOfWork.Complete ();
            return RedirectToAction ("Index");
        }
    }
}