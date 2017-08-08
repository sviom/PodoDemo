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
    public class AppointmentsController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth;

        public AppointmentsController(PodoDemoNContext context)
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
                _userAuth = ss.CheckUseauth(userid, "3-3");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// 약속 페이지로 이동
        /// </summary>
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
            // 로그인유저
            ViewBag.UserId = HttpContext.Session.GetString("userId");
            // 팝업
            if (isPop == null)
            {
                ViewBag.isPop = false;
            }
            else
            {
                ViewBag.isPop = isPop;
            }

            List<Appointment> appointmentList = await _context.Appointment.Include(a => a.Owner).ToListAsync();
            foreach (Appointment item in appointmentList)
            {
                item.State = _context.OptionMasterDetail.Where(x => x.Optionid == item.State && x.Isused == true).Single().Name;
                item.Ownerid = _context.User.Where(x => x.Id == item.Ownerid).Single().Name;
            }

            return View((Object)JsonConvert.SerializeObject(appointmentList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// 할일 검색
        /// </summary>
        [HttpPost]
        public string Search([FromBody]AppointmentSearch appointmentSearch)
        {
            List<Appointment> appointmentList = new List<Appointment>();
            try
            {
                if (appointmentSearch != null)
                {
                    appointmentList = (from appointments in _context.Appointment
                                where
                                 (appointments.Name.Contains(appointmentSearch.Name) || appointmentSearch.Name.Equals(""))
                                 && (appointments.State.Equals(appointmentSearch.State) || appointmentSearch.State.Equals(""))
                                 && (appointments.Startdate >= appointmentSearch.Startdate || appointmentSearch.Startdate.Equals(DateTime.MinValue))
                                 && (appointments.Ownerid.Equals(appointmentSearch.Ownerid) || appointmentSearch.Ownerid.Equals(""))
                                select appointments).ToList();

                    foreach (Appointment item in appointmentList)
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
            return JsonConvert.SerializeObject(appointmentList);
        }

        /// <summary>
        /// 약속 생성 페이지로 이동
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

            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x => x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();

            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        /// <summary>
        /// 약속 생성
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // 사용자에게 쓰기 권한이 있는지 체크
            CreaetUserAuth();            
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (ModelState.IsValid)
            {
                appointment.Createdate = DateTime.Now;
                appointment.Createuser = HttpContext.Session.GetString("userId");
                appointment.Modifydate = DateTime.Now;
                appointment.Modifyuser = HttpContext.Session.GetString("userId");
                appointment.Ownerid = HttpContext.Session.GetString("userId");
                appointment.Owner = _context.User.Single(x => x.Id == appointment.Ownerid);

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
            // 사용자 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            return View(appointment);
        }

        /// <summary>
        /// 약속 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 사용자 권한
            CreaetUserAuth();
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // 읽기 권한이 없으면 아예 들어가지 못하게 한다.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 관련항목 출력 서브메뉴 리스트
            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x => x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();

            var appointment = await _context.Appointment.SingleOrDefaultAsync(m => m.Appointmentid == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
            return View(appointment);
        }

        /// <summary>
        /// 약속 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="appointment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Appointment appointment)
        {
            // 사용자 수정 권한 체크
            CreaetUserAuth();
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            if (id != appointment.Appointmentid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.Modifydate = DateTime.Now;
                    appointment.Modifyuser = HttpContext.Session.GetString("userId");
                    //appointment.Ownerid = HttpContext.Session.GetString("userId");
                    appointment.Owner = _context.User.Single(x => x.Id == appointment.Ownerid);

                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Appointmentid))
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

            #region 수정에 실패한 경우
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            // 읽기 권한이 없으면 아예 들어가지 못하게 한다.
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
            // 관련항목 출력 서브메뉴 리스트
            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.Where(x => x.Ismanager == false && x.Mainmenuid != 7).ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();
            #endregion

            return View(appointment);
        }


        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var appointment = await _context.Appointment.SingleOrDefaultAsync(m => m.Appointmentid == id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AppointmentExists(long id)
        {
            return _context.Appointment.Any(e => e.Appointmentid == id);
        }
    }
}
