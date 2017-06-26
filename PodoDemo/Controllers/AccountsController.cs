using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace PodoDemo.Controllers
{
    public class AccountsController : Controller
    {
        private readonly PodoDemoNContext _context;
        UserAuth _userAuth;

        public AccountsController(PodoDemoNContext context)
        {
            _context = context;
            CommonAPIController ss = new CommonAPIController(_context);
            _userAuth = new UserAuth();
            _userAuth = ss.CheckUseauth(HttpContext.Session.GetString("userId"), "1-1");
        }

        // GET: Accounts
        public async Task<IActionResult> Index(bool? isPop)
        {   
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

            ViewBag.UserId = HttpContext.Session.GetString("userId");

            List<Account> accountList = await _context.Account.ToListAsync();
            return View((Object)JsonConvert.SerializeObject(accountList));
        }

        /// <summary>
        /// 거래처 검색
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public string Search([FromBody]AccountSearch info)
        {
            var query = (from ac in _context.Account
                         where (ac.Name.Contains(info.Name) || ac.Name.Equals(""))
                         && (ac.Ownerid.Contains(info.Ownerid) || ac.Ownerid.Equals(""))
                         && (ac.Phone.Contains(info.Phone) || ac.Phone.Equals(""))
                         select ac
                         ).ToList<Account>();
            return JsonConvert.SerializeObject(query);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            int count = this.GetType().GetProperties().Count();
            count = typeof(Account).GetProperties().Count();

            ViewBag.AccountPropertyCount = count;
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(Account).GetProperties();
            List<string> propertyList = new List<string>();
            List<string> propertyTypeList = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.Name.Equals("Accountid") &&
                    !propertyInfo.Name.Equals("Contact"))
                {
                    propertyList.Add(propertyInfo.Name);
                    propertyTypeList.Add(propertyInfo.PropertyType.Name);
                }
            }
            ViewBag.AccountPropertyTypeList = propertyTypeList;
            ViewBag.AccountPropertyList = propertyList;

            // 사용자 권한 검색
            if (_userAuth.Write.Equals("4-1") || _userAuth.Write.Equals("4-2"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { viewMessage = "해당 페이지에 접속할 권한이 없습니다." });
            }            
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    account.Createdate = DateTime.Now;
                    account.Createuser = HttpContext.Session.GetString("userId");
                    account.Modifydate = DateTime.Now;
                    account.Modifyuser = HttpContext.Session.GetString("userId");
                    account.Isdeleted = false;
                    account.Ownerid = HttpContext.Session.GetString("userId");

                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // 로그 
                    string dd = ex.InnerException.Message;

                    View();
                }
            }
            return View();
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.SingleOrDefaultAsync(m => m.Accountid == id);
            List<Contact> connContactslist = _context.Contact.Where(x => x.Accountid == account.Accountid).ToList();
            if (connContactslist.Count > 0)
            {
                ViewData["connctedContactList"]
                    = JsonConvert.SerializeObject(connContactslist, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            else
            {
                ViewData["connctedContactList"] = "";
            }

            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Accountid,Name,Phone,Fax,Homepage,Ceo,Postcode,Address,Addresscity,Addressdetail,Addresstype,Biznum,Founddate,Detail,Ownerid,Createuser,Createdate,Modifydate,Modifyuser")] Account account)
        {
            if (id != account.Accountid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    account.Modifydate = DateTime.Now;
                    account.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Accountid))
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
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var account = await _context.Account.SingleOrDefaultAsync(m => m.Accountid == id);
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AccountExists(long id)
        {
            return _context.Account.Any(e => e.Accountid == id);
        }
    }
}
