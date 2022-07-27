using Common;
using Controller;
using System.Collections.Generic;

namespace JinkaiCloud.ajax
{
    public class ManagePage : System.Web.UI.Page
    {
        public static string SESSION_ADMIN_INFO = "jinkai_erp_admin";
        public static string SESSION_CODE = "jinkai_erp_code";

        public static string COOKIE_USERNAME = "jinkai_erp_username";
        public static string COOKIE_PASSWORD = "jinkai_erp_password";

        #region 管理员============================================
        /// <summary>
        /// 判断管理员是否已经登录(解决Session超时问题)
        /// </summary>
        public bool IsAdminLogin()
        {
            //如果Session为Null
            if (Session[SESSION_ADMIN_INFO] != null)
            {
                return true;
            }

            //检查Cookies
            string username = Utils.GetCookie(COOKIE_USERNAME);
            string password = Utils.GetCookie(COOKIE_PASSWORD);
            if (username != "" && password != "")
            {
                AdminController controller = new AdminController();
                Model.Admin model = controller.UserLogin(username, password);
                if (model != null)
                {
                    Session[SESSION_ADMIN_INFO] = model;
                    Session.Timeout = 120;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 取得管理员信息
        /// </summary>
        public Model.Admin GetAdminInfo()
        {
            if (IsAdminLogin())
            {
                Model.Admin model = Session[SESSION_ADMIN_INFO] as Model.Admin;
                if (model != null)
                {
                    return model;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得用户ID
        /// </summary>
        /// <returns></returns>
        public string GetUserID()
        {
            if (IsAdminLogin())
            {
                Model.Admin model = Session[SESSION_ADMIN_INFO] as Model.Admin;
                if (model != null)
                {
                    return model.id;
                }
            }
            return "0";
        }
        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="module"></param>
        /// <param name="action"></param>
        /// <returns>true 具有权限， false 没有权限</returns>
        public bool VerifyRight(string module, string action)
        {
            if (string.IsNullOrEmpty(module))
            {
                return false;
            }
            Model.Admin admin = GetAdminInfo();
            if (admin.roleId == "0" || admin.roleType == 1)
            {
                return true;
            }
            List<string> rights = admin.rights;
            for (int i = 0; i < rights.Count; i++)
            {
                if (rights[i].Equals(module + "_" + action))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="module"></param>
        /// <param name="action"></param>
        /// <returns>true 具有权限， false 没有权限</returns>
        public bool VerifyRight(string module, Constant.ActionEnum action)
        {
            if (string.IsNullOrEmpty(module))
            {
                return false;
            }
            Model.Admin admin = GetAdminInfo();
            if (admin.roleId == "0" || admin.roleType == 1)
            {
                return true;
            }
            List<string> rights = admin.rights;
            for (int i = 0; i < rights.Count; i++)
            {
                if (rights[i].Equals(module + "_" + action.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 写入管理日志
        /// </summary>
        /// <param name="action_type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddAdminLog(string remark, Constant.ActionEnum action = Constant.ActionEnum.Edit, Constant.InfoEnum info = Constant.InfoEnum.Noraml)
        {

            Model.Admin model = GetAdminInfo();
            if (model != null)
            {
                LogsController log = new LogsController();
                bool result = log.Add(remark, action, model, info);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ErrorLoginLog(string remark, string userName)
        {

            LogsController log = new LogsController();
            Model.Admin model = new Model.Admin();
            model.username = userName;
            model.id = "0";
            bool result = log.Add(remark, Constant.ActionEnum.Login, model, Constant.InfoEnum.Error);
            return result;
        }
        #endregion

    }
}
