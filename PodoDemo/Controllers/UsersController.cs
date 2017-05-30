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

        public UsersController(PodoDemoNContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
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
