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
using PodoDemo.Common;

namespace PodoDemo.Controllers
{
    public class ManageController : Controller
    {
        private readonly PodoDemoNContext _context;

        public ManageController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 대메뉴 페이지 이동 및 불러오기
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
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

        /// <summary>
        /// 대메뉴 생성 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult MenuCreate([FromQuery]bool isPop)
        {
            ViewBag.isPop = isPop;
            return View();
        }

        /// <summary>
        /// 대메뉴 생성
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
                        // 기존의 순서가 존재하면 맨 뒤로 보내기
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
                    // 로그 
                    string dd = ex.InnerException.Message;
                    return View("Close", "Home");
                }
            }
            return View("Close", "Home");
        }

        /// <summary>
        /// 메뉴 수정 페이지로 이동
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

            var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewBag.isPop = isPop;

            return View(menu);
        }

        /// <summary>
        /// 실제 메뉴 수정 기능 수행
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MenuEdit(long? Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser")] Menu mainMenu)
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
                    
                    var sub = _context.Menu.AsNoTracking();
                    long oldOrder = sub.SingleOrDefault(x => x.Id == mainMenu.Id).Order;
                    if (sub.Any(e => e.Order == mainMenu.Order))
                    {
                        // 수정하고 있는 메뉴창에서 입력한 Order가 이미 존재한다면 교체
                        Menu exist = sub.SingleOrDefault(x => x.Order == mainMenu.Order);
                        exist.Order = oldOrder;     // 기존 메뉴를 새로 입력한 Order로 교체

                        SqlParameter[] param
                            = new SqlParameter[]{
                                new SqlParameter(){ ParameterName="@menuId", Value=mainMenu.Id, SqlDbType=SqlDbType.BigInt},
                                new SqlParameter(){ ParameterName="@newOrder",Value=mainMenu.Order, SqlDbType=SqlDbType.BigInt},
                                new SqlParameter(){ ParameterName="@existMenuId",Value=exist.Id, SqlDbType=SqlDbType.BigInt},
                                new SqlParameter(){ ParameterName="@oldOrder",Value=oldOrder, SqlDbType=SqlDbType.BigInt}
                            };

                        DataSet userResult = DatabaseUtil.getDataSet("P_Update_MainmenuOrder", param);

                        _context.Update(mainMenu);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // 존재하지 않으면 넣은 값으로 그대로 업데이트
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
        /// 메뉴 삭제(삭제 항목 업데이트)
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
        /// 대메뉴 더블클릭할 때 상세 메뉴 목록 가져오기
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
                                     UserAuth = sm.UserAuth
                                 }).ToList<SubMenu>();

                    return JsonConvert.SerializeObject(query);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 세부메뉴 생성 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult SubmenuCreate([FromQuery]bool isPop, int mainMenuid)
        {
            ViewBag.isPop = isPop;                  // 팝업여부
            ViewData["mainMenuid"] = mainMenuid;    // 메인 메뉴 고유값
            return View();
        }

        /// <summary>
        /// 상세메뉴 생성
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

                    // 기존의 순서가 존재하면 메뉴 순서 바꾸기
                    var sub = _context.SubMenu.Where(x => x.Mainmenuid == subMenu.Mainmenuid);
                    int menuCount = sub.Count();
                    // 존재하면
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

                    // 권한 추가
                    UserAuth newMenuUserAuth = new UserAuth() {
                        Userid = HttpContext.Session.GetString("userId"),
                        Read= "4-3",
                        Modify  = "4-3",
                        Write = "4-3",
                        Delete = "4-3",
                        Submenuid = subMenu.Id,
                        Createdate = DateTime.Now,
                        Createuser = HttpContext.Session.GetString("userId"),
                        Modifydate = DateTime.Now,
                        Modifyuser = HttpContext.Session.GetString("userId")
                    };

                    _context.UserAuth.Add(newMenuUserAuth);
                    await _context.SaveChangesAsync();
                    
                    return RedirectToAction("Close", "Home");
                }
                catch (Exception ex)
                {
                    // 로그 
                    string dd = ex.InnerException.Message;
                    return View("Close", "Home");
                }
            }
            return RedirectToAction("Close", "Home");
        }

        /// <summary>
        /// 상세 메뉴 정보 가져오기
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

            var menu = await _context.SubMenu.SingleOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewData["mainMenuid"] = mainMenuid;    // 메인 메뉴 고유값
            ViewBag.isPop = isPop;

            return View(menu);
        }

        /// <summary>
        /// 실제 상세 메뉴 수정 기능 수행
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

                    var sub = _context.SubMenu.AsNoTracking().Where(x => x.Mainmenuid == subMenu.Mainmenuid);      // 해당 대메뉴가 가지고 있는 서브메뉴들
                    int oldOrder = sub.SingleOrDefault(x => x.Id == subMenu.Id).Order;              // 수정하고 있는 세부메뉴의 Order
                    if (sub.Any(e => e.Order == subMenu.Order))
                    {
                        // 수정하고 있는 세부메뉴창에서 입력한 Order가 이미 존재한다면 교체
                        SubMenu exist = sub.SingleOrDefault(x => x.Order == subMenu.Order);
                        exist.Order = oldOrder;     // 기존 메뉴를 새로 입력한 Order로 교체

                        SqlParameter[] param 
                            = new SqlParameter[]{
                                new SqlParameter(){ ParameterName="@menuId", Value=subMenu.Id, SqlDbType=SqlDbType.NVarChar},
                                new SqlParameter(){ ParameterName="@newOrder",Value=subMenu.Order, SqlDbType=SqlDbType.Int},
                                new SqlParameter(){ ParameterName="@existMenuId",Value=exist.Id, SqlDbType=SqlDbType.NVarChar},
                                new SqlParameter(){ ParameterName="@oldOrder",Value=oldOrder, SqlDbType=SqlDbType.Int}
                            };

                        DataSet userResult = DatabaseUtil.getDataSet("P_Update_SubmenuOrder", param);

                        _context.Update(subMenu);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // 존재하지 않으면 넣은 값으로 그대로 업데이트
                        if(subMenu.Order > sub.Count())
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
        /// 메뉴 삭제(삭제 항목 업데이트)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteSubmenu(string Id, bool IsPop, [Bind("Id,Name,Order,Isused,Isdeleted,Createdate,Createuser,Modifydate,Modifyuser")] SubMenu subMenu)
        {
            //var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == Id);
            //_context.Menu.Remove(menu);
            //await _context.SaveChangesAsync();
            //return RedirectToAction("Close","Home");

            if (Id != subMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    subMenu.Isdeleted = true;
                    subMenu.Modifydate = DateTime.Now;
                    subMenu.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(subMenu);
                    await _context.SaveChangesAsync();
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
        /// 해당 메뉴가 존재하는지 검사
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool MenuExists(long id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }

        /// <summary>
        /// 상세 메뉴 존재하는지 검사
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SubmenuExists(string id)
        {
            return _context.SubMenu.Any(e => e.Id == id);
        }
    }
}