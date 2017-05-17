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

        public AccountsController(PodoDemoNContext context)
        {
            _context = context;    
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            List<Account> accountList = await _context.Account.ToListAsync();
            ViewBag.UserId = HttpContext.Session.GetString("userId");
            return View((Object)JsonConvert.SerializeObject(accountList));
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .SingleOrDefaultAsync(m => m.Accountid == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
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
            return View();
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
                catch(Exception ex)
                {
                    // ·Î±× 
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

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .SingleOrDefaultAsync(m => m.Accountid == id);
            if (account == null)
            {
                return NotFound();
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
