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

namespace PodoDemo.Controllers
{
    [Produces("application/json")]
    [Route("api/CommonAPI")]
    public class CommonAPIController : Controller
    {
        /// <summary>
        /// 해당 키값에 맞는 옵션셋 가져오기
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
        /// 현재 가입되어 있는 사용자 목록 전부 가져오기
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetJoinedUserDDL")]
        public List<DDL> GetJoinedUserDDL([FromBody]DDL input)
        {
            using (SqlConnection con = new SqlConnection(DatabaseUtil._connString.DBConnectionString))
            {
                List<DDL> list = new List<DDL>();
                SqlCommand cmd = new SqlCommand("P_Get_UserList", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Department", input.SearchKey);
                cmd.Parameters.AddWithValue("@UserId", input.Userid);

                con.Open();

                IDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    DDL data = new DDL()
                    {
                        Value = reader["Value"].ToString(),
                        Text = reader["Text"].ToString()
                    };

                    list.Add(data);
                }
                con.Close();

                return list;
            }
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