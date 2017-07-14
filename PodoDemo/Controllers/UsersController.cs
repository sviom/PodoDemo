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
    public class UsersController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static User loginedUser = new Models.User();

        public UsersController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 사용자 목록 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            loginedUser
                = await _context.User
                            .Where(x => x.Id == HttpContext.Session.GetString("userId"))
                            .SingleAsync();

            // 관리자가 아니면 접근 못하게
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "시스템관리자" && loginedUser.Level != "CRM관리자")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            //JsonConvert.SerializeObject(mainMenuList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //var podoDemoNContext = _context.User.Include(u => u.DepartmentNavigation);
            List<User> podoDemoNContext = await _context.User.ToListAsync();

            // 부서 직급 사용자 등급 표시
            foreach (var userItem in podoDemoNContext)
            {
                if (!string.IsNullOrEmpty(userItem.Department))
                {
                    userItem.Department = _context.OptionMasterDetail.SingleOrDefault(x => x.Optionid == userItem.Department).Name;
                }

                if (!string.IsNullOrEmpty(userItem.Position))
                {
                    userItem.Position = _context.OptionMasterDetail.SingleOrDefault(x => x.Optionid == userItem.Position).Name;
                }

                if (!string.IsNullOrEmpty(userItem.Level))
                {
                    userItem.Level = _context.OptionMasterDetail.SingleOrDefault(x => x.Optionid == userItem.Level).Name;
                }
            }

            return View((Object)JsonConvert.SerializeObject(podoDemoNContext, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// 사용자 생성 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            // 관리자가 아니면 접근 못하게
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "시스템관리자" && loginedUser.Level != "CRM관리자")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            ViewData["Department"] = new SelectList(_context.OptionMasterDetail, "Optionid", "Optionid");
            return View();
        }

        /// <summary>
        /// 사용자 생성
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pw,Name,Engname,Email,Phone,Mobile,Department,Position,Excelauth,Level")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Createdate = DateTime.Now;
                user.Createuser = HttpContext.Session.GetString("userId");
                user.Modifydate = DateTime.Now;
                user.Modifyuser = HttpContext.Session.GetString("userId");

                // 시스템 관리자 여부
                if (user.Level == "2-1")
                {
                    user.Ismaster = true;
                }
                else
                {
                    user.Ismaster = false;
                }

                _context.Add(user);
                await _context.SaveChangesAsync();

                // 사용자 생성 후 해당 사용자에 대한 권한 추가(기본값은 없음)
                var submenuList = _context.SubMenu.ToList();
                foreach (var item in submenuList)
                {
                    UserAuth createdUserAuth = new UserAuth()
                    {
                        Createdate = DateTime.Now,
                        Createuser = HttpContext.Session.GetString("userId"),
                        Modifydate = DateTime.Now,
                        Modifyuser = HttpContext.Session.GetString("userId"),

                        Delete = "4-3",
                        Modify = "4-3",
                        Write = "4-3",
                        Read = "4-3",

                        Submenu = item,
                        Submenuid = item.Id,
                        User = user
                    };

                    _context.Add(createdUserAuth);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            ViewData["Department"] = new SelectList(_context.OptionMasterDetail, "Optionid", "Optionid", user.Department);
            return View(user);
        }

        /// <summary>
        /// 사용자 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 관리자가 아니면 접근 못하게
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "시스템관리자" && loginedUser.Level != "CRM관리자")
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.OptionMasterDetail, "Optionid", "Optionid", user.Department);
            return View(user);
        }

        /// <summary>
        /// 사용자 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="existPw"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string existPw, [Bind("Id,Pw,Name,Engname,Email,Phone,Mobile,Department,Position,Excelauth,Level,Createdate,Createuser,Modifydate,Modifyuser")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Modifydate = DateTime.Now;
                    user.Modifyuser = HttpContext.Session.GetString("userId");

                    // 시스템 관리자 여부
                    if (user.Level == "2-1")
                    {
                        user.Ismaster = true;
                    }
                    else
                    {
                        user.Ismaster = false;
                    }

                    // 비밀번호 변경 여부
                    if (string.IsNullOrEmpty(user.Pw))
                    {
                        user.Pw = existPw;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["Department"] = new SelectList(_context.OptionMasterDetail, "Optionid", "Optionid", user.Department);
            return View(user);
        }

        /// <summary>
        /// 사용자 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
