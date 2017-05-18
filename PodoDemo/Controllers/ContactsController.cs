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

        public ContactsController(PodoDemoNContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index(bool? isPop)
        {
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

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Include(c => c.Account)
                .SingleOrDefaultAsync(m => m.Contactid == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 연락처 생성
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
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
            //ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid);
            return View(contact);
        }

        // GET: Contacts/Edit/5
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
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 실제 연락처 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
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

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Include(c => c.Account)
                .SingleOrDefaultAsync(m => m.Contactid == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var contact = await _context.Contact.SingleOrDefaultAsync(m => m.Contactid == id);
            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ContactExists(long id)
        {
            return _context.Contact.Any(e => e.Contactid == id);
        }
    }
}
