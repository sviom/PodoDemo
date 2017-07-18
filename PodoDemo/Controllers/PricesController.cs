using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;

namespace PodoDemo.Controllers
{
    public class PricesController : Controller
    {
        private readonly PodoDemoNContext _context;

        public PricesController(PodoDemoNContext context)
        {
            _context = context;    
        }

        // GET: Prices
        public async Task<IActionResult> Index()
        {
            var podoDemoNContext = _context.Price.Include(p => p.Product);
            return View(await podoDemoNContext.ToListAsync());
        }

        // GET: Prices/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var price = await _context.Price
                .Include(p => p.Product)
                .SingleOrDefaultAsync(m => m.Priceid == id);
            if (price == null)
            {
                return NotFound();
            }

            return View(price);
        }

        // GET: Prices/Create
        public IActionResult Create()
        {
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser");
            return View();
        }

        // POST: Prices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Priceid,Productid,Prices,Cost,Currency,Createdate,Createuser,Modifydate,Modifyuser,Ownerid")] Price price)
        {
            if (ModelState.IsValid)
            {
                _context.Add(price);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            return View(price);
        }

        // GET: Prices/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var price = await _context.Price.SingleOrDefaultAsync(m => m.Priceid == id);
            if (price == null)
            {
                return NotFound();
            }
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            return View(price);
        }

        // POST: Prices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Priceid,Productid,Prices,Cost,Currency,Createdate,Createuser,Modifydate,Modifyuser,Ownerid")] Price price)
        {
            if (id != price.Priceid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(price);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(price.Priceid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            return View(price);
        }

        // GET: Prices/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var price = await _context.Price
                .Include(p => p.Product)
                .SingleOrDefaultAsync(m => m.Priceid == id);
            if (price == null)
            {
                return NotFound();
            }

            return View(price);
        }

        // POST: Prices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var price = await _context.Price.SingleOrDefaultAsync(m => m.Priceid == id);
            _context.Price.Remove(price);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PriceExists(long id)
        {
            return _context.Price.Any(e => e.Priceid == id);
        }
    }
}
