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
            using (SqlConnection con = new SqlConnection(DatabaseUtil._connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand("P_Get_Option", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@V_MASTERID", input.SearchKey);

                con.Open();

                IDataReader reader = cmd.ExecuteReader();

                List<DDL> list = new List<DDL>();
                while (reader.Read())
                {
                    DDL data = new DDL()
                    {
                        Value = reader["Value"].ToString(),
                        Text = reader["Text"].ToString(),
                        IsDefault = Convert.ToBoolean(reader["isDefault"].ToString())
                    };
                    list.Add(data);
                }

                return list;
            }
        }

        /// <summary>
        /// ���� ���ԵǾ� �ִ� ����� ��� ���� ��������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetJoinedUserDDL")]
        public List<DDL> GetJoinedUserDDL([FromBody]DDL input)
        {
            //using (SqlConnection con = new SqlConnection(DatabaseUtil._connString.DBConnectionString))
            //{
            //    List<DDL> list = new List<DDL>();
            //    SqlCommand cmd = new SqlCommand("P_Get_UserList", con);
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cmd.Parameters.AddWithValue("@Department", input.SearchKey);
            //    cmd.Parameters.AddWithValue("@UserId", input.Userid);

            //    con.Open();

            //    IDataReader reader = cmd.ExecuteReader();


            //    while (reader.Read())
            //    {
            //        DDL data = new DDL()
            //        {
            //            Value = reader["Value"].ToString(),
            //            Text = reader["Text"].ToString()
            //        };

            //        list.Add(data);
            //    }
            //    con.Close();

            //    return list;
            //}

            List<User> joinedUserList 
                = _context.User
                    .Where(x => x.Level != "2-1" && x.Ismaster == false)
                    .Where(x=>x.Department == input.SearchKey || input.SearchKey == "")
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

        [HttpPost("Logout")]
        public bool Logout()
        {
            bool logoutResult = false;
            //HttpContext.Session.SetString("userId", userResult.Tables[0].Rows[0]["id"].ToString());
            //HttpContext.Session.SetString("userName", userResult.Tables[0].Rows[0]["name"].ToString());
            try
            {
                HttpContext.Session.Remove("userId");
                HttpContext.Session.Remove("userName");

                logoutResult = true;
            }
            catch(Exception ex)
            {
                throw;
            }
            
            return logoutResult;
        }
    }
}