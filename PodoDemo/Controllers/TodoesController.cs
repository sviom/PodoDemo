using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PodoDemo.Models.InnerModels;

namespace PodoDemo.Controllers
{
    public class TodoesController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth;

        public TodoesController(PodoDemoNContext context)
        {
            _context = context;    
        }
        /// <summary>
        /// ����� ���� �ֱ�
        /// </summary>
        /// <returns></returns>
        public IActionResult CreaetUserAuth()
        {
            CommonAPIController ss = new CommonAPIController(_context);
            string userid = HttpContext.Session.GetString("userId");

            // ����� ���� üũ
            if (!string.IsNullOrEmpty(userid))
            {
                _userAuth = ss.CheckUseauth(userid, "3-2");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// ���� �������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            CreaetUserAuth();

            // ����� �б� ���� üũ
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // ����
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

            return View((Object)JsonConvert.SerializeObject(await _context.Todo.ToListAsync()));
        }
        
        /// <summary>
        /// ���� �˻�
        /// </summary>
        [HttpPost]
        public string Search([FromBody]Todo todoSearch)
        {
            List<Todo> todoList = new List<Todo>();
            try
            {
                if (todoSearch != null)
                {
                    todoList = 
                        _context.Todo
                        .Where(x => x.Name.Contains(todoSearch.Name) || todoSearch.Name.Equals(""))
                        .Where(x => x.State.Equals(todoSearch.State) || todoSearch.State.Equals(""))
                        .Where(x => x.Startdate > todoSearch.Startdate || todoSearch.Startdate.Equals(""))
                        .Where(x => x.Ownerid.Equals(todoSearch.Ownerid) || todoSearch.Ownerid.Equals(""))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return JsonConvert.SerializeObject(todoList);
        }

        /// <summary>
        /// ���� ���������� �̵�
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            CreaetUserAuth();

            // ����� ����
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // ����ڿ��� ���� ������ �ִ��� üũ
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x=>x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();
            return View();
        }

        // POST: Todoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Todoid,Name,Description,Regardingobjectid,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,State")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: Todoes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo.SingleOrDefaultAsync(m => m.Todoid == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        // POST: Todoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Todoid,Name,Description,Regardingobjectid,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,State")] Todo todo)
        {
            if (id != todo.Todoid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.Todoid))
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
            return View(todo);
        }

        // GET: Todoes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo
                .SingleOrDefaultAsync(m => m.Todoid == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var todo = await _context.Todo.SingleOrDefaultAsync(m => m.Todoid == id);
            _context.Todo.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TodoExists(long id)
        {
            return _context.Todo.Any(e => e.Todoid == id);
        }
    }
}
