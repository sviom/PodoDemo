using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace PodoDemo.Controllers
{
    public class ContactsController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth;

        public ContactsController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 연락처 목록으로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            CommonAPIController ss = new CommonAPIController(_context);
            _userAuth = new UserAuth();
            string userid = HttpContext.Session.GetString("userId");

            // 사용자 세션 체크
            if (!string.IsNullOrEmpty(userid))
            {
                _userAuth = ss.CheckUseauth(userid, "1-2");
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 사용자 수정 권한 체크
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

            List<Contact> contactList = await _context.Contact.Include(a => a.Account).ToListAsync();
            return View((Object)JsonConvert.SerializeObject(contactList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// 연락처 검색
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public string Search([FromBody]ContactSearch info)
        {
            var query = (from cc in _context.Contact
                         join ca in _context.Account on cc.Accountid equals ca.Accountid
                         where (cc.Name.Contains(info.Name) || cc.Name.Equals(""))
                         && (cc.Ownerid.Contains(info.Ownerid) || cc.Ownerid.Equals(""))
                         && (cc.Phone.Contains(info.Phone) || cc.Phone.Equals(""))
                         select cc
                         )
                         .Include(a => a.Account)
                         .ToList<Contact>();
            return JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        /// <summary>
        /// 연락처 생성 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            // 사용자에게 쓰기 권한이 있는지 체크 
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum");
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return View();
        }

        /// <summary>
        /// 연락처 생성
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
            // 사용자 수정 권한 체크
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                contact.Createdate = DateTime.Now;
                contact.Createuser = HttpContext.Session.GetString("userId");
                contact.Modifydate = DateTime.Now;
                contact.Modifyuser = HttpContext.Session.GetString("userId");
                contact.Isdeleted = false;
                contact.Ownerid = HttpContext.Session.GetString("userId");

                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        /// <summary>
        /// 연락처 수정페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.Contactid == id);
            if (contact == null)
            {
                return NotFound();
            }

            // 거래처 아이디로 거래처 이름 포함 가져오기
            ViewData["Accountid"]
                = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid).SelectedValue;
            ViewData["Accountname"] = _context.Account.SingleOrDefault(a => a.Accountid == contact.Accountid).Name;
            ViewData["Bossname"] = _context.Contact.SingleOrDefault(c => c.Contactid == contact.Bossid).Name;

            ViewData["userId"] = HttpContext.Session.GetString("userId");

            // 읽기 권한이 없으면 아예 들어가지 못하게 한다.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;

            if (_context.User.Where(x => x.Id == HttpContext.Session.GetString("userId")).Single().Ismaster)
            {
                ViewData["Modify"] = _userAuth.Modify;
                ViewData["Delete"] = _userAuth.Delete;
            }
            else
            {
                // 담당자가 아니면 수정을 못하게 한다.
                if (!_userAuth.Modify.Equals("4-3") && HttpContext.Session.GetString("userId").Equals(contact.Ownerid))
                {
                    ViewData["Modify"] = _userAuth.Modify;
                }
                else
                {
                    ViewData["Modify"] = "4-3";
                }
                // 담당자가 아니면 삭제를 못하게 한다.
                if (!_userAuth.Delete.Equals("4-3") && HttpContext.Session.GetString("userId").Equals(contact.Ownerid))
                {
                    ViewData["Delete"] = _userAuth.Delete;
                }
                else
                {
                    ViewData["Delete"] = "4-3";
                }
            }

            return View(contact);
        }

        /// <summary>
        /// 실제 연락처 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
            // 사용자 수정 권한 체크 및 담당자가 아니면 수정을 못하게 한다.
            if (_userAuth.Modify.Equals("4-3") || HttpContext.Session.GetString("userId") != contact.Ownerid)
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (id != contact.Contactid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contact.Modifydate = DateTime.Now;
                    contact.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Contactid))
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
            ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid);
            return View(contact);
        }

        private bool ContactExists(long id)
        {
            return _context.Contact.Any(e => e.Contactid == id);
        }

        /// <summary>
        /// 연락처 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contact contact = await _context.Contact.Where(x => x.Contactid == id).SingleOrDefaultAsync();

            if (contact != null)
            {
                _context.Remove(contact);
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

            return View("Index");
        }
    }
}
