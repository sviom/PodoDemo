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
using PodoDemo.Models;

namespace PodoDemo.Controllers
{
    public class HomeController : Controller
    {
        //private AppSettings _connString { get; set; }           // Connection String 얻어오기
        //public HomeController(IOptions<AppSettings> _settings)  // Connection String 얻어오기
        //{
        //    _connString = _settings.Value;
        //    DatabaseUtil._connString = _settings.Value;
        //}
        private readonly PodoDemoNContext _context;
        public HomeController(PodoDemoNContext context)
        {
            _context = context;
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
        /// 사용자 체크
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userPW"></param>
        [HttpPost]
        public ActionResult CheckUser(string userID, string userPW)
        {
            if (!_CheckUser(userID, userPW))
            {
                return RedirectToAction("Error", "Home", new { errormessage = "UserNotFound" });
            }
            return RedirectToAction("Index", "Accounts");          // 사용자 존재 시 거래처 목록으로 이동
        }

        /// <summary>
        /// 사용자 체크 기능 수행
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userPW"></param>
        /// <returns></returns>
        private bool _CheckUser(string userID, string userPW)
        {
            bool userCheckResult = false;
            try
            {
                User userResult = _context.User.Where(x => x.Id == userID && x.Pw == userPW).Single();
                if (userResult != null)
                {
                    HttpContext.Session.SetString("userId", userResult.Id);
                    HttpContext.Session.SetString("userName", userResult.Name);
                    userCheckResult = true;
                }
                else
                {
                    userCheckResult = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            if (errormessage == "UserNotFound")
            {
                ViewBag.message = errormessage;
            }
            else if (errormessage == "UserauthError")
            {
                ViewBag.message = errormessage;
            }

            return View();
        }

        /// <summary>
        /// 사용자 로그아웃
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("userId");
                HttpContext.Session.Remove("userName");
            }
            catch (Exception ex)
            {
                throw;
            }

            return Redirect("/Home/Index");
        }
    }
}