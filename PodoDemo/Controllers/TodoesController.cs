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
            List<Todo> das = await _context.Todo.ToListAsync();

            foreach (Todo item in das)
            {
                item.State = _context.OptionMasterDetail.Where(x => x.Optionid == item.State && x.Isused == true).Single().Name;
                item.Ownerid = _context.User.Where(x => x.Id == item.Ownerid).Single().Name;
            }

            ViewBag.UserId = HttpContext.Session.GetString("userId");

            return View((Object)JsonConvert.SerializeObject(das));
        }
        
        /// <summary>
        /// ���� �˻�
        /// </summary>
        [HttpPost]
        public string Search([FromBody]TodoSearch todoSearch)
        {
            List<Todo> todoList = new List<Todo>();
            try
            {
                if (todoSearch != null)
                {
                    todoList = (from todoes in _context.Todo
                                where
                                 (todoes.Name.Contains(todoSearch.Name) || todoSearch.Name.Equals(""))
                                 && (todoes.State.Equals(todoSearch.State) || todoSearch.State.Equals(""))
                                 && (todoes.Startdate >= todoSearch.Startdate || todoSearch.Startdate.Equals(DateTime.MinValue))
                                 && (todoes.Ownerid.Equals(todoSearch.Ownerid) || todoSearch.Ownerid.Equals(""))
                                select todoes).ToList();

                    foreach (Todo item in todoList)
                    {
                        item.State = _context.OptionMasterDetail.Where(x => x.Optionid == item.State && x.Isused == true).Single().Name;
                        item.Ownerid = _context.User.Where(x => x.Id == item.Ownerid).Single().Name;
                    }
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

        /// <summary>
        /// ���� ����
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Todo todo)
        {
            //[Bind("Name,Description,Regardingobjectid,Startdate,Enddate,State")] 
            CreaetUserAuth();
            // ����ڿ��� ���� ������ �ִ��� üũ
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                todo.Createdate = DateTime.Now;
                todo.Createuser = HttpContext.Session.GetString("userId");
                todo.Modifydate = DateTime.Now;
                todo.Modifyuser = HttpContext.Session.GetString("userId");
                todo.Ownerid = HttpContext.Session.GetString("userId");


                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // ����� ����
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            return View(todo);
        }

        /// <summary>
        /// ������������ �̵�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CreaetUserAuth();

            // ����� ����
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // �б� ������ ������ �ƿ� ���� ���ϰ� �Ѵ�.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // �����׸� ��� ����޴� ����Ʈ
            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x => x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();

            var todo = await _context.Todo.SingleOrDefaultAsync(m => m.Todoid == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Todoid,Name,Description,Regardingobjectid,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,State")] Todo todo)
        {
            CreaetUserAuth();

            // ����� ���� ���� üũ
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (id != todo.Todoid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    todo.Modifydate = DateTime.Now;
                    todo.Modifyuser = HttpContext.Session.GetString("userId");

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

            #region ������ ������ ���
            CreaetUserAuth();

            // ����� ����
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // �б� ������ ������ �ƿ� ���� ���ϰ� �Ѵ�.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // �����׸� ��� ����޴� ����Ʈ
            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x => x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();
            #endregion
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
