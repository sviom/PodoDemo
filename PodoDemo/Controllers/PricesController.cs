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
        /// ����� ���� �ֱ�
        /// </summary>
        /// <returns></returns>
        public IActionResult CreaetUserAuth()
        {
            CommonAPIController ss = new CommonAPIController(_context);
            string userid = HttpContext.Session.GetString("userId");

            // ����� ���� üũ
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
        /// ����ǥ ��� �������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            // ����� �б� ���� üũ
            CreaetUserAuth();
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // ����
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

            // �ɼ� ���� �̸����� ��ȯ
            foreach (Price item in priceList)
            {
                item.Ownerid = _context.User.Single(x => x.Id == item.Ownerid).Name;
                item.Currency = _context.OptionMasterDetail.Single(x => x.Optionid == item.Currency).Name;
            }

            return View((Object)JsonConvert.SerializeObject(priceList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// ����ǥ ���� �������� �̵�
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            // ����� ����
            CreaetUserAuth();
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // ����ڿ��� ���� ������ �ִ��� üũ
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser");
            return View();
        }

        /// <summary>
        /// ����ǥ ����
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productid,Prices,Cost,Currency")] Price price)
        {
            // ����� ���� ���� üũ
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                price.Createdate = DateTime.Now;
                price.Createuser = HttpContext.Session.GetString("userId");
                price.Modifydate = DateTime.Now;
                price.Modifyuser = HttpContext.Session.GetString("userId");
                price.Ownerid = HttpContext.Session.GetString("userId");
                price.Product = _context.Product.SingleOrDefault(x => x.Productid == price.Productid);

                _context.Add(price);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            #region ��ǰ ����⿡ ������ ���
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            #endregion

            return View(price);
        }

        /// <summary>
        /// ����ǥ ���� �������� �̵�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            // ����� ����
            CreaetUserAuth();

            // �б� ������ ������ �ƿ� ���� ���ϰ� �Ѵ�.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            if (id == null)
            {
                return NotFound();
            }

            var price = await _context.Price.SingleOrDefaultAsync(m => m.Priceid == id);
            price.Product = _context.Product.Single(x => x.Productid == price.Productid);
            if (price == null)
            {
                return NotFound();
            }
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            return View(price);
        }

        /// <summary>
        /// ���� ����ǥ ����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Priceid,Productid,Prices,Cost,Currency,Createdate,Createuser,Modifydate,Modifyuser,Ownerid")] Price price)
        {
            // ���� ���� �˻�
            CreaetUserAuth();
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return View(price);
        }
        
        /// <summary>
        /// ���� ����ǥ ����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // ���� �˻�
            CreaetUserAuth();
            if (_userAuth.Delete.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
            
            var price = await _context.Price.SingleOrDefaultAsync(m => m.Priceid == id);
            _context.Price.Remove(price);
            await _context.SaveChangesAsync();

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return RedirectToAction("Index");
        }

        private bool PriceExists(long id)
        {
            return _context.Price.Any(e => e.Priceid == id);
        }
    }
}
