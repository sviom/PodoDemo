using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Common
{
    public static class DatabaseUtil
    {
        public static AppSettings _connString { get; set; }

        /// <summary>
        /// 쿼리를 실행합니다.(값을 반환하지 않습니다.)
        /// </summary>
        /// <param name="query">쿼리 문자열</param>
        public static void ExecuteQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(_connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandTimeout = 600;
                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        /// <summary>
        /// 쿼리를 실행하고, 데이터셋을 반환합니다.
        /// </summary>
        /// <param name="query">Query 문자열</param>
        /// <returns>Query실행 결과 데이터 셋</returns>
        public static DataSet getDataSet(string query)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandTimeout = 600;
                conn.Open();

                SqlDataAdapter ADAP = new SqlDataAdapter(cmd);
                ADAP.Fill(ds);

                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 프로시저와 파라미터를 이용하여 데이터셋 반환
        /// </summary>
        /// <param name="procedureName">프로시저 이름</param>
        /// <param name="parm">파라미터 배열</param>
        /// <returns>쿼리 결과에 따른 데이터셋</returns>
        public static DataSet getDataSet(string procedureName, SqlParameter[] parm)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                if (parm != null)
                {
                    cmd.Parameters.AddRange(parm);
                }
                conn.Open();


                SqlDataAdapter ADAP = new SqlDataAdapter(cmd);

                ADAP.Fill(ds);

                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 프로시저와 단일 파라미터를 이용하여 데이터셋 반환
        /// </summary>
        /// <param name="procedureName">프로시저 이름</param>
        /// <param name="parm">단일 파라미터</param>
        /// <returns>쿼리 결과에 따른 데이터셋</returns>
        public static DataSet getDataSet(string procedureName, SqlParameter parm)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connString.DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                if (parm != null)
                {
                    cmd.Parameters.Add(parm);
                }
                conn.Open();


                SqlDataAdapter ADAP = new SqlDataAdapter(cmd);

                ADAP.Fill(ds);

                conn.Close();
            }
            return ds;
        }
    }
}
