using ErrorAPI.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ErrorAPI.Controllers
{
    public class ErrorController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        /// <summary>
        /// DB Connection 세팅
        /// </summary>
        [ActionName("SetDBConnection")]
        public bool SetDBConnection(string id = "")
        {
            bool setResult = false;
            
            switch (id)
            {
                case "podocm":
                    CommonData.connectionString = ConfigurationManager.ConnectionStrings["podocmDB"].ToString();
                    break;
                case "pododemo":
                    CommonData.connectionString = ConfigurationManager.ConnectionStrings["podoDemoDB"].ToString();
                    break;
                default:
                    break;
            }

            return setResult;
        }

        /// <summary>
        /// 현재 설정되어 있는 DB Connection String 확인
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("GetConnectedString")]
        public string GetConnectedString(string id)
        {
            string connectionString = "ERROR";
            switch (id)
            {
                case "podocm":
                    connectionString = ConfigurationManager.ConnectionStrings["podocmDB"].ToString();
                    break;
                case "pododemo":
                    connectionString = ConfigurationManager.ConnectionStrings["podoDemoDB"].ToString();
                    break;
                default:
                    break;
            }

            return connectionString;
        }
    }
}