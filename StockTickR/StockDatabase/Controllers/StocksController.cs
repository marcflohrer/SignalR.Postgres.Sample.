using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockDatabase.Models;
using StockDatabase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace StocksDatabase.Controllers
{
    [Route("[controller]")]
    public class StocksController : Controller
    {

        public IUnitOfWork UnitOfWork { get; }
        public ILogger<StocksController> Logger { get; }


        public StocksController(IUnitOfWork unitOfWork, ILogger<StocksController> logger)
        {
            UnitOfWork = unitOfWork;
            Logger = logger;
        }

        // GET: /stocks/
        [HttpGet]
        public IEnumerable<Stock> Get()
        {
            Logger.LogDebug("[StocksController]: public IEnumerable<Stock> Get");
            return UnitOfWork.Stocks.GetAll();
        }

        // GET: /stocks/GOOG
        [HttpGet("{symbol}")]
        public Stock Get(string symbol)
        {
            Logger.LogDebug("[StocksController]: public Stock Get");
            return UnitOfWork.Stocks.Get(symbol);
        }

        // POST: stocks/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostCreate([FromBody] Stock stock)
        {
            var already = Get(stock.Symbol);
            if (already == null)
            {
                if (ModelState.IsValid)
                {
                    return ExecuteTransaction(() => UnitOfWork.Stocks.Insert(stock));
                }
            }

            if (ModelState.IsValid)
            {
                return ExecuteTransaction(() => UnitOfWork.Stocks.Update(stock));
            }
            return View(stock);
        }

        private IActionResult ExecuteTransaction(Action action)
        {
            action.Invoke();
            UnitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}