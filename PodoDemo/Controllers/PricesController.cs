using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PodoDemo.Controllers
{
    public class PricesController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth = new UserAuth();

        public PricesController(PodoDemoNContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// 사용자 권한 넣기
        /// </summary>
        /// <returns></returns>
        public IActionResult CreaetUserAuth()
        {
            CommonAPIController ss = new CommonAPIController(_context);
            string userid = HttpContext.Session.GetString("userId");

            // 사용자 세션 체크
            if (!string.IsNullOrEmpty(userid))
            {
                _userAuth = ss.CheckUseauth(userid, "2-3");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// 가격표 목록 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            // 사용자 읽기 권한 체크
            CreaetUserAuth();
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            if (isPop == null)
            {
                ViewBag.isPop = false;
            }
            else
            {
                ViewBag.isPop = isPop;
            }

            List<Price> priceList = await _context.Price.Include(p => p.Product).ToListAsync();

            // 옵션 관련 이름으로 변환
            foreach (Price item in priceList)
            {
                item.Ownerid = _context.User.Single(x => x.Id == item.Ownerid).Name;
            }

            return View((Object)JsonConvert.SerializeObject(priceList));
        }

        /// <summary>
        /// 가격표 생성 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            // 사용자 권한
            CreaetUserAuth();
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // 사용자에게 쓰기 권한이 있는지 체크
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser");
            return View();
        }

        /// <summary>
        /// 가격표 생성
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productid,Prices,Cost,Currency,")] Price price)
        {
            // 사용자 쓰기 권한 체크
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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
