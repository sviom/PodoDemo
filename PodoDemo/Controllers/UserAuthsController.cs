using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PodoDemo.Models;
using Microsoft.AspNetCore.Http;
using PodoDemo.Models.InnerModels;
using Newtonsoft.Json;

namespace PodoDemo.Controllers
{
    public class UserAuthsController : Controller
    {
        private readonly PodoDemoNContext _context;
        private static User loginedUser = new Models.User();

        public UserAuthsController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 권한 목록 페이지로 이동
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

            ViewBag.UserId = HttpContext.Session.GetString("userId");

            List<DDL> menuDDL = new List<DDL>();
            foreach (var item in _context.Menu.ToList())
            {
                menuDDL.Add(new DDL() { Value = item.Id.ToString(), Text = item.Name });
            }

            List<DDL> submenuDDL = new List<DDL>();
            foreach (var item in _context.SubMenu.ToList())
            {
                submenuDDL.Add(new DDL() { Value = item.Id, Text = item.Name });
            }
            ViewBag.MenuList = JsonConvert.SerializeObject(menuDDL).ToString();
            ViewBag.SubmenuList = JsonConvert.SerializeObject(submenuDDL).ToString();

            List<UserAuth> podoDemoNContext = await _context.UserAuth.Include(u => u.Submenu).Include(u => u.User).ToListAsync();
            return View(podoDemoNContext);
        }

        /// <summary>
        /// 사용자에 해당하는 권한 목록 얻어오기
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="menuId"></param>
        /// <param name="submenuId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetUserauthList([FromBody]UserAuthSearch info)
        {
            List<UserAuth> _userauthlist = new List<UserAuth>();

            if (!string.IsNullOrEmpty(info.Menuid))
            {
                if (!string.IsNullOrEmpty(info.Submenuid))
                {
                    _userauthlist =
                        await _context.UserAuth
                        .Where(x => x.Userid == info.Userid)
                        .Where(x => x.Submenu.Mainmenuid == Convert.ToDouble(info.Menuid))
                        .Where(x => x.Submenuid == info.Submenuid)
                        .ToListAsync();
                }
                else
                {
                    _userauthlist =
                        await _context.UserAuth
                        .Where(x => x.Userid == info.Userid)
                        .Where(x => x.Submenu.Mainmenuid == Convert.ToDouble(info.Menuid))
                        .ToListAsync();
                }
            }
            else
            {
                _userauthlist = await _context.UserAuth.Where(x => x.Userid == info.Userid).ToListAsync();
            }


            // 해당 권한 메뉴 이름 배정
            foreach (UserAuth item in _userauthlist)
            {
                item.Submenu = _context.SubMenu.Where(x => x.Id == item.Submenuid).SingleOrDefault();
            }

            return JsonConvert.SerializeObject(_userauthlist, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        /// <summary>
        /// 권한 종류 가져오기
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetAuthList()
        {
            List<OptionMasterDetail> authList 
                = await _context.OptionMasterDetail
                                .Where(x => x.Masterid == 4 && x.Isused == true)
                                .ToListAsync();
            List<DDL> authListDDL = new List<DDL>();
            foreach (OptionMasterDetail item in authList)
            {
                authListDDL.Add(new DDL() { Value = item.Optionid, Text = item.Name });
            }

            return JsonConvert.SerializeObject(authListDDL);
        }

        /// <summary>
        /// 사용자 권한 수정
        /// </summary>
        /// <param name="userAuthList"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> EditUserauthList([FromBody]List<UserAuth> userAuthList)
        {
            // 관리자가 아니면 접근 못하게
            if (loginedUser.Level != "2-1" && loginedUser.Level != "2-2" && loginedUser.Level != "시스템관리자" && loginedUser.Level != "CRM관리자")
            {
                return false;
            }

            bool editResult = false;
            try
            {
                foreach (UserAuth userAuth in userAuthList)
                {
                    userAuth.Createdate = DateTime.Now;
                    userAuth.Createuser = HttpContext.Session.GetString("userId");
                    userAuth.Modifydate = DateTime.Now;
                    userAuth.Modifyuser = HttpContext.Session.GetString("userId");
                }

                _context.UpdateRange(userAuthList);
                await _context.SaveChangesAsync();

                editResult = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return editResult;
        }

        private bool UserAuthExists(long id)
        {
            return _context.UserAuth.Any(e => e.Id == id);
        }
    }
}
