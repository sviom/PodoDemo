using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Microsoft.AspNetCore.Http;
using PodoDemo.Models.InnerModels;
using Newtonsoft.Json;

namespace PodoDemo.Controllers
{
    public class UserAuthsController : Controller
    {
        private readonly PodoDemoNContext _context;

        public UserAuthsController(PodoDemoNContext context)
        {
            _context = context;    
        }

        // GET: UserAuths
        public async Task<IActionResult> Index()
        {
            ViewBag.UserId = HttpContext.Session.GetString("userId");

            List<DDL> menuDDL = new List<DDL>();
            foreach (var item in _context.Menu.ToList())
            {
                menuDDL.Add(new DDL() { Value = item.Id.ToString(), Text = item.Name });
            }

            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            //ViewBag.MenuList = JsonConvert.SerializeObject(menuDDL);
            //ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL);
            ViewBag.MenuList = menuDDL;
            ViewBag.SubmenuList = submenuDDL;

            var podoDemoNContext = _context.UserAuth.Include(u => u.Submenu).Include(u => u.User);
            return View(await podoDemoNContext.ToListAsync());
        }

        // GET: UserAuths/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuth = await _context.UserAuth
                .Include(u => u.Submenu)
                .Include(u => u.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userAuth == null)
            {
                return NotFound();
            }

            return View(userAuth);
        }

        // GET: UserAuths/Create
        public IActionResult Create()
        {
            ViewData["Submenuid"] = new SelectList(_context.SubMenu, "Id", "Id");
            ViewData["Userid"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: UserAuths/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Userid,Read,Modify,Write,Delete,Submenuid,Createdate,Createuser,Modifydate,Modifyuser")] UserAuth userAuth)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAuth);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["Submenuid"] = new SelectList(_context.SubMenu, "Id", "Id", userAuth.Submenuid);
            ViewData["Userid"] = new SelectList(_context.User, "Id", "Id", userAuth.Userid);
            return View(userAuth);
        }

        // GET: UserAuths/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuth = await _context.UserAuth.SingleOrDefaultAsync(m => m.Id == id);
            if (userAuth == null)
            {
                return NotFound();
            }
            ViewData["Submenuid"] = new SelectList(_context.SubMenu, "Id", "Id", userAuth.Submenuid);
            ViewData["Userid"] = new SelectList(_context.User, "Id", "Id", userAuth.Userid);
            return View(userAuth);
        }

        // POST: UserAuths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Userid,Read,Modify,Write,Delete,Submenuid,Createdate,Createuser,Modifydate,Modifyuser")] UserAuth userAuth)
        {
            if (id != userAuth.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAuth);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAuthExists(userAuth.Id))
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
            ViewData["Submenuid"] = new SelectList(_context.SubMenu, "Id", "Id", userAuth.Submenuid);
            ViewData["Userid"] = new SelectList(_context.User, "Id", "Id", userAuth.Userid);
            return View(userAuth);
        }

        // GET: UserAuths/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuth = await _context.UserAuth
                .Include(u => u.Submenu)
                .Include(u => u.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userAuth == null)
            {
                return NotFound();
            }

            return View(userAuth);
        }

        // POST: UserAuths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userAuth = await _context.UserAuth.SingleOrDefaultAsync(m => m.Id == id);
            _context.UserAuth.Remove(userAuth);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserAuthExists(long id)
        {
            return _context.UserAuth.Any(e => e.Id == id);
        }
    }
}
