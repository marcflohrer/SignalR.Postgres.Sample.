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

        // 
        // GET: /stocks/
        public IEnumerable<Stock> Stocks()
        {
            return UnitOfWork.Stocks.GetAll();
        }

        // POST: Stocks/Edit/1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Symbol,Price")] Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return ExecuteTransaction(() => UnitOfWork.Stocks.Update(stock));
            }
            return View(stock);
        }

        // POST: Stocks/Edit/<Symbol>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert([Bind("Symbol,Price")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                return ExecuteTransaction(() => UnitOfWork.Stocks.Insert(stock));
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