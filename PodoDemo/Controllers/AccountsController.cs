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

        }

        /// <summary>
        /// �ŷ�ó ����Ʈ�� �̵�
        /// </summary>
        /// <param name="isPop">�˾����� �ƴ���</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            CommonAPIController ss = new CommonAPIController(_context);
            _userAuth = new UserAuth();
            string userid = HttpContext.Session.GetString("userId");
            _userAuth = ss.CheckUseauth(userid, "1-1");

            // ����� ���� ���� üũ
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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
        /// �ŷ�ó �˻�
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

        /// <summary>
        /// �ŷ�ó ���� �������� �̵�
        /// </summary>
        /// <returns></returns>
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

            // ����� ���� �˻�
            if (_userAuth.Write.Equals("4-1") || _userAuth.Write.Equals("4-2"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// ���� �ŷ�ó ����
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            // ����� ���� ���� üũ
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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
                    // �α� 
                    string dd = ex.InnerException.Message;

                    View();
                }
            }
            return View();
        }

        /// <summary>
        /// �ŷ�ó ���� �������� �̵�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.SingleOrDefaultAsync(m => m.Accountid == id);
            // ����� ����ó ����Ʈ
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

            // �ŷ�ó ���� ���
            if (account == null)
            {
                return NotFound();
            }

            // ����� ���� ���� üũ
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            return View(account);
        }

        /// <summary>
        /// ���� �ŷ�ó ����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Accountid,Name,Phone,Fax,Homepage,Ceo,Postcode,Address,Addresscity,Addressdetail,Addresstype,Biznum,Founddate,Detail,Ownerid,Createuser,Createdate,Modifydate,Modifyuser")] Account account)
        {
            // ����� ���� ���� üũ
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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
