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
    public class ProductsController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth = new UserAuth();

        public ProductsController(PodoDemoNContext context)
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
                _userAuth = ss.CheckUseauth(userid, "2-2");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// ��� �������� �̵�
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

            List<Product> productList = await _context.Product.ToListAsync();

            // �ɼ� ���� �̸����� ��ȯ
            foreach (Product item in productList)
            {
                if(item.Maker != null)
                {
                    item.Maker = _context.OptionMasterDetail.Where(x => x.Optionid == item.Maker).Single().Name;
                }
                item.Ownerid = _context.User.Single(x => x.Id == item.Ownerid).Name;
            }

            return View((Object)JsonConvert.SerializeObject(productList));
        }

        /// <summary>
        /// ��ǰ �˻�
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public string Search([FromBody]ProductSearch info)
        {
            var query = (from pc in _context.Product
                         where (pc.Name.Contains(info.Name) || info.Name.Equals(""))
                         && (pc.Ownerid.Contains(info.Ownerid) || info.Ownerid.Equals(""))
                         && (pc.Productcode.Contains(info.Productcode) || info.Productcode.Equals(""))
                         && (pc.Maker.Equals(info.Maker) || info.Maker.Equals(""))
                         select pc
                         ).ToList<Product>();

            // ����ڿ��� �б����(=�˻�)�� �ִ��� üũ
            if (_userAuth.Read.Equals("4-3"))
            {
                return "";
            }
            else
            {
                return JsonConvert.SerializeObject(query);
            }
        }

        /// <summary>
        /// ��ǰ ���� �������� �̵�
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

            return View();
        }

        /// <summary>
        /// ��ǰ ���� ����
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Maker,Origin,Productcode")] Product product)
        {
            // ����� ���� ���� üũ
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                product.Createdate = DateTime.Now;
                product.Createuser = HttpContext.Session.GetString("userId");
                product.Modifydate = DateTime.Now;
                product.Modifyuser = HttpContext.Session.GetString("userId");
                product.Ownerid = HttpContext.Session.GetString("userId");

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            #region ������ ������ ���
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            #endregion
            return View(product);
        }

        /// <summary>
        /// ���� �������� �̵�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            // �б� ������ ������ �ƿ� ���� ���ϰ� �Ѵ�.
            CreaetUserAuth();
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

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Productid == id);
            if (product == null)
            {
                return NotFound();
            }

            // ����� ����ǥ ����Ʈ
            List<Price> priceList = _context.Price.Where(x => x.Productid == product.Productid).ToList();

            // �ɼ� ���� �̸����� ��ȯ
            foreach (Price item in priceList)
            {
                item.Ownerid = _context.User.Single(x => x.Id == item.Ownerid).Name;
                item.Currency = _context.OptionMasterDetail.Single(x => x.Optionid == item.Currency).Name;
            }

            if (priceList.Count > 0)
            {
                ViewData["connctedPriceList"]
                    = JsonConvert.SerializeObject(priceList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            else
            {
                ViewData["connctedPriceList"] = "";
            }

            return View(product);
        }

        /// <summary>
        /// ��ǰ ���� ��� ����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Productid,Name,Maker,Ownerid,Origin,Productcode,Createdate,Createuser,Modifydate,Modifyuser")] Product product)
        {
            // ���� ���� �˻�
            CreaetUserAuth();
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (id != product.Productid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Modifydate = DateTime.Now;
                    product.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Productid))
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

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return View(product);
        }
        
        /// <summary>
        /// ��ǰ ���� 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            // ���� �˻�
            CreaetUserAuth();
            if (_userAuth.Delete.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Productid == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(long id)
        {
            return _context.Product.Any(e => e.Productid == id);
        }
    }
}
