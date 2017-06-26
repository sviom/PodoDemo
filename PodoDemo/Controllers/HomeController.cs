using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Data;
using PodoDemo.Common;

namespace PodoDemo.Controllers
{
    public class HomeController : Controller
    {
        private AppSettings _connString { get; set; }           // Connection String ������
        public HomeController(IOptions<AppSettings> _settings)  // Connection String ������
        {
            _connString = _settings.Value;
            DatabaseUtil._connString = _settings.Value;
        }

        public IActionResult Index(string viewMessage = null)
        {
            if (!string.IsNullOrEmpty(viewMessage))
            {
                ViewBag.viewMessage = viewMessage;
            }
            return View();
        }

        /// <summary>
        /// ����� üũ
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userPW"></param>
        [HttpPost]
        public ActionResult CheckUser(string userID, string userPW)
        {
            if (!_CheckUser(userID, userPW))
            {
                // return Redirect(Request.UrlReferrer.ToString());        // ����� ���� �� �ٽ� ���ư���
                return RedirectToAction("Error","Home", new { errormessage = "UserNotFound" });          // ����� ���� �� �ŷ�ó ������� �̵�
            }
            return RedirectToAction("Index", "Accounts");          // ����� ���� �� �ŷ�ó ������� �̵�
        }

        private bool _CheckUser(string userID, string userPW)
        {
            bool userCheckResult = false;
            try
            {
                SqlParameter[] param = new SqlParameter[]{
                        new SqlParameter(){ ParameterName="@V_USER_ID", Value=userID, SqlDbType=SqlDbType.NVarChar}
                        , new SqlParameter(){ ParameterName="@V_USER_PW",Value=userPW, SqlDbType=SqlDbType.NVarChar}
                    };

                DataSet userResult = DatabaseUtil.getDataSet("P_Get_Login_UserInfo", param);

                if (userResult.Tables[0].Rows.Count == 0)
                {
                    //Response.Write(CommonUtil.JavascriptAlertMessage("���̵�� ��й�ȣ�� Ȯ���� �ּ���."));
                    return userCheckResult;
                }

                //�̷½ױ�
                //SqlParameter[] sqlparams =
                //    {
                //        new SqlParameter("@V_USER_ID", SqlDbType.VarChar),
                //        new SqlParameter("@V_USER_NAME", SqlDbType.NVarChar)
                //    };

                //sqlparams[0].Value = userResult.Tables[0].Rows[0]["id"].ToString();
                //sqlparams[1].Value = userResult.Tables[0].Rows[0]["name"].ToString();

                //DataSet results = DatabaseUtil.getDataSet("P_CREATE_LOG_LOGIN", sqlparams);

                HttpContext.Session.SetString("userId", userResult.Tables[0].Rows[0]["id"].ToString());
                HttpContext.Session.SetString("userName", userResult.Tables[0].Rows[0]["name"].ToString());

                //Session["userName"] = userResult.Tables[0].Rows[0]["name"];
                //Session["userLevel"] = userResult.Tables[0].Rows[0]["level"];
                //Session["userLevelText"] = userResult.Tables[0].Rows[0]["LEVEL_TEXT"];
                //Session["userIsMaster"] = userResult.Tables[0].Rows[0]["isMaster"];
                //Session["userImageURL"] = userResult.Tables[0].Rows[0]["userImageURL"];
                //Session["userDept"] = userResult.Tables[0].Rows[0]["department"];
                //Session["userDeptText"] = userResult.Tables[0].Rows[0]["DEPAREMENT_TEXT"];
                //Session["userExcelAuth"] = userResult.Tables[0].Rows[0]["excelAuth"];
                //Session["alreadyMenu"] = false;
                //Session.Timeout = 60 * 9;

                //Session["SERVER_HOST_NAME"] = Request.Url.Host;

                ////Response.Cookies["SERVER_HOST_NAME"].Value = Request.Url.Host;

                //List<Auth> auth = CommonUtil.GetAuth_ALL(userResult.Tables[0].Rows[0]["id"].ToString());
                //Session["userAuth"] = JsonConvert.SerializeObject(auth);

                userCheckResult = true;

                //Response.Redirect(string.Format("/Default.aspx"));
                //Response.Redirect("/Views/Account/AccountList", false);
                //RedirectToAction("AccountList", "Account");
            }
            catch (Exception ex)
            {
                //Log.ErrorMessage(ex, string.Format("[����] {0}", ex.Message));
            }

            return userCheckResult;
        }
        
        public IActionResult Close()
        {
            return View();
        }

        public IActionResult Error(string errormessage)
        {
            ViewBag.message = "Error";
            if(errormessage == "UserNotFound")
            {
                ViewBag.message = errormessage;
            }
            else if (errormessage == "UserauthError")
            {
                ViewBag.message = errormessage;
            }

            return View();
        }
    }
}