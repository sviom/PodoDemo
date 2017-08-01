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
        private static UserAuth _userAuth = new UserAuth();

        public AccountsController(PodoDemoNContext context)
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
                _userAuth = ss.CheckUseauth(userid, "1-1");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// 거래처 리스트로 이동
        /// </summary>
        /// <param name="isPop">팝업인지 아닌지</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            CreaetUserAuth();

            // 사용자 읽기 권한 체크
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

            ViewBag.UserId = HttpContext.Session.GetString("userId");

            List<Account> accountList = await _context.Account.ToListAsync();

            foreach (Account item in accountList)
            {
                item.Ownerid = _context.User.Single(x => x.Id == item.Ownerid).Name;
            }

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

            // 사용자에게 읽기권한(=검색)이 있는지 체크
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
        /// 거래처 생성 페이지로 이동
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

            return View();
        }

        /// <summary>
        /// 실제 거래처 생성
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            // 사용자 수정 권한 체크
            CreaetUserAuth();
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
                    // 로그 
                    string dd = ex.InnerException.Message;

                    View();
                }
            }
            return View();
        }

        /// <summary>
        /// 거래처 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            CreaetUserAuth();

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.SingleOrDefaultAsync(m => m.Accountid == id);
            // 연결된 연락처 리스트
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

            // 거래처 없을 경우
            if (account == null)
            {
                return NotFound();
            }

            // 읽기 권한이 없으면 아예 들어가지 못하게 한다.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;           

            return View(account);
        }

        /// <summary>
        /// 실제 거래처 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Accountid,Name,Phone,Fax,Homepage,Ceo,Postcode,Address,Addresscity,Addressdetail,Addresstype,Biznum,Founddate,Detail,Ownerid,Createuser,Createdate,Modifydate,Modifyuser")] Account account)
        {
            // 수정 권한 검사
            CreaetUserAuth();
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

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return View(account);
        }

        private bool AccountExists(long id)
        {
            return _context.Account.Any(e => e.Accountid == id);
        }

        /// <summary>
        /// 거래처 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(long? id)
        {
            // 권한 검사
            CreaetUserAuth();
            if (_userAuth.Delete.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (id == null)
            {
                return NotFound();
            }

            Account account = await _context.Account.Where(x => x.Accountid == id).SingleOrDefaultAsync();

            if (account != null)
            {
                _context.Remove(account);
                int deleteResult = await _context.SaveChangesAsync();

                // 삭제가 정상적으로 이루어지지 않았을 때
                if (deleteResult <= 0)
                {
                    return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
                }
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewBag.isPop = false;
            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return RedirectToAction("Index", "Accounts");
        }
    }
}
