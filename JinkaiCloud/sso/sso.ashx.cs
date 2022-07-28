using Common;
using JinkaiCloud.ajax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace JinkaiCloud.sso
{
    /// <summary>
    /// Summary description for sso
    /// </summary>
    public class sso : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            // 取得code
            string code = context.Request["code"];

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            LogWriter.PutEvtLog("sso.ashx", "sso", "code: " + code + "");

            //StringBuilder result = new StringBuilder();
            //result.Append("code: " + code + "\n");

            // 获取token
            string ssoUrl = ConfigurationManager.AppSettings["ssoUrl"];

            string appId = ConfigurationManager.AppSettings["appcode"];
            string appKey = ConfigurationManager.AppSettings["OAuthSecret"];

            // 测试TOKEN地址
            //string getTokenUrl = ssoUrl + "/token.json?appcode=" + appId + "&secret=" + appKey + "&code=" + code;
            string getTokenUrl = ssoUrl + "/token?appcode=" + appId + "&secret=" + appKey + "&code=" + code;


            byte[] tokenResponse = HttpUtils.HttpGet(getTokenUrl, 15);

            if (null == tokenResponse)
            {
                // 请求失败
                context.Response.Write(ErrorMsg("请求" + getTokenUrl + "失败"));
                return;
            }

            string tokenJsonText = Encoding.UTF8.GetString(tokenResponse);
            JObject tokenResult = (JObject)JsonConvert.DeserializeObject(tokenJsonText);

            string token = tokenResult["accessToken"].ToString();

            // 测试TOKEN地址
            //string getUserUrl = ssoUrl + "/userinfo.json?appcode=" + appId + "&secret=" + appKey + "&token=" + token;
            string getUserUrl = ssoUrl + "/userinfo?appcode=" + appId + "&secret=" + appKey + "&token=" + token;

            byte[] userResponse = HttpUtils.HttpGet(getUserUrl, 15);

            if (null == userResponse)
            {
                // 请求失败
                context.Response.Write(ErrorMsg("请求" + getUserUrl + "失败"));
                return;
            }

            string userJsonText = Encoding.UTF8.GetString(userResponse);
            JObject userResult = (JObject)JsonConvert.DeserializeObject(userJsonText);

            if (userResult["errorCode"] != null && userResult["errorCode"].ToString() != "")
            {
                LogWriter.PutErrLog("sso.ashx", "sso", userJsonText);
                context.Response.Write(userResult["errorMsg"].ToString());
                return;
            }
            // 用户名
            string accountName = userResult["accountName"].ToString();

            // 用户登录模块
            //AuthUserController controller = new AuthUserController();

            string errMsg = "";
            string password = "";
            //Model.auth_user model = controller.OauthUserLogin(accountName, ref password, ref errMsg);
            //if (model != null)
            //{
            //    // 写入Cookie
            //    Utils.WriteCookie(ManagePage.COOKIE_ADMIN_USERNAME, accountName);

            //    Utils.WriteCookie(ManagePage.COOKIE_ADMIN_PASSWORD, password);
            //    // 写入Session
            //    context.Session[ManagePage.SESSION_ADMIN_INFO] = model;
            //    context.Session.Timeout = 120;
            //    // 更新最后登录时间
            //    controller.UpdateLastLogin();
            //    // 添加日志文件
            //    string first_name = string.IsNullOrEmpty(model.first_name) ? "" : model.first_name;
            //    new ManagePage().AddAdminLog("登录", Utils.GetClientIP(), 1);

                //result.Append("token: " + token + "\nuserinfo: " + userJsonText);

                //string rst = result.ToString().Replace("/\n", "<br/>");
                context.Response.Redirect("/Index.html");
                //context.Response.Write(rst);
                return;
            //}
           // else
            //{
               // context.Response.Write(errMsg);
               // return;
            //}
        }
        public string ErrorMsg(string msg)
        {
            return msg;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}