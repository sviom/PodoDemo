using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PodoDemo.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace PodoDemo.Controllers
{
    public class ManageController : Controller
    {
        private readonly PodoDemoNContext _context;

        public ManageController(PodoDemoNContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Menu()
        {
            try
            {
                List<Menu> mainMenuList = _context.Menu.ToList();
                List<SubMenu> subMenuList = await _context.SubMenu.ToListAsync();

                ViewBag.MainMenuList
                    = JsonConvert.SerializeObject(mainMenuList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                ViewBag.SubMenuList
                    = JsonConvert.SerializeObject(subMenuList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IActionResult> MenuEdit(long? id, [FromQuery]bool isPop)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewBag.isPop = isPop;

            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> MenuEdit(long? Id, Menu mainMenu)
        {
            if (Id != mainMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mainMenu.Modifydate = DateTime.Now;
                    mainMenu.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(mainMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(mainMenu.Id))
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
            return View(mainMenu);
        }

        private bool MenuExists(long id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }
    }
}