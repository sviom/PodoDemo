using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Index()
        {
            //var podoDemoNContext = _context.Contact.Include(c => c.Account);
            List<Contact> contactList = await _context.Contact.ToListAsync();
            //List<Account> accountList = await _context.Account.ToListAsync();
            return View((Object)JsonConvert.SerializeObject(contactList));

            //return View(await podoDemoNContext.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Contactid,Name,Department,Accountid,Email,Phone,Mobile,Detail,Bossid,Createdate,Createuser,Modifydate,Modifyuser,Isdeleted,Ownerid")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid);
            return View(contact);
        }

        // GET: Contacts/Edit/5
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
            ViewData["Accountid"] = new SelectList(_context.Account, "Accountid", "Biznum", contact.Accountid);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
