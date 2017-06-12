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

        public UserAuthsController(PodoDemoNContext context)
        {
            _context = context;
        }

        // GET: UserAuths
        public async Task<IActionResult> Index()
        {
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
            //ViewBag.MenuList = menuDDL;
            //ViewBag.SubmenuList = submenuDDL;

            var podoDemoNContext = _context.UserAuth.Include(u => u.Submenu).Include(u => u.User);
            return View(await podoDemoNContext.ToListAsync());
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
            List<UserAuth> _userauthlist = await _context.UserAuth.Where(x => x.Userid == info.Userid).ToListAsync();
            return JsonConvert.SerializeObject(_userauthlist);
        }

        /// <summary>
        /// 권한 종류 가져오기
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetAuthList()
        {
            List<OptionMasterDetail> authList = await _context.OptionMasterDetail.Where(x => x.Masterid == 4).ToListAsync();
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
                throw;
            }
            return editResult;
        }


        private bool UserAuthExists(long id)
        {
            return _context.UserAuth.Any(e => e.Id == id);
        }
    }
}
