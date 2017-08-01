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
                item.Currency = _context.OptionMasterDetail.Single(x => x.Optionid == item.Currency).Name;
            }

            return View((Object)JsonConvert.SerializeObject(priceList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
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
        public async Task<IActionResult> Create([Bind("Productid,Prices,Cost,Currency")] Price price)
        {
            // 사용자 쓰기 권한 체크
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

            #region 제품 만들기에 실패할 경우
            ViewData["Productid"] = new SelectList(_context.Product, "Productid", "Createuser", price.Productid);
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            #endregion

            return View(price);
        }

        /// <summary>
        /// 가격표 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            // 사용자 권한
            CreaetUserAuth();

            // 읽기 권한이 없으면 아예 들어가지 못하게 한다.
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
        /// 실제 가격표 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Priceid,Productid,Prices,Cost,Currency,Createdate,Createuser,Modifydate,Modifyuser,Ownerid")] Price price)
        {
            // 수정 권한 검사
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
        /// 실제 가격표 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // 권한 검사
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
