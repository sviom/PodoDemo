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

namespace PodoDemo.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth;


        public HolidaysController(PodoDemoNContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// 사용자 권한 넣기
        /// </summary>
        /// <returns></returns>
        public IActionResult CreaetUserAuth()
        {
            CommonAPIController ss = new CommonAPIController(_context);
            string userid = HttpContext.Session.GetString("userId");

            // 사용자 세션 체크
            if (!string.IsNullOrEmpty(userid))
            {
                _userAuth = ss.CheckUseauth(userid, "3-4");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// 휴가 목록 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            // 사용자 읽기 권한 체크
            CreaetUserAuth();
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            ViewBag.UserId = HttpContext.Session.GetString("userId");
            if (isPop == null)
            {
                ViewBag.isPop = false;
            }
            else
            {
                ViewBag.isPop = isPop;
            }
            
            List<Holiday> holidayList = await _context.Holiday.Include(h => h.Owner).ToListAsync();

            foreach (Holiday item in holidayList)
            {
                item.Type = _context.OptionMasterDetail.Where(x => x.Optionid == item.Type && x.Isused == true).Single().Name;
                item.Ownerid = _context.User.Where(x => x.Id == item.Ownerid).Single().Name;
            }

            return View((Object)JsonConvert.SerializeObject(holidayList));
        }

        /// <summary>
        /// 휴가 생성 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult Create(bool? isPop)
        {
            // 사용자 권한
            CreaetUserAuth();
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // 사용자에게 쓰기 권한이 있는지 체크
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (isPop == null)
            {
                ViewBag.isPop = false;
            }
            else
            {
                ViewBag.isPop = isPop;
            }

            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id");

            return View();
        }

        /// <summary>
        /// 휴가 생성
        /// </summary>
        /// <param name="holiday"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Startdate,Enddate,Type")] Holiday holiday)
        {
            // 사용자에게 쓰기 권한이 있는지 체크
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                holiday.Createdate = DateTime.Now;
                holiday.Createuser = HttpContext.Session.GetString("userId");
                holiday.Modifydate = DateTime.Now;
                holiday.Modifyuser = HttpContext.Session.GetString("userId");
                holiday.Ownerid = HttpContext.Session.GetString("userId");
                holiday.Owner = _context.User.Single(x => x.Id == holiday.Ownerid);
                holiday.Remaindate = 0;

                _context.Add(holiday);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", holiday.Ownerid);
            return View(holiday);
        }

        // GET: Holidays/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holiday.SingleOrDefaultAsync(m => m.Holidayid == id);
            if (holiday == null)
            {
                return NotFound();
            }
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", holiday.Ownerid);
            return View(holiday);
        }

        // POST: Holidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Holidayid,Name,Description,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,Type,Remaindate")] Holiday holiday)
        {
            if (id != holiday.Holidayid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(holiday);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidayExists(holiday.Holidayid))
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
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", holiday.Ownerid);
            return View(holiday);
        }

        // GET: Holidays/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holiday
                .Include(h => h.Owner)
                .SingleOrDefaultAsync(m => m.Holidayid == id);
            if (holiday == null)
            {
                return NotFound();
            }

            return View(holiday);
        }

        // POST: Holidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var holiday = await _context.Holiday.SingleOrDefaultAsync(m => m.Holidayid == id);
            _context.Holiday.Remove(holiday);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool HolidayExists(long id)
        {
            return _context.Holiday.Any(e => e.Holidayid == id);
        }
    }
}
