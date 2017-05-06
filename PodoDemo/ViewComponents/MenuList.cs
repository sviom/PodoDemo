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

        public void GetMenuList()
        {
            using (SqlConnection con = new SqlConnection(DatabaseUtil._connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand("P_GET_USER_DISPLAY_MEMU_LIST", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.AddWithValue("@V_USER_ID", Session["userID"].ToString());
                cmd.Parameters.AddWithValue("@V_USER_ID", HttpContext.Session.GetString("userId"));

                con.Open();

                IDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                }
                //await _context.Account.ToListAsync()

                //while (reader.Read())
                //{
                //    UserDisplayMenu data = new UserDisplayMenu();
                //    //data.ParentMenuID = int.Parse(reader["PARENT_ID"].ToString());
                //    //data.ParentMenuName = reader["PARENT_NAME"].ToString();
                //    //data.ParentMenuOffIcon = reader["iconOffFile"].ToString();
                //    //data.ParentMenuOnIcon = reader["iconOnFile"].ToString();
                //    //data.SubMenuID = reader["CHILD_ID"].ToString();
                //    //data.SubMenuName = reader["CHILD_NAME"].ToString();
                //    //data.SubMenuURL = reader["menuURL"].ToString();
                //    if (reader["PARENT_NAME"].ToString().IndexOf("관리") > -1)
                //    {
                //        if (Session["userLevel"] != null)
                //        {
                //            //시스템 관리자
                //            if (Session["userLevel"].ToString() == "13-2")
                //            {
                //                data.ParentMenuID = int.Parse(reader["PARENT_ID"].ToString());
                //                data.ParentMenuName = reader["PARENT_NAME"].ToString();
                //                data.ParentMenuOffIcon = reader["iconOffFile"].ToString();
                //                data.ParentMenuOnIcon = reader["iconOnFile"].ToString();
                //                data.SubMenuID = reader["CHILD_ID"].ToString();
                //                data.SubMenuName = reader["CHILD_NAME"].ToString();
                //                data.SubMenuURL = reader["menuURL"].ToString();
                //                data.MenuOrder = int.Parse(reader["order"].ToString());
                //                data.IsPopup = reader["isPopup"].ToString();
                //                _displayMenuList.Add(data);
                //            }
                //            //CRM 관리자
                //            if (Session["userLevel"].ToString() == "13-3")
                //            {
                //                if (reader["CHILD_ID"].ToString() != "7-5" && reader["CHILD_ID"].ToString() != "7-4")
                //                {
                //                    if (!bool.Parse(reader["isManager"].ToString()))
                //                    {
                //                        data.ParentMenuID = int.Parse(reader["PARENT_ID"].ToString());
                //                        data.ParentMenuName = reader["PARENT_NAME"].ToString();
                //                        data.ParentMenuOffIcon = reader["iconOffFile"].ToString();
                //                        data.ParentMenuOnIcon = reader["iconOnFile"].ToString();
                //                        data.SubMenuID = reader["CHILD_ID"].ToString();
                //                        data.SubMenuName = reader["CHILD_NAME"].ToString();
                //                        data.SubMenuURL = reader["menuURL"].ToString();
                //                        data.MenuOrder = int.Parse(reader["order"].ToString());
                //                        data.IsPopup = reader["isPopup"].ToString();
                //                        _displayMenuList.Add(data);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        data.ParentMenuID = int.Parse(reader["PARENT_ID"].ToString());
                //        data.ParentMenuName = reader["PARENT_NAME"].ToString();
                //        data.ParentMenuOffIcon = reader["iconOffFile"].ToString();
                //        data.ParentMenuOnIcon = reader["iconOnFile"].ToString();
                //        data.SubMenuID = reader["CHILD_ID"].ToString();
                //        data.SubMenuName = reader["CHILD_NAME"].ToString();
                //        data.SubMenuURL = reader["menuURL"].ToString();
                //        data.MenuOrder = int.Parse(reader["order"].ToString());
                //        data.IsPopup = reader["isPopup"].ToString();
                //        _displayMenuList.Add(data);
                //    }
                //}
                //Session["alreadyMenu"] = true;
            }
        }
    }
}
