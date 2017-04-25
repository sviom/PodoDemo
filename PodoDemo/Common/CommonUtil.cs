using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Common
{
    public class CommonUtil
    {
        //private static AppSettings _connString { get; set; }
        //public CommonUtil(AppSettings _settings)
        //{
        //    _connString = _settings;
        //}

        /// <summary>
        /// 현재 페이지의 사용자 권한을 반환합니다.
        /// </summary>
        /// <param name="userID">로그인한 사용자 ID</param>
        /// <param name="currentURL">현재 URL Path(Request.Url.PathAndQuery를 이용하여 반환된 값)</param>
        /// <returns>권한 배열(읽기,쓰기,수정,삭제)</returns>
        public static List<string> CurrentPageAuthCheck(string userID, string currentURL)
        {
            currentURL = currentURL.Substring(0, currentURL.IndexOf("?") > -1 ? currentURL.IndexOf("?") : currentURL.Length);
            List<string> authList = new List<string>();

            SqlParameter[] param = new SqlParameter[]
            {
            new SqlParameter("@V_USER_ID", SqlDbType.NVarChar)
            , new SqlParameter("@V_URL_PATH", SqlDbType.NVarChar)
            };
            param[0].Value = userID;
            param[1].Value = currentURL;

            DataSet authResult = DatabaseUtil.getDataSet("P_GET_URLPATH_AUTH", param);

            string _isRead = "5-1";
            string _isWrite = "5-1";
            string _isModify = "5-1";
            string _isDelete = "5-1";

            bool _nullCheck = false;

            if (authResult.Tables[0].Rows.Count > 0)
            {
                _nullCheck = string.IsNullOrEmpty(authResult.Tables[0].Rows[0]["READ_AUTH"].ToString()) ? true : false;
                _isRead = _nullCheck ? _isRead : authResult.Tables[0].Rows[0]["READ_AUTH"].ToString();

                _nullCheck = string.IsNullOrEmpty(authResult.Tables[0].Rows[0]["WRIT_AUTH"].ToString()) ? true : false;
                _isWrite = _nullCheck ? _isWrite : authResult.Tables[0].Rows[0]["WRIT_AUTH"].ToString();

                _nullCheck = string.IsNullOrEmpty(authResult.Tables[0].Rows[0]["MODIFY_AUTH"].ToString()) ? true : false;
                _isModify = _nullCheck ? _isModify : authResult.Tables[0].Rows[0]["MODIFY_AUTH"].ToString();

                _nullCheck = string.IsNullOrEmpty(authResult.Tables[0].Rows[0]["DELETE_AUTH"].ToString()) ? true : false;
                _isDelete = _nullCheck ? _isDelete : authResult.Tables[0].Rows[0]["DELETE_AUTH"].ToString();
            }

            authList.Add(_isRead);
            authList.Add(_isWrite);
            authList.Add(_isModify);
            authList.Add(_isDelete);

            return authList;
        }

        /// <summary>
        /// 현재 사용자의 모든 페이지 권한을 반환합니다.
        /// </summary>
        /// <param name="userID">로그인한 사용자 ID</param>
        /// <returns>권한 배열(읽기,쓰기,수정,삭제)</returns>
        //public static List<Auth> GetAuth_ALL(string UserID)
        //{
        //    SqlParameter[] param = new SqlParameter[]
        //    {
        //    new SqlParameter("@V_USER_ID", SqlDbType.NVarChar)
        //    };
        //    param[0].Value = UserID;

        //    DataSet authResult = DatabaseUtil.getDataSet("P_GET_AUTH_ALL", param);


        //    List<Auth> list = new List<Auth>();
        //    string _default = "5-1";
        //    bool _nullCheck = false;

        //    foreach (DataRow dr in authResult.Tables[0].Rows)
        //    {
        //        Auth d = new Auth();

        //        d.UserID = dr["userID"].ToString();

        //        d.SubMenuID = dr["subMenuID"].ToString();

        //        _nullCheck = string.IsNullOrEmpty(dr["READ_AUTH"].ToString()) ? true : false;
        //        d.ReadAuth = _nullCheck ? _default : dr["READ_AUTH"].ToString();

        //        _nullCheck = string.IsNullOrEmpty(dr["WRIT_AUTH"].ToString()) ? true : false;
        //        d.WritAuth = _nullCheck ? _default : dr["WRIT_AUTH"].ToString();

        //        _nullCheck = string.IsNullOrEmpty(dr["MODIFY_AUTH"].ToString()) ? true : false;
        //        d.ModifyAuth = _nullCheck ? _default : dr["MODIFY_AUTH"].ToString();

        //        _nullCheck = string.IsNullOrEmpty(dr["DELETE_AUTH"].ToString()) ? true : false;
        //        d.DeleteAuth = _nullCheck ? _default : dr["DELETE_AUTH"].ToString();

        //        list.Add(d);
        //    }

        //    return list;
        //}

        /// <summary>
        /// 현재 페이지의 URL을 바탕으로 SUB메뉴의 ID값을 가져옵니다.
        /// </summary>
        /// <param name="URL">현재 페이지 URL</param>
        /// <returns></returns>
        public static string GetCurrentPageSubMenuID(string URL)
        {
            string id = "";

            SqlParameter[] param = new SqlParameter[]
            {
            new SqlParameter("@FILENAME", SqlDbType.VarChar)
            };
            param[0].Value = URL;

            DataSet Result = DatabaseUtil.getDataSet("P_GET_SUBMENUID_BY_FILENAME", param);

            foreach (DataRow dr in Result.Tables[0].Rows)
            {
                id = dr["id"].ToString();
            }

            return id;
        }
    }
}
