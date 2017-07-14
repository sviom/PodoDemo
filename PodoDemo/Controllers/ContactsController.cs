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
        /// ����ó ������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            CommonAPIController ss = new CommonAPIController(_context);
            _userAuth = new UserAuth();
            string userid = HttpContext.Session.GetString("userId");

            // ����� ���� üũ
            if (!string.IsNullOrEmpty(userid))
            {
                _userAuth = ss.CheckUseauth(userid, "1-2");
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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

            List<Contact> contactList = await _context.Contact.Include(a => a.Account).ToListAsync();
            return View((Object)JsonConvert.SerializeObject(contactList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// ����ó �˻�
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
        /// ����ó ���� �������� �̵�
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            // ����ڿ��� ���� ������ �ִ��� üũ 
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
        /// ����ó ����
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
            // ����� ���� ���� üũ
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
        /// ����ó ������������ �̵�
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

            // �ŷ�ó ���̵�� �ŷ�ó �̸� ���� ��������
            ViewData["Accountid"]
                = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid).SelectedValue;
            ViewData["Accountname"] = _context.Account.SingleOrDefault(a => a.Accountid == contact.Accountid).Name;
            ViewData["Bossname"] = _context.Contact.SingleOrDefault(c => c.Contactid == contact.Bossid).Name;

            ViewData["userId"] = HttpContext.Session.GetString("userId");

            // �б� ������ ������ �ƿ� ���� ���ϰ� �Ѵ�.
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
                // ����ڰ� �ƴϸ� ������ ���ϰ� �Ѵ�.
                if (!_userAuth.Modify.Equals("4-3") && HttpContext.Session.GetString("userId").Equals(contact.Ownerid))
                {
                    ViewData["Modify"] = _userAuth.Modify;
                }
                else
                {
                    ViewData["Modify"] = "4-3";
                }
                // ����ڰ� �ƴϸ� ������ ���ϰ� �Ѵ�.
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
        /// ���� ����ó ����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
            // ����� ���� ���� üũ �� ����ڰ� �ƴϸ� ������ ���ϰ� �Ѵ�.
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
        /// ����ó ����
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

                // ������ ���������� �̷������ �ʾ��� ��
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
