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
using PodoDemo.Common;
using System.Data;

namespace PodoDemo.Controllers
{
    public class OptionMastersController : Controller
    {
        private readonly PodoDemoNContext _context;

        public OptionMastersController(PodoDemoNContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// �ɼ� ����Ʈ �������� �̵�
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            List<OptionMaster> optionmasterList = await _context.OptionMaster.ToListAsync();
            return View((Object)JsonConvert.SerializeObject(optionmasterList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// �ɼ� ���� �������� �̵�
        /// </summary>
        /// <param name="isPop"></param>
        /// <returns></returns>
        public IActionResult MasterCreate([FromQuery]bool isPop)
        {
            ViewBag.isPop = isPop;
            return View();
        }

        /// <summary>
        /// �ɼ� ����
        /// </summary>
        /// <param name="isPop"></param>
        /// <param name="optionMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterCreate(bool isPop, [Bind("Name,Description,Isused,Issystem")] OptionMaster optionMaster)
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
        /// �ɼ� ���� �������� �̵�
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
            return View(optionMaster);
        }

        /// <summary>
        /// �ɼ� ����
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
        /// �޴� ����
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
        /// ��޴� ����Ŭ���� �� �� �޴� ��� ��������
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
                throw;
            }
        }

        /// <summary>
        /// ���� �ɼ� ���� �������� �̵�
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
        /// ���� �ɼ� ����
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

                optionmasterDetail.Optionid = optionmasterDetail.Masterid + "-" + (_context.OptionMasterDetail.Count() + 1);

                _context.Add(optionmasterDetail);
                await _context.SaveChangesAsync();

                return RedirectToAction("Close", "Home");
            }
            ViewBag.isPop = isPop;
            return View(optionmasterDetail);
        }

        /// <summary>
        /// ���� �ɼ� ���� �������� �̵�
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
        /// ���� �ɼ� ����
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

                    var sub = _context.OptionMasterDetail.AsNoTracking().Where(x => x.Masterid == optionmasterDetail.Masterid);
                    long oldOrder = sub.SingleOrDefault(x => x.Optionid == optionmasterDetail.Optionid).Order;
                    if (sub.Any(e => e.Order == optionmasterDetail.Order))
                    {
                        // �����ϰ� �ִ� ���θ޴�â���� �Է��� Order�� �̹� �����Ѵٸ� ��ü
                        OptionMasterDetail exist = sub.SingleOrDefault(x => x.Order == optionmasterDetail.Order);
                        exist.Order = oldOrder;     // ���� �޴��� ���� �Է��� Order�� ��ü

                        SqlParameter[] param
                            = new SqlParameter[]{
                                new SqlParameter(){ ParameterName="@optionId", Value=optionmasterDetail.Optionid, SqlDbType=SqlDbType.NVarChar},
                                new SqlParameter(){ ParameterName="@newOrder",Value=optionmasterDetail.Order, SqlDbType=SqlDbType.BigInt},
                                new SqlParameter(){ ParameterName="@existOptionId",Value=exist.Optionid, SqlDbType=SqlDbType.NVarChar},
                                new SqlParameter(){ ParameterName="@oldOrder",Value=oldOrder, SqlDbType=SqlDbType.BigInt}
                            };

                        DataSet userResult = DatabaseUtil.getDataSet("P_Update_OptiondetailOrder", param);

                        _context.Update(optionmasterDetail);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // �������� ������ ���� ������ �״�� ������Ʈ
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
