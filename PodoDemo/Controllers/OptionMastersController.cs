using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Newtonsoft.Json;

namespace PodoDemo.Controllers
{
    public class OptionMastersController : Controller
    {
        private readonly PodoDemoNContext _context;

        public OptionMastersController(PodoDemoNContext context)
        {
            _context = context;    
        }

        // GET: OptionMasters
        public async Task<IActionResult> Index()
        {
            List<OptionMaster> optionmasterList = await _context.OptionMaster.ToListAsync();
            return View((Object)JsonConvert.SerializeObject(optionmasterList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        // GET: OptionMasters/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionMaster = await _context.OptionMaster
                .SingleOrDefaultAsync(m => m.Masterid == id);
            if (optionMaster == null)
            {
                return NotFound();
            }

            return View(optionMaster);
        }

        // GET: OptionMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OptionMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Masterid,Name,Description,Isused,Defaultvalue,Ownerid,Createdate,Createuser,Modifydate,Modifyuser,Issystem")] OptionMaster optionMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(optionMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(optionMaster);
        }

        // GET: OptionMasters/Edit/5
        public async Task<IActionResult> Edit(long? id)
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
            return View(optionMaster);
        }

        // POST: OptionMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Masterid,Name,Description,Isused,Defaultvalue,Ownerid,Createdate,Createuser,Modifydate,Modifyuser,Issystem")] OptionMaster optionMaster)
        {
            if (id != optionMaster.Masterid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction("Index");
            }
            return View(optionMaster);
        }

        // GET: OptionMasters/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionMaster = await _context.OptionMaster
                .SingleOrDefaultAsync(m => m.Masterid == id);
            if (optionMaster == null)
            {
                return NotFound();
            }

            return View(optionMaster);
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
                throw;
            }
        }

        // POST: OptionMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var optionMaster = await _context.OptionMaster.SingleOrDefaultAsync(m => m.Masterid == id);
            _context.OptionMaster.Remove(optionMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OptionMasterExists(long id)
        {
            return _context.OptionMaster.Any(e => e.Masterid == id);
        }
    }
}
