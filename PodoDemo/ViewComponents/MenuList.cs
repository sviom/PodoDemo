using PodoDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using PodoDemo.Models.InnerModels;

namespace PodoDemo.ViewComponents
{
    [ViewComponent(Name = "MenuList")]
    public class MenuList : ViewComponent
    {
        private readonly PodoDemoNContext _context;

        public MenuList(PodoDemoNContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            User loginedUser
                = await _context.User
                            .Where(x => x.Id == HttpContext.Session.GetString("userId"))
                            .SingleAsync();

            // 권한에 따른 대메뉴 출력
            List<MenuDisplay> query = new List<MenuDisplay>();

            if (loginedUser.Level == "2-1" || loginedUser.Level == "2-2" || loginedUser.Level == "시스템관리자" || loginedUser.Level == "CRM관리자")
            {
                // 시스템관리자/CRM관리자
                query = (from ua in _context.UserAuth
                         join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                         join mm in _context.Menu on sm.Mainmenuid equals mm.Id
                         where
                              ua.Userid == HttpContext.Session.GetString("userId")
                              && mm.Isused == true
                              && sm.Isused == true
                              && sm.Isdeleted == false
                         group mm by new
                         {
                             mm.Id,
                             mm.Name
                         } into mmg
                         select new MenuDisplay
                         {
                             ParentMenuId = mmg.Key.Id,
                             ParentMenuName = mmg.Key.Name
                         }).ToList<MenuDisplay>();
            }
            else
            {
                // 일반 사용자
                query = (from ua in _context.UserAuth
                         join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                         join mm in _context.Menu on sm.Mainmenuid equals mm.Id
                         where
                              ua.Userid == HttpContext.Session.GetString("userId")
                              && mm.Isused == true
                              && sm.Isused == true
                              && sm.Isdeleted == false
                              && mm.Id != 7
                              && ua.Read != "4-3"
                         group mm by new
                         {
                             mm.Id,
                             mm.Name
                         } into mmg
                         select new MenuDisplay
                         {
                             ParentMenuId = mmg.Key.Id,
                             ParentMenuName = mmg.Key.Name
                         }).ToList<MenuDisplay>();
            }

            // 권한에 따른 세부메뉴 출력(관리자/최종관리자)
            List<MenuDisplay> subMenuDisplay = new List<MenuDisplay>();

            if (loginedUser.Ismaster && (loginedUser.Level == "2-1" || loginedUser.Level == "시스템관리자"))
            {
                // 최종 관리자(트루인포 관리자)
                subMenuDisplay
                    = (from ua in _context.UserAuth
                       join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                       where
                            ua.Userid == HttpContext.Session.GetString("userId")
                            && sm.Isused == true
                       select new MenuDisplay
                       {
                           SubMenuId = sm.Id,
                           SubMenuName = sm.Name,
                           MenuUrl = sm.Menuurl,
                           ParentMenuId = sm.Mainmenuid,
                           IsManager = sm.Ismanager
                       }).ToList<MenuDisplay>();
            }
            else if (loginedUser.Level == "2-2" || loginedUser.Level == "CRM관리자")
            {
                // CRM 관리자(업체 관리자)
                subMenuDisplay
                    = (from ua in _context.UserAuth
                       join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                       where
                            ua.Userid == HttpContext.Session.GetString("userId")
                            && sm.Isused == true
                            && sm.Ismanager == false
                            && ua.Read != "4-3"
                       select new MenuDisplay
                       {
                           SubMenuId = sm.Id,
                           SubMenuName = sm.Name,
                           MenuUrl = sm.Menuurl,
                           ParentMenuId = sm.Mainmenuid,
                           IsManager = sm.Ismanager
                       }).ToList<MenuDisplay>();
            }
            else
            {
                // 일반 사용자 
                subMenuDisplay
                    = (from ua in _context.UserAuth
                       join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                       where
                            ua.Userid == HttpContext.Session.GetString("userId")
                            && sm.Isused == true
                            && sm.Ismanager == false
                            && sm.Mainmenuid != 7
                            && ua.Read != "4-3"
                       select new MenuDisplay
                       {
                           SubMenuId = sm.Id,
                           SubMenuName = sm.Name,
                           MenuUrl = sm.Menuurl,
                           ParentMenuId = sm.Mainmenuid,
                           IsManager = sm.Ismanager
                       }).ToList<MenuDisplay>();
            }

            ViewBag.SubMenuList = subMenuDisplay;
            ViewBag.UserName = HttpContext.Session.GetString("userName");

            return View(query);
        }
    }
}
