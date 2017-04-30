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
            // or
            count = typeof(Account).GetProperties().Count();

            ViewBag.AccountPropertyCount = count;
            //Account data = new Account();
            //Dictionary<string, string> myDict = new Dictionary<string, string>();
            //Type t = data.GetType();
            //foreach (PropertyInfo pi in t.GetProperties())
            //{
            //    //myDict[pi.Name] = //...value appropiate sended data.
            //    myDict[pi.Name] = pi.GetValue(data, null).ToString();
            //}
            //ViewBag.AccountPropertyList = myDict;

            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(Account).GetProperties();
            List<string> propertyList = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                propertyList.Add(propertyInfo.Name);
            }

            ViewBag.AccountPropertyList = propertyList;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Accountid,Name,Phone,Fax,Homepage,Ceo,Postcode,Address,Addresscity,Addressdetail,Addresstype,Biznum,Founddate,Detail,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(account);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Accountid,Name,Phone,Fax,Homepage,Ceo,Postcode,Address,Addresscity,Addressdetail,Addresstype,Biznum,Founddate,Detail,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Account account)
        {
            if (id != account.Accountid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
