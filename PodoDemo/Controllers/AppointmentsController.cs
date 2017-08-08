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
                _userAuth = ss.CheckUseauth(userid, "3-3");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }

        /// <summary>
        /// ��� �������� �̵�
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(bool? isPop)
        {
            // ����� �б� ���� üũ
            CreaetUserAuth();
            if (_userAuth.Read.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // ����
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;
            // �α�������
            ViewBag.UserId = HttpContext.Session.GetString("userId");
            // �˾�
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
        /// ��� ���� �������� �̵�
        /// </summary>
        /// <returns></returns>
        public IActionResult Create(bool? isPop)
        {
            // ����� ����
            CreaetUserAuth();
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            // ����ڿ��� ���� ������ �ִ��� üũ
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

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Appointmentid,Name,Description,Regardingobjectid,Regardingobjecttypeid,Regardingobjectname,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,State")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.SingleOrDefaultAsync(m => m.Appointmentid == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Appointmentid,Name,Description,Regardingobjectid,Regardingobjecttypeid,Regardingobjectname,Startdate,Enddate,Createdate,Createuser,Modifydate,Modifyuser,Ownerid,State")] Appointment appointment)
        {
            if (id != appointment.Appointmentid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["Ownerid"] = new SelectList(_context.User, "Id", "Id", appointment.Ownerid);
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
