using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using PodoDemo.Models.InnerModels;
using PodoDemo.Models;
using Newtonsoft.Json;

namespace PodoDemo.Controllers
{
    [Produces("application/json")]
    [Route("api/CommonAPI")]
    public class CommonAPIController : Controller
    {
        private readonly PodoDemoNContext _context;

        public CommonAPIController(PodoDemoNContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 사용자 권한 체크
        /// </summary>
        /// <param name="userId">현재 로그인한 사용자 아이디</param>
        /// <param name="submenuId">권한을 확인하고자하는 세부메뉴아이디</param>
        /// <returns></returns>
        public UserAuth CheckUseauth(string userId, string submenuId)
        {
            UserAuth _userauth =
                _context.UserAuth
                .Where(x => x.Userid.Equals(userId) && x.Submenuid.Equals(submenuId))
                .SingleOrDefault();

            return _userauth;
        }

        /// <summary>
        /// 해당 키값에 맞는 옵션셋 가져오기
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetOptionDDL")]
        public List<DDL> GetOptionDDL([FromBody]DDL input)
        {
            List<DDL> list = new List<DDL>();
            var optionList = _context.OptionMasterDetail.Where(x => x.Isused == true && x.Masterid == Convert.ToDouble(input.SearchKey)).ToList();
            foreach (OptionMasterDetail item in optionList)
            {
                DDL data = new DDL()
                {
                    Value = item.Optionid,
                    Text = item.Name
                };
                list.Add(data);
            }
            return list;
        }

        /// <summary>
        /// 현재 가입되어 있는 사용자 목록 전부 가져오기
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetJoinedUserDDL")]
        public List<DDL> GetJoinedUserDDL([FromBody]DDL input)
        {
            User loginedUser
                = _context.User
                            .Where(x => x.Id == HttpContext.Session.GetString("userId"))
                            .Single();

            List<User> joinedUserList = new List<Models.User>();

            // 시스템 관리자와 일반 관리자가 가져오는 내용이 다르다.
            if (loginedUser.Level == "2-1" || loginedUser.Level == "시스템관리자" )
            {
                joinedUserList
                    = _context.User
                        .Where(x => x.Department == input.SearchKey || input.SearchKey == "")
                        .ToList();
                List<DDL> userDDLList = new List<DDL>();

                foreach (User item in joinedUserList)
                {
                    DDL tempDDL = new DDL()
                    {
                        Text = item.Name,
                        Value = item.Id
                    };
                    userDDLList.Add(tempDDL);
                }

                return userDDLList;
            }
            else
            {
                // 사용자 중 CRM ADMIN은 가져오지 않는다.
                joinedUserList
                    = _context.User
                        .Where(x => x.Level != "2-1" && x.Ismaster == false)
                        .Where(x => x.Department == input.SearchKey || input.SearchKey == "")
                        .ToList();
                List<DDL> userDDLList = new List<DDL>();

                foreach (User item in joinedUserList)
                {
                    DDL tempDDL = new DDL()
                    {
                        Text = item.Name,
                        Value = item.Id
                    };
                    userDDLList.Add(tempDDL);
                }

                return userDDLList;
            }            
        }

        /// <summary>
        /// 각 메뉴의 URL 가져오기
        /// </summary>
        [HttpPost("GetSubmenuUrl")]
        public string GetSubmenuUrl([FromBody]SubMenu inSubmenu)
        {
            SubMenu _submenu = _context.SubMenu.Where(x => x.Id == inSubmenu.Id).SingleOrDefault();
            return JsonConvert.SerializeObject(_submenu);
        }

        /// <summary>
        /// 현재 생성되어 있는 조직 목록 가져오기
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetOrganizationDDL")]
        public List<DDL> GetOrganizationDDL()
        {
            List<Organization> organizationList = _context.Organization.ToList();
            List<DDL> organizationDDLList = new List<DDL>();

            foreach (Organization item in organizationList)
            {
                DDL tempDDL = new DDL()
                {
                    Text = item.Name,
                    Value = item.Organizationid.ToString()
                };
                organizationDDLList.Add(tempDDL);
            }

            return organizationDDLList;
        }
    }
}