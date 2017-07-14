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
using System.Data.SqlClient;
using System.Data;

namespace PodoDemo.Controllers
{
    public class OptionMastersController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static User loginedUser = new Models.User();

        public OptionMastersController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 옵션 리스트 페이지로 이동
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

            // 옵션의 시스템 여부 및 기타 여부에 따른 옵션 표시 항목 제한
            List<OptionMaster> optionmasterList = new List<OptionMaster>();
            if (loginedUser.Ismaster && (loginedUser.Level == "2-1" || loginedUser.Level == "시스템관리자"))
            {
                optionmasterList = await _context.OptionMaster.ToListAsync();
            }
            else
            {
                optionmasterList = await _context.OptionMaster.Where(x => x.Issystem == false).ToListAsync();
            }


            return View((Object)JsonConvert.SerializeObject(optionmasterList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// 옵션 생성 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult MasterCreate([FromQuery]bool isPop)
        {
            ViewBag.isPop = isPop;
            ViewBag.Ismaster = loginedUser.Ismaster;
            return View();
        }

        /// <summary>
        /// 옵션 생성
        /// </summary>
        /// <param name="isPop"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterCreate(bool isPop, [Bind("Name,Description,Isused")] OptionMaster optionMaster)
        {
            if (ModelState.IsValid)
            {
                optionMaster.Createdate = DateTime.Now;
                optionMaster.Createuser = HttpContext.Session.GetString("userId");
                optionMaster.Modifydate = DateTime.Now;
                optionMaster.Modifyuser = HttpContext.Session.GetString("userId");
                optionMaster.Ownerid = HttpContext.Session.GetString("userId");

                _context.Add(optionMaster);
                await _context.SaveChangesAsync();

                return RedirectToAction("Close", "Home");
            }
            ViewBag.isPop = isPop;
            return View(optionMaster);
        }

        /// <summary>
        /// 옵션 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> MasterEdit(long? id, [FromQuery]bool isPop)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionMaster = await _context.OptionMaster.SingleOrDefaultAsync(m => m.Masterid == id);
            if (optionMaster == null)
            {
                return NotFound();
            }

            ViewBag.isPop = isPop;
            ViewBag.Ismaster = loginedUser.Ismaster;

            return View(optionMaster);
        }

        /// <summary>
        /// 옵션 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterEdit(long Masterid, bool IsPop, [Bind("Masterid,Name,Description,Isused,Createdate,Createuser,Modifydate,Modifyuser,Issystem,Ownerid")] OptionMaster optionMaster)
        {
            if (Masterid != optionMaster.Masterid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    optionMaster.Modifydate = DateTime.Now;
                    optionMaster.Modifyuser = HttpContext.Session.GetString("userId");

                    _context.Update(optionMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionMasterExists(optionMaster.Masterid))
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
            return View(optionMaster);
        }

        /// <summary>
        /// 옵션 및 옵션 세부내용 삭제
        /// </summary>
        /// <param name="Masterid"></param>
        /// <param name="IsPop"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        public async Task<IActionResult> MasterDelete(long Masterid, bool IsPop, [Bind("Masterid,Name,Description,Isused,Createdate,Createuser,Modifydate,Modifyuser,Issystem,Ownerid")] OptionMaster optionMaster)
        {
            //var menu = await _context.Menu.SingleOrDefaultAsync(m => m.Id == Id);
            //_context.Menu.Remove(menu);
            //await _context.SaveChangesAsync();
            //return RedirectToAction("Close","Home");

            var dd = await _context.OptionMaster.SingleOrDefaultAsync(m => m.Masterid == Masterid);
            _context.OptionMaster.Remove(dd);

            var ss = _context.OptionMasterDetail.Where(m => m.Masterid == Masterid).ToList();
            _context.OptionMasterDetail.RemoveRange(ss);

            await _context.SaveChangesAsync();

            return RedirectToAction("Close", "Home");
        }

        /// <summary>
        /// 대메뉴 더블클릭할 때 상세 메뉴 목록 가져오기
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetOptiondetailList([FromBody]long Id)
        {
            try
            {
                if (!OptionMasterExists(Id))
                {
                    return "";
                }
                else
                {
                    List<OptionMasterDetail> optiondetailList
                        = await _context.OptionMasterDetail.Where(x => x.Masterid == Id).ToListAsync();

                    return JsonConvert.SerializeObject(optiondetailList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 세부 옵션 생성 페이지로 이동
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult OptionDetailCreate([FromQuery]bool isPop, long Masterid)
        {
            ViewBag.isPop = isPop;
            ViewData["Masterid"] = Masterid;
            return View();
        }

        /// <summary>
        /// 세부 옵션 생성
        /// </summary>
        /// <param name="isPop"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OptionDetailCreate(bool isPop, OptionMasterDetail optionmasterDetail)
        {
            if (ModelState.IsValid)
            {
                optionmasterDetail.Createdate = DateTime.Now;
                optionmasterDetail.Createuser = HttpContext.Session.GetString("userId");
                optionmasterDetail.Modifydate = DateTime.Now;
                optionmasterDetail.Modifyuser = HttpContext.Session.GetString("userId");

                optionmasterDetail.Optionid = optionmasterDetail.Masterid + "-" + (_context.OptionMasterDetail.Where(x => x.Masterid == optionmasterDetail.Masterid).Count() + 1);

                _context.Add(optionmasterDetail);
                await _context.SaveChangesAsync();

                return RedirectToAction("Close", "Home");
            }
            ViewBag.isPop = isPop;
            return View(optionmasterDetail);
        }

        /// <summary>
        /// 세부 옵션 수정 페이지로 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OptionDetailEdit(string id, [FromQuery]bool isPop, long Masterid)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionMasterDetail = await _context.OptionMasterDetail.SingleOrDefaultAsync(m => m.Optionid == id && m.Masterid == Masterid);
            if (optionMasterDetail == null)
            {
                return NotFound();
            }

            ViewBag.isPop = isPop;
            return View(optionMasterDetail);
        }

        /// <summary>
        /// 세부 옵션 수정
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsPop"></param>
        /// <param name="optionmasterDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OptionDetailEdit(string Optionid, bool IsPop, OptionMasterDetail optionmasterDetail)
        {
            if (Optionid != optionmasterDetail.Optionid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    optionmasterDetail.Modifydate = DateTime.Now;
                    optionmasterDetail.Modifyuser = HttpContext.Session.GetString("userId");

                    // 세부 옵션
                    var sub 
                        = _context.OptionMasterDetail
                                .AsNoTracking()
                                .Where(x => x.Masterid == optionmasterDetail.Masterid);

                    // 기존 순서
                    long oldOrder = sub.SingleOrDefault(x => x.Optionid == optionmasterDetail.Optionid).Order;

                    // 기존 순서 존재 시
                    if (sub.Any(e => e.Order == optionmasterDetail.Order) && oldOrder != optionmasterDetail.Order)
                    {
                        // 수정하고 있는 세부메뉴창에서 입력한 Order가 이미 존재한다면 교체
                        OptionMasterDetail exist = sub.SingleOrDefault(x => x.Order == optionmasterDetail.Order);
                        exist.Order = oldOrder;     // 기존 순서를 새로 입력한 Order로 교체
                        _context.Update(exist);     // 기존 순서를 업데이트
                        _context.Update(optionmasterDetail);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // 존재하지 않으면 넣은 값으로 그대로 업데이트
                        if (optionmasterDetail.Order > sub.Count())
                        {
                            optionmasterDetail.Order = sub.Count() + 1;
                        }

                        _context.Update(optionmasterDetail);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionDetailExists(optionmasterDetail.Optionid))
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
            return View(optionmasterDetail);
        }

        /// <summary>
        /// 옵션 세부 내용 삭제
        /// </summary>
        /// <param name="Masterid"></param>
        /// <param name="IsPop"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        public async Task<IActionResult> OptiondetailDelete(string Optionid, bool IsPop, OptionMasterDetail optionmasterDetail)
        {
            var ss = _context.OptionMasterDetail.SingleOrDefault(m => m.Optionid == Optionid);
            _context.OptionMasterDetail.Remove(ss);

            await _context.SaveChangesAsync();

            return RedirectToAction("Close", "Home");
        }

        private bool OptionMasterExists(long id)
        {
            return _context.OptionMaster.Any(e => e.Masterid == id);
        }

        private bool OptionDetailExists(string id)
        {
            return _context.OptionMasterDetail.Any(e => e.Optionid == id);
        }
    }
}
