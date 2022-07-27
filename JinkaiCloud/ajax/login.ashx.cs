using Common;
using Controller;
using System;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// 管理员登陆的摘要说明
    /// </summary>
    public class login : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            try
            {
                switch (action)
                {
                    // 管理员登陆
                    case "login":
                        context.Response.Write(Login(context));
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                context.Response.Write(JsonHelp.ErrorJson(msg));
            }
        }

        /// <summary>
        /// 用户登陆执行的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Login(HttpContext context)
        {
            string username = context.Request["username"];
            string password = context.Request["userpwd"];

            if (string.IsNullOrEmpty(username))
            {
                return JsonHelp.ErrorJson("用户名不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return JsonHelp.ErrorJson("密码不能为空");
            }

            //验证码验证
            string txtcode = context.Request["txtcode"];
            if (string.IsNullOrEmpty(txtcode))
            {
                return JsonHelp.ErrorJson("验证码不能为空");
            }

            if (context.Session[ManagePage.SESSION_CODE] == null)
            {
                return JsonHelp.ErrorJson("验证码已过期");
            }
            string code = Utils.MD5(context.Session[ManagePage.SESSION_CODE].ToString().ToLower());//图片验证码

            //统一转为小写比较
            if (!txtcode.Equals(code))
            {
                return JsonHelp.ErrorJson("验证码不正确");
            }

            AdminController controller = new AdminController();
            Model.Admin model = controller.UserLogin(username, password);
            if (model != null)
            {
                int status = model.status;
                if (status == 0)
                {
                    return JsonHelp.ErrorJson("账号已经被停用");
                }

                // 写入Cookie
                Utils.WriteCookie(ManagePage.COOKIE_USERNAME, username);
                Utils.WriteCookie(ManagePage.COOKIE_PASSWORD, password);

                // 写入Session
                context.Session[ManagePage.SESSION_ADMIN_INFO] = model;
                context.Session.Timeout = 120;
                // 更新用户登录时间
                controller.UpdateLoginTime(model.id, Utils.GetClientIP());
                // 添加日志文件
                new ManagePage().AddAdminLog("PC登陆成功 用户名：" + username, Constant.ActionEnum.Login);

                return JsonHelp.SuccessJson("登录成功");
            }
            else
            {
                // 添加日志文件
                new ManagePage().ErrorLoginLog("用户名或密码错误 用户名：" + username, username);
                return JsonHelp.ErrorJson("用户名或密码错误");
            }
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