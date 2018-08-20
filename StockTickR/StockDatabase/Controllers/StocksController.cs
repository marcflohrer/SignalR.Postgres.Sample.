using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockDatabase.Models;
using StockDatabase.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace StocksDatabase.Controllers
{
    public class StocksController : Controller
    {

        public IUnitOfWork UnitOfWork { get; }

        public StocksController(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

        // 
        // GET: /Stocks/

        public IEnumerable<Stock> Stocks() => UnitOfWork.Stocks.GetAll();

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