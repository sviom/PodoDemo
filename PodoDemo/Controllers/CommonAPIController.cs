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
    }
}