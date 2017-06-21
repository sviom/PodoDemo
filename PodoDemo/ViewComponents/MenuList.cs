using PodoDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using PodoDemo.Common;
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
            //GetMenuList();
            List<MenuDisplay> query = (from ua in _context.UserAuth
                                       join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                                       join mm in _context.Menu on sm.Mainmenuid equals mm.Id
                                       where
                                            ua.Userid == HttpContext.Session.GetString("userId")
                                            && mm.Isused == true
                                            && sm.Isused == true
                                            && sm.Isdeleted == false
                                       //&& ua.Read != "1-1"
                                       //orderby mm.Order ascending, sm.Order ascending
                                       group mm by new
                                       {
                                           mm.Id,
                                           mm.Name
                                       } into mmg
                                       select new MenuDisplay
                                       {
                                           ParentMenuId = mmg.Key.Id,
                                           ParentMenuName = mmg.Key.Name
                                           //SubMenuId = sm.Id,
                                           //SubMenuName = sm.Name,
                                           //MenuOrder = mmg.Order
                                           //MenuUrl = sm.Menuurl
                                       }).ToList<MenuDisplay>();

            ViewBag.UserName = HttpContext.Session.GetString("userName");

            List<MenuDisplay> subMenuDisplay = (from ua in _context.UserAuth
                                                 join sm in _context.SubMenu on ua.Submenuid equals sm.Id
                                                 where ua.Userid == HttpContext.Session.GetString("userId")
                                                 && sm.Isused == true
                                                 select new MenuDisplay
                                                 {
                                                     SubMenuId = sm.Id,
                                                     SubMenuName = sm.Name,
                                                     MenuUrl = sm.Menuurl,
                                                     ParentMenuId = sm.Mainmenuid
                                                 }).ToList<MenuDisplay>();

            ViewBag.SubMenuList = subMenuDisplay;

            return View(query);
        }        

        
    }
}
