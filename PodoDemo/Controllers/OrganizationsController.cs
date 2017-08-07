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
    public class OrganizationsController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static UserAuth _userAuth = new UserAuth();

        public OrganizationsController(PodoDemoNContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// 시스템 관리자 권한 체크
        /// </summary>
        /// <returns></returns>
        public bool CheckSystemUserAsync()
        {
            User loginedUser
                = _context.User
                            .Where(x => x.Id == HttpContext.Session.GetString("userId"))
                            .Single();

            // 관리자가 아니면 접근 못하게
            if (loginedUser.Level != "2-1" && loginedUser.Level != "시스템관리자")
            {
                return false;
            }
            else
            {
                return true;
            }
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
                _userAuth = ss.CheckUseauth(userid, "7-5");
                return null;
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }
        }


        /// <summary>
        /// 인덱스 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 읽기 권한 없으면 못들어가게 한다.
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

            List<Organization> podoDemoNContext = await _context.Organization.ToListAsync();

            return View((Object)JsonConvert.SerializeObject(podoDemoNContext, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        /// <summary>
        /// 생성 페이지로 이동
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 사용자 쓰기 권한 체크
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            return View();
        }

        /// <summary>
        /// 조직 생성
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Memo")] Organization organization)
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 사용자 쓰기 권한 체크
            CreaetUserAuth();
            if (_userAuth.Write.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            if (ModelState.IsValid)
            {
                organization.Createdate = DateTime.Now;

                _context.Add(organization);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(organization);
        }

        /// <summary>
        /// 조직 수정 페이지 이동
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 읽기 권한 없으면 못들어가게 한다.
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

            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organization.SingleOrDefaultAsync(m => m.Organizationid == id);
            if (organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }

        /// <summary>
        /// 실제 조직 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Organizationid,Name,Createdate,Memo")] Organization organization)
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 수정 권한 체크 
            CreaetUserAuth();
            if (_userAuth.Modify.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            if (id != organization.Organizationid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(organization.Organizationid))
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
            return View(organization);
        }

        /// <summary>
        /// 조직 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long? id)
        {
            if (!CheckSystemUserAsync())
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 삭제 권한 체크 
            CreaetUserAuth();
            if (_userAuth.Delete.Equals("4-3"))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserauthError" });
            }

            // 권한
            ViewData["Read"] = _userAuth.Read;
            ViewData["Write"] = _userAuth.Write;
            ViewData["Modify"] = _userAuth.Modify;
            ViewData["Delete"] = _userAuth.Delete;

            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organization.SingleOrDefaultAsync(m => m.Organizationid == id);
            _context.Organization.Remove(organization);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 조직 중복 검사
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost,ActionName("Search")]
        public bool Search([FromBody]Organization organization)
        {
            List<Organization> organiList = _context.Organization.Where(x => x.Name == organization.Name).ToList();
            if(organiList.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool OrganizationExists(long id)
        {
            return _context.Organization.Any(e => e.Organizationid == id);
        }

        
    }
}
