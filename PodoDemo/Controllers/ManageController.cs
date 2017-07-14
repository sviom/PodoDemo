using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PodoDemo.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Data;

namespace PodoDemo.Controllers
{
    public class ManageController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static User loginedUser = new Models.User();

        public ManageController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ��޴� ������ �̵� �� �ҷ�����
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                loginedUser
                = await _context.User
                            .Where(x => x.Id == HttpContext.Session.GetString("userId"))
                            .SingleAsync();

                // �����ڰ� �ƴϸ� ���� ���ϰ�
                if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "�ý��۰�����" && loginedUser.Level != "CRM������")
                {
                    return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
                }

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
                throw ex;
            }
        }

        /// <summary>
        /// ��޴� ���� �������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult MenuCreate([FromQuery]bool isPop)
        {
            // �����ڰ� �ƴϸ� ���� ���ϰ�
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "�ý��۰�����" && loginedUser.Level != "CRM������")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewBag.isPop = isPop;
            return View();
        }

        /// <summary>
        /// ��޴� ����
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MenuCreate(bool isPop, Menu mainMenu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    mainMenu.Createdate = DateTime.Now;
                    mainMenu.Createuser = HttpContext.Session.GetString("userId");
                    mainMenu.Modifydate = DateTime.Now;
                    mainMenu.Modifyuser = HttpContext.Session.GetString("userId");
                    mainMenu.Isdeleted = false;

                    if (_context.Menu.Any(e => e.Order == mainMenu.Order))
                    {
                        // ������ ������ �����ϸ� �� �ڷ� ������
                        Menu exist = _context.Menu.SingleOrDefault(x => x.Order == mainMenu.Order);
                        int menuCount = _context.Menu.Count();
                        exist.Order = menuCount + 1;
                        _context.Update(exist);
                        await _context.SaveChangesAsync();
                    }

                    _context.Add(mainMenu);
                    await _context.SaveChangesAsync();

                    return View("Close", "Home");
                }
                catch (Exception ex)
                {
                    // �α� 
                    string dd = ex.InnerException.Message;
                    return View("Close", "Home");
                }
            }
            return View("Close", "Home");
        }

        /// <summary>
        /// �޴� ���� �������� �̵�
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> MenuEdit(long? id, [FromQuery]bool isPop)
        {
            if (id == null)
            {
                return NotFound();
            }

            // �����ڰ� �ƴϸ� ���� ���ϰ�
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "�ý��۰�����" && loginedUser.Level != "CRM������")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewBag.isPop = isPop;

            return View(menu);
        }

        /// <summary>
        /// ���� �޴� ���� ��� ����
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MenuEdit(long? Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser")] Menu mainMenu)
        {
            // �����ڰ� �ƴϸ� ���� ���ϰ�
            if (loginedUser.Level != "2-1" && loginedUser.Level != "�ý��۰�����")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

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

                    var sub = _context.Menu.AsNoTracking();

                    // ���� ����
                    long oldOrder = sub.SingleOrDefault(x => x.Id == mainMenu.Id).Order;

                    // 
                    if (sub.Any(e => e.Order == mainMenu.Order) && oldOrder != mainMenu.Order)
                    {
                        // �����ϰ� �ִ� �޴�â���� �Է��� Order�� �̹� �����Ѵٸ� ��ü
                        Menu exist = sub.SingleOrDefault(x => x.Order == mainMenu.Order);
                        exist.Order = oldOrder;     // ���� �޴��� ���� �Է��� Order�� ��ü
                        _context.Update(exist);
                        _context.Update(mainMenu);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // �������� ������ ���� ������ �״�� ������Ʈ
                        if (mainMenu.Order > sub.Count())
                        {
                            mainMenu.Order = sub.Count() + 1;
                        }

                        _context.Update(mainMenu);
                        await _context.SaveChangesAsync();
                    }
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
                return RedirectToAction("Close", "Home");
            }

            ViewBag.isPop = true;
            return View(mainMenu);
        }

        /// <summary>
        /// �޴� ����(���� �׸� ������Ʈ)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MenuDelete(long? Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser")] Menu mainMenu)
        {
            //var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == Id);
            //_context.Menu.Remove(menu);
            //await _context.SaveChangesAsync();
            //return RedirectToAction("Close","Home");

            if (Id != mainMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mainMenu.Isdeleted = true;
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
                return RedirectToAction("Close", "Home");
            }

            ViewBag.isPop = true;
            return View(mainMenu);
        }

        /// <summary>
        /// ��޴� ����Ŭ���� �� �� �޴� ��� ��������
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetSubmenulist([FromBody]long Id)
        {
            try
            {
                if (!MenuExists(Id))
                {
                    return "";
                }
                else
                {
                    var query = (from sm in _context.SubMenu
                                 where sm.Mainmenuid.Equals(Id)
                                 select new SubMenu
                                 {
                                     Name = sm.Name,
                                     Id = sm.Id,
                                     Createdate = sm.Createdate,
                                     Createuser = sm.Createuser,
                                     Modifydate = sm.Modifydate,
                                     Modifyuser = sm.Modifyuser,
                                     Isdeleted = sm.Isdeleted,
                                     Isused = sm.Isused,
                                     Ismanager = sm.Ismanager,
                                     Mainmenu = sm.Mainmenu,
                                     Mainmenuid = sm.Mainmenuid,
                                     Menuurl = sm.Menuurl,
                                     Order = sm.Order,
                                     //UserAuth = sm.UserAuth
                                 }).ToList<SubMenu>();

                    return JsonConvert.SerializeObject(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ���θ޴� ���� �������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult SubmenuCreate([FromQuery]bool isPop, int mainMenuid)
        {
            // �����ڰ� �ƴϸ� ���� ���ϰ�
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "�ý��۰�����" && loginedUser.Level != "CRM������")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewBag.isPop = isPop;                  // �˾�����
            ViewData["mainMenuid"] = mainMenuid;    // ���� �޴� ������
            return View();
        }

        /// <summary>
        /// �󼼸޴� ����
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubmenuCreate(bool isPop, SubMenu subMenu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    subMenu.Createdate = DateTime.Now;
                    subMenu.Createuser = HttpContext.Session.GetString("userId");
                    subMenu.Modifydate = DateTime.Now;
                    subMenu.Modifyuser = HttpContext.Session.GetString("userId");
                    subMenu.Isdeleted = false;

                    // ������ ������ �����ϸ� �޴� ���� �ٲٱ�
                    var sub = _context.SubMenu.Where(x => x.Mainmenuid == subMenu.Mainmenuid);
                    int menuCount = sub.Count();
                    // �����ϸ�
                    if (sub.Any(e => e.Order == subMenu.Order))
                    {
                        SubMenu exist = sub.SingleOrDefault(x => x.Order == subMenu.Order);
                        exist.Order = menuCount + 1;
                        _context.Update(exist);
                        await _context.SaveChangesAsync();
                    }

                    subMenu.Id = subMenu.Mainmenuid + "-" + (menuCount + 1);

                    _context.Add(subMenu);
                    await _context.SaveChangesAsync();

                    // ����� �� ��ŭ ���� �߰�
                    List<User> userList = _context.User.ToList();
                    List<UserAuth> addedUserAuthList = new List<UserAuth>();
                    foreach (User item in userList)
                    {
                        UserAuth newMenuUserAuth = new UserAuth();
                        newMenuUserAuth.Userid = item.Id;

                        // �޴� ���� �ÿ��� ���������ڴ� ��� ���� ȹ��
                        if(item.Level == "2-1" || item.Level == "�ý��۰�����" || item.Ismaster == true)
                        {
                            newMenuUserAuth.Read = "4-1";
                            newMenuUserAuth.Modify = "4-1";
                            newMenuUserAuth.Write = "4-1";
                            newMenuUserAuth.Delete = "4-1";
                        }
                        else
                        {
                            newMenuUserAuth.Read = "4-3";
                            newMenuUserAuth.Modify = "4-3";
                            newMenuUserAuth.Write = "4-3";
                            newMenuUserAuth.Delete = "4-3";
                        }
                        
                        newMenuUserAuth.Submenuid = subMenu.Id;
                        newMenuUserAuth.Createdate = DateTime.Now;
                        newMenuUserAuth.Createuser = HttpContext.Session.GetString("userId");
                        newMenuUserAuth.Modifydate = DateTime.Now;
                        newMenuUserAuth.Modifyuser = HttpContext.Session.GetString("userId");

                        addedUserAuthList.Add(newMenuUserAuth);
                    }

                    //_context.UserAuth.Add(newMenuUserAuth);
                    _context.UserAuth.AddRange(addedUserAuthList);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Close", "Home");
                }
                catch (Exception ex)
                {
                    // �α� 
                    string dd = ex.InnerException.Message;
                    return View("Close", "Home");
                }
            }
            return RedirectToAction("Close", "Home");
        }

        /// <summary>
        /// �� �޴� ���� ��������
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> SubmenuEdit(string id, [FromQuery]bool isPop, int mainMenuid)
        {
            if (id == null)
            {
                return NotFound();
            }

            // �����ڰ� �ƴϸ� ���� ���ϰ�
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "�ý��۰�����" && loginedUser.Level != "CRM������")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            var menu = await _context.SubMenu.SingleOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewData["mainMenuid"] = mainMenuid;    // ���� �޴� ������
            ViewBag.isPop = isPop;

            return View(menu);
        }

        /// <summary>
        /// ���� �� �޴� ���� ��� ����
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SubmenuEdit(string Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser,Menuurl,Mainmenuid")] SubMenu subMenu)
        {
            if (Id != subMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    subMenu.Modifydate = DateTime.Now;
                    subMenu.Modifyuser = HttpContext.Session.GetString("userId");

                    var sub = _context.SubMenu.AsNoTracking().Where(x => x.Mainmenuid == subMenu.Mainmenuid);      // �ش� ��޴��� ������ �ִ� ����޴���
                    int oldOrder = sub.SingleOrDefault(x => x.Id == subMenu.Id).Order;              // �����ϰ� �ִ� ���θ޴��� Order
                    if (sub.Any(e => e.Order == subMenu.Order) && oldOrder != subMenu.Order)
                    {
                        // �����ϰ� �ִ� ���θ޴�â���� �Է��� Order�� �̹� �����Ѵٸ� ��ü
                        SubMenu exist = sub.SingleOrDefault(x => x.Order == subMenu.Order);
                        exist.Order = oldOrder;     // ���� �޴��� ���� �Է��� Order�� ��ü
                        _context.Update(exist);
                        _context.Update(subMenu);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // �������� ������ ���� ������ �״�� ������Ʈ
                        if (subMenu.Order > sub.Count())
                        {
                            subMenu.Order = sub.Count() + 1;
                        }
                        _context.Update(subMenu);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmenuExists(subMenu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Close", "Home");
            }

            ViewBag.isPop = true;
            return View(subMenu);
        }

        /// <summary>
        /// �޴� ����(���� �׸� ������Ʈ)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteSubmenu(string Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser")] SubMenu subMenu)
        {
            if (Id != subMenu.Id)
            {
                return NotFound();
            }

            // Submenu ����
            var menu = await _context.SubMenu.SingleOrDefaultAsync(m => m.Id == Id);
            _context.SubMenu.Remove(menu);
            await _context.SaveChangesAsync();

            // Submenu�� �ش��ϴ� UserAuth ����
            List<UserAuth> removedUserAuthList = _context.UserAuth.Where(x => x.Submenuid == Id).ToList();
            _context.UserAuth.RemoveRange(removedUserAuthList);
            await _context.SaveChangesAsync();

            return RedirectToAction("Close", "Home");
        }

        /// <summary>
        /// �ش� �޴��� �����ϴ��� �˻�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool MenuExists(long id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }

        /// <summary>
        /// �� �޴� �����ϴ��� �˻�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SubmenuExists(string id)
        {
            return _context.SubMenu.Any(e => e.Id == id);
        }
    }
}