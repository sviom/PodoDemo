using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using PodoDemo.Common;
using PodoDemo.Models.InnerModels;
using PodoDemo.Models;

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
        /// ����� ���� üũ
        /// </summary>
        /// <param name="userId">���� �α����� ����� ���̵�</param>
        /// <param name="submenuId">������ Ȯ���ϰ����ϴ� ���θ޴����̵�</param>
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
        /// �ش� Ű���� �´� �ɼǼ� ��������
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
        /// ���� ���ԵǾ� �ִ� ����� ��� ���� ��������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetJoinedUserDDL")]
        public List<DDL> GetJoinedUserDDL([FromBody]DDL input)
        {
            // ����� �� CRM ADMIN�� �������� �ʴ´�.
            List<User> joinedUserList
                = _context.User
                    .Where(x => x.Level != "2-1" && x.Ismaster == false)
                    .Where(x => x.Department == input.SearchKey || input.SearchKey == "")
                    .ToList();
            List<DDL> userDDLList = new List<DDL>();

            foreach (User item in joinedUserList)
            {
                DDL tempDDL = new DDL();
                tempDDL.Text = item.Name;
                tempDDL.Value = item.Id;
                userDDLList.Add(tempDDL);
            }

            return userDDLList;
        }
    }
}