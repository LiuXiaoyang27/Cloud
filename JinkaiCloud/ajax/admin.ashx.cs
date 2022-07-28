using Common;
using Controller;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace JinkaiCloud.ajax
{
    /// <summary>
    /// 管理员以及权限设置的摘要说明
    /// </summary>
    public class admin : IHttpHandler, IRequiresSessionState
    {
        // 保存用户信息
        private Model.Admin admins = null;
        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            ManagePage page = new ManagePage();
            if (!page.IsAdminLogin())
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }
            else
            {
                admins = page.GetAdminInfo();
            }
            string action = context.Request["action"];
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            try
            {
                switch (action)
                {

                    // 选择用户显示
                    case "init":
                        context.Response.Write(GetInitJson());
                        break;
                    // 获取用户信息
                    case "index":
                        context.Response.Write(GetIndexJson(context));
                        break;
                    // 列表显示
                    case "list":
                        context.Response.Write(GetListJson(context));
                        break;
                    case "treeView":
                        context.Response.Write(GetUserTreeView(context));
                        break;
                    // 查询详情信息
                    case "query":
                        context.Response.Write(GetModel(context));
                        break;
                    // 管理员退出登陆
                    case "loginOut":
                        context.Response.Write(LoginOut(context));
                        break;
                    // 新增用户
                    case "add":
                        context.Response.Write(Add(context));
                        break;
                    // 根据用户名判断用户是否存在
                    case "checkUserName":
                        context.Response.Write(CheckUserName(context));
                        break;
                    // 根据用户名判断用户是否存在
                    case "checkMobile":
                        context.Response.Write(CheckMobile(context));
                        break;
                    // 启用和停用接口
                    case "disable":
                        context.Response.Write(Disable(context));
                        break;
                    // 修改登录用户密码
                    case "updatePassword":
                        context.Response.Write(UpdateUserPassword(context));
                        break;
                    // 重置用户密码
                    case "resetPassword":
                        context.Response.Write(ResetUserPassword(context));
                        break;
                    // 删除一条用户数据
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 修改一条用户数据
                    case "update":
                        context.Response.Write(UpdateAdmin(context));
                        break;
                    // 上传管理员头像
                    case "avatar":
                        context.Response.Write(UploadAvatar(context));
                        break;
                    // 修改登录用户的个人信息
                    case "updateUserInfo":
                        context.Response.Write(UpdateLoginUser(context));
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
        /// 修改一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string UpdateAdmin(HttpContext context)
        {
            string module = "authority";
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("请求参数错误");
            }
            string errMsg = "";
            Model.Admin model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            model.id = id;
            model.modifyUserId = admins.id;
            AdminController controller = new AdminController();
            bool result = controller.UpdateAdmin(model);
            if (result)
            {
                new ManagePage().AddAdminLog("修改用户 用户名:" + model.username, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson("修改用户成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改用户失败");
            }
        }

        /// <summary>
        /// 将获得管理员的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetInitJson()
        {
            AdminController controller = new AdminController();
            DataSet data = controller.GetList(" and t1.status = 1");
            string json = JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), 1, 1);
            return json;
        }
        /// <summary>
        /// 根据ID查询详情信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetModel(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("请求参数错误");
            }
            AdminController controller = new AdminController();
            Model.Admin model = controller.GetUserModel(id);
            if (model == null)
            {
                return JsonHelp.ErrorJson("不存在该人员信息");
            }
            return JsonHelp.SuccessJson(model);
        }

        /// <summary>
        /// 获取登陆用户信息
        /// </summary>
        /// <returns></returns>
        public string GetIndexJson(HttpContext context)
        {
            object obj = context.Session[ManagePage.SESSION_ADMIN_INFO];
            if (obj == null)
            {
                return JsonHelp.ErrorJson("登录失败，请重新登录。");
            }
            else
            {
                AdminController controller = new AdminController();
                Admin model = controller.GetUserModel(((Admin)obj).id);

                JObject data = new JObject();
                data["userProvider"] = GetUserProvider(model);
                data["authorizeMenu"] = GetUserMenu();
                data["userTree"] = GetUserTreeView();
                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = "success";
                result["data"] = data;

                return result.ToString();
            }
        }
        /// <summary>
        /// 获得用户树形结构数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JArray GetUserTreeView()
        {
            AdminController controller = new AdminController();
            List<Dept> listOrg = new DeptController().GetListModel();
            List<Admin> listUsers = GetListModel(" and t1.status = 1");
            JArray result = controller.GetUserTreeView(listOrg, listUsers);
            return result;
        }
        public string GetUserTreeView(HttpContext context)
        {
            string include = context.Request["include"];
            //获取当前登陆用户
            string uid = new ManagePage().GetAdminInfo().id;
            string strWhere = "  and t1.status = 1 ";
            if (include == "0")
            {
                strWhere += " and t1.id <> '" + uid + "'";
            }
            AdminController controller = new AdminController();
            List<Dept> listOrg = new DeptController().GetListModel();
            List<Admin> listUsers = GetListModel(strWhere);
            JArray result = controller.GetUserTreeView(listOrg, listUsers);
            return JsonHelp.SuccessJson(result); ;
        }

        /// <summary>
        /// 获取实体类集合 
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Admin> GetListModel(string strWhere = "")
        {
            AdminController controller = new AdminController();
            DataTable dt = controller.GetList(strWhere).Tables[0];
            List<Admin> list = new List<Admin>();
            foreach (DataRow dataRow in dt.Rows)
            {
                list.Add(controller.GetListModel(dataRow));
            }
            return list;
        }

        /// <summary>
        /// 获得用户个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JObject GetUserProvider(Model.Admin model)
        {
            JObject userProvider = new JObject();
            userProvider["userAccount"] = model.username;
            userProvider["userName"] = model.name;
            userProvider["userId"] = model.id;
            userProvider["roleId"] = model.roleId;
            userProvider["deptId"] = model.deptId;
            userProvider["deptName"] = model.deptName;
            userProvider["mobile"] = model.mobile;
            userProvider["roleName"] = model.roleName;
            userProvider["avatar"] = model.avatar;
            userProvider["creatorTime"] = model.creatorTime;
            userProvider["description"] = model.description;
            userProvider["gender"] = model.gender;
            userProvider["nation"] = model.nation;
            userProvider["nativeplace"] = model.nativeplace;
            userProvider["entryDate"] = model.entryDate;
            userProvider["certificatesType"] = model.certificatesType;
            userProvider["certificatesNumber"] = model.certificatesNumber;
            userProvider["email"] = model.email;
            userProvider["education"] = model.education;
            userProvider["birthday"] = model.birthday;
            userProvider["telephone"] = model.telephone;
            userProvider["landLine"] = model.landLine;
            userProvider["postalAddress"] = model.postalAddress;
            if (model.prevLoginTime != null && model.prevLoginTime != "")
            {
                userProvider["prevLogin"] = 1;
                userProvider["prevLoginTime"] = model.prevLoginTime;
                userProvider["prevLoginIPAddress"] = model.prevLoginIPAddress;
            }

            if (model.roleType == 1 || model.roleId == "0")
            {
                userProvider["isAdmin"] = true;
            }
            else
            {
                userProvider["isAdmin"] = false;
                // todo
                JObject rights = new AdminController().GetUserRightsJson(model.roleId);
                userProvider["rights"] = rights;
            }
            return userProvider;
        }

        /// <summary>
        /// 获得用户的菜单信息
        /// </summary>
        /// <returns></returns>
        private JArray GetUserMenu()
        {

            //获得当前登录管理员信息
            Model.Admin adminModel = new ManagePage().GetAdminInfo();
            string roleId = adminModel.roleId;
            int roleType = adminModel.roleType;

            DataSet listMenus = null;
            MenuController controller = new MenuController();
            // 超级管理员 拥有全部权限
            if (roleType == 1 || roleId == "0")
            {
                listMenus = controller.GetList(" and status = 1");
            }
            else
            {
                // 根据角色查询用户权限
                listMenus = controller.GetRoleList(roleId);
            }
            return GetMenuJsonList(listMenus.Tables[0]);
        }

        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetMenuJsonObj(DataRow dataRow)
        {
            JObject item = new JObject();
            item["Id"] = dataRow["id"].ToString() + "";
            item["FullName"] = dataRow["name"].ToString();
            item["ParentId"] = dataRow["parentId"].ToString();
            //item["Path"] = dataRow["cPath"].ToString();
            item["Type"] = int.Parse(dataRow["navType"].ToString());
            item["Icon"] = "fa " + dataRow["fontIcon"].ToString();
            item["module"] = dataRow["module"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["ordnum"] = int.Parse(dataRow["ordnum"].ToString());
            item["UrlAddress"] = dataRow["linkUrl"].ToString();
            item["typeNumber"] = dataRow["typeNumber"].ToString();
            item["LinkTarget"] = dataRow["linkTarget"].ToString();
            item["Category"] = "Web";

            //item["detail"] = dataRow["detail"].ToString();
            item["remark"] = dataRow["remark"].ToString();
            item["modifyTime"] = DateTime.Parse(dataRow["modifyTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            return item;
        }

        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetMenuJsonList(DataTable dt)
        {
            JArray items = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(GetMenuJsonObj(dt.Rows[i]));
            }
            return items;
        }

        /// <summary>
        /// 将获得管理员的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string skey = context.Request["skey"];
            string deptId = context.Request["deptId"];
            string page = context.Request["page"]; // 当前页码
            string rowNum = context.Request["rows"]; // 每页显示行数

            string strWhere = "";
            if (!string.IsNullOrEmpty(skey))
            {
                strWhere = " and ( t1.username like '%" + skey + "%' or t1.name like '%" + skey + "%' or t1.mobile like '%" + skey + "%')";
            }
            if (!string.IsNullOrEmpty(deptId) && deptId != "undefined")
            {
                string[] deptIds = deptId.Split(',');
                string ids = "";
                for (var i = 0; i < deptIds.Length; i++)
                {
                    ids += ",'" + deptIds[i] + "'";
                }
                strWhere += " and ( t1.DEPTID in( " + ids.Substring(1) + "))";
            }
            Model.Admin userInfo = new ManagePage().GetAdminInfo();
            int records;
            string filedOrder = " t1.id asc ";
            AdminController controller = new AdminController();
            DataSet data = controller.GetList(int.Parse(rowNum), int.Parse(page), strWhere, filedOrder, out records);
            if (data != null)
            {

                int total = Utils.GetPageCount(int.Parse(rowNum), records);
                return JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
            }
            else
            {
                return JsonHelp.ErrorJson("请求失败，稍后再试。");
            }

        }

        /// <summary>
        /// 退出登陆执行的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string LoginOut(HttpContext context)
        {
            context.Session[ManagePage.SESSION_ADMIN_INFO] = null;
            // 写入Cookie
            Utils.WriteCookie(ManagePage.COOKIE_USERNAME, "");
            Utils.WriteCookie(ManagePage.COOKIE_PASSWORD, "");
            return JsonHelp.SuccessJson("退出登陆成功");
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Add(HttpContext context)
        {
            string module = "authority";
            bool right = new ManagePage().VerifyRight(module, "Add");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string errMsg = "";
            Model.Admin model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            AdminController controller = new AdminController();
            Model.Admin userInfo = new ManagePage().GetAdminInfo();
            model.CREATORTIME = DateTime.Now;
            model.creatorUserId = admins.id;
            bool result = controller.Add(model, userInfo.id);
            if (result)
            {
                new ManagePage().AddAdminLog("新增用户 用户名:" + model.username, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson("新增用户成功");
            }
            else
            {
                return JsonHelp.ErrorJson("新增用户失败");
            }
        }

        /// <summary>
        /// 根据用户名查询用户是否注册
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string CheckUserName(HttpContext context)
        {
            string userName = context.Request["userName"];
            string id = context.Request["id"];

            if (string.IsNullOrEmpty(userName))
            {
                return JsonHelp.ErrorJson("参数userName错误");
            }
            AdminController controller = new AdminController();
            string strWhere = " and (t1.username='" + userName + "')";
            if (!string.IsNullOrEmpty(id))
            {
                strWhere += " and (t1.id <> '" + id + "') ";
            }
            Model.Admin admin = controller.GetModel(strWhere);
            if (admin != null)
            {
                return JsonHelp.SuccessJson(true);
            }
            else
            {
                return JsonHelp.SuccessJson(false);
            }
        }

        /// <summary>
        /// 根据手机号码检查手机号码是否注册
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string CheckMobile(HttpContext context)
        {
            string mobile = context.Request["mobile"];
            string id = context.Request["id"];

            if (string.IsNullOrEmpty(mobile))
            {
                return JsonHelp.ErrorJson("mobile错误");
            }
            AdminController controller = new AdminController();

            string strWhere = " and (t1.mobile='" + mobile + "')";
            if (!string.IsNullOrEmpty(id))
            {
                strWhere += " and (t1.id <> '" + id + "') ";
            }
            Model.Admin admin = controller.GetModel(strWhere);
            if (admin != null)
            {
                return JsonHelp.SuccessJson(true);
            }
            else
            {
                return JsonHelp.SuccessJson(false);
            }

        }
        /// <summary>
        /// 上传管理员头像
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string UploadAvatar(HttpContext context)
        {
            HttpPostedFile postedFile = context.Request.Files[0];
            // 检验文件类型
            string fileName = postedFile.FileName;
            string fileExt = Utils.GetFileExt(fileName); //文件扩展名，不含“.”
            Regex regex = new Regex(@"jpg|png");
            bool isMatch = regex.IsMatch(fileExt.ToLower());
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }

            object obj = context.Session[ManagePage.SESSION_ADMIN_INFO];
            string uid = "";
            if (obj == null)
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            else
            {
                Model.Admin saveUser = (Model.Admin)obj;
                uid = saveUser.id;
            }

            // 检验人员信息
            string filePath = "";
            bool result = UploadHelper.AvatarSaveAs(postedFile, ref filePath);

            if (result)
            {
                new AdminController().UpdateUserAvatar(uid, filePath);

                return JsonHelp.SuccessJson(filePath + "?" + DateTime.Now.ToString("HHmmssfff"));
            }
            else
            {
                return JsonHelp.ErrorJson("修改用户头像失败");
            }

        }
        /// <summary>
        /// 启用和停用 执行的方法
        /// </summary>
        /// <returns></returns>
        public string Disable(HttpContext context)
        {
            string module = "authority";
            bool right = new ManagePage().VerifyRight(module, "Enabled");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string id = context.Request["id"];
            string status = context.Request["status"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            if (string.IsNullOrEmpty(status))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            AdminController controller = new AdminController();
            bool result = controller.Disable(id, int.Parse(status));
            Model.Admin model = controller.GetUserModel(id);
            if (result)
            {
                // 添加操作日志
                if (status.Equals("0"))
                {
                    new ManagePage().AddAdminLog("禁用用户 用户名：" + model.username, Constant.ActionEnum.Edit);
                }
                else
                {
                    new ManagePage().AddAdminLog("启用用户 用户名：" + model.username, Constant.ActionEnum.Edit);
                }

                return JsonHelp.SuccessJson("用户状态修改成功");
            }
            else
            {
                return JsonHelp.ErrorJson("用户状态修改失败");
            }
        }

        /// <summary>
        /// 重置用户密码(将用户密码重置为123456)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string ResetUserPassword(HttpContext context)
        {
            string userId = context.Request["userId"];

            if (string.IsNullOrEmpty(userId))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            AdminController controller = new AdminController();
            // 获得用户信息
            Model.Admin userInfo = controller.GetModel(" and t1.id = '" + userId + "'");
            if (userInfo == null)
            {
                return JsonHelp.ErrorJson("不存在该用户");
            }
            // admin类赋值操作
            Model.Admin model = new Model.Admin();
            model.userpwd = Utils.MD5("123456").ToLower();
            model.id = userId;
            model.modifyTime = DateTime.Now;
            // 更新操作
            bool result = controller.UpdateUserPassword(model);
            if (result)
            {
                // 添加日志信息
                new ManagePage().AddAdminLog("重置密码 用户名：" + userInfo.username + "/" + userInfo.name, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("重置密码成功");
            }
            else
            {
                return JsonHelp.ErrorJson("重置密码失败");
            }
        }
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string UpdateUserPassword(HttpContext context)
        {

            object obj = context.Session[ManagePage.SESSION_ADMIN_INFO];
            string uid = "";
            if (obj == null)
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            else
            {
                Model.Admin saveUser = (Model.Admin)obj;
                uid = saveUser.id;
            }

            string oldPassword = context.Request["oldPassword"];
            string password = context.Request["password"];
            string txtCode = context.Request["code"];

            if (string.IsNullOrEmpty(oldPassword))
            {
                return JsonHelp.ErrorJson("旧密码不能为空");
            }

            if (string.IsNullOrEmpty(password))
            {
                return JsonHelp.ErrorJson("新密码不能为空");
            }
            if (string.IsNullOrEmpty(txtCode))
            {
                return JsonHelp.ErrorJson("验证码不能为空");
            }
            string code = Utils.MD5(context.Session[ManagePage.SESSION_CODE].ToString().ToLower());//图片验证码

            //统一转为小写比较
            if (!txtCode.ToLower().Equals(code))
            {
                return JsonHelp.ErrorJson("验证码不正确");
            }
            AdminController controller = new AdminController();
            // 获得用户信息
            Model.Admin userInfo = controller.GetModel(" and t1.id = " + uid);
            if (!userInfo.userpwd.ToLower().Equals(oldPassword.ToLower()))
            {
                return JsonHelp.ErrorJson("旧密码不正确");
            }
            // admin类赋值操作
            Model.Admin model = new Model.Admin();
            model.userpwd = password.ToLower();
            model.id = uid;
            model.modifyTime = DateTime.Now;
            // 更新操作
            bool result = controller.UpdateUserPassword(model);
            if (result)
            {
                // 添加日志信息
                new ManagePage().AddAdminLog("修改密码 用户名：" + userInfo.username, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("修改密码成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改密码失败");
            }
        }

        /// <summary>
        /// 删除一条辅助类别数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string Delete(HttpContext context)
        {
            string module = "authority";
            bool right = new ManagePage().VerifyRight(module, "Delete");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string strId = context.Request["id"];
            if (string.IsNullOrEmpty(strId))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            AdminController controller = new AdminController();
            // 判断是否存在该记录
            Model.Admin model = controller.GetUserModel(strId);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 该类别是否是系统角色
            if (model.roleId == "0")
            {
                return JsonHelp.ErrorJson("系统用户不允许删除");
            }
            model.deleteUserId = admins.id;
            // 执行删除操作，并添加日志文件
            if (controller.Delete(model))
            {
                new ManagePage().AddAdminLog("删除用户， Id:" + strId + ",用户名:" + model.username, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除用户成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除用户失败");
            }

        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string UpdateLoginUser(HttpContext context)
        {
            string errMsg = "";
            Model.Admin model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            model.id = admins.id;
            model.modifyUserId = admins.id;
            AdminController controller = new AdminController();
            // 执行添加操作
            bool result = controller.UpdateLoginUser(model);
            if (result)
            {
                new ManagePage().AddAdminLog("修改个人信息 用户名:" + model.username, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson("修改个人信息成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改个人信息失败");
            }
        }
        /// <summary>
        /// 将请求参数转为管理员类
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public Model.Admin GetRequestModel(HttpContext context, ref string errMsg)
        {
            string realName = Utils.UrlDecode(context.Request["Name"]);
            string userName = Utils.UrlDecode(context.Request["UserName"]);
            string userMobile = Utils.UrlDecode(context.Request["Mobile"]);
            string HeadIcon = Utils.UrlDecode(context.Request["HeadIcon"]);
            string deptId = context.Request["DeptId"];
            string roleId = context.Request["RoleId"];
            string description = context.Request["Description"];
            string gender = context.Request["Gender"];
            string nation = context.Request["Nation"];
            string nativePlace = context.Request["NativePlace"];
            string entryDate = context.Request["EntryDate"];
            string certificatesType = context.Request["CertificatesType"];
            string certificatesNumber = context.Request["CertificatesNumber"];
            string education = context.Request["Education"];
            string birthday = context.Request["Birthday"];
            string telePhone = context.Request["TelePhone"];
            string landLine = context.Request["LandLine"];
            string email = context.Request["Email"];
            string postalAddress = context.Request["PostalAddress"];
            string password = "123456";//Utils.UrlDecode(context.Request["password"]);
            string id = context.Request["id"];
            //string action = context.Request["action"];
            //string ret_password = context.Request["ret_password"];
            string isAdmin = context.Request["isAdmin"];
            // 判断参数是否为空
            if (string.IsNullOrEmpty(realName))
            {
                errMsg = "账户不能为空";
                return null;
            }

            if (realName.Length > 10)
            {
                errMsg = "账户最多10字符";
                return null;
            }
            // 判断参数是否为空
            if (string.IsNullOrEmpty(userName))
            {
                errMsg = "用户名不能为空";
                return null;
            }

            if (userName.Length > 20 || userName.Length < 4)
            {
                errMsg = "用户名长度为4-20字符";
                return null;
            }

            string regex = @"^[a-zA-Z0-9]{4,20}$";

            if (!Regex.IsMatch(userName, regex))
            {
                errMsg = "用户名由字母数字组成";
                return null;
            }
            // 判断参数是否为空
            if (string.IsNullOrEmpty(userMobile))
            {
                errMsg = "手机号不能为空";
                return null;
            }
            if (!Utils.IsMobile(userMobile))
            {
                errMsg = "手机号不符合规范";
                return null;
            }
            // 判断参数是否为空
            if (string.IsNullOrEmpty(roleId))
            {
                errMsg = "用户角色不能为空";
                return null;
            }

            AdminController controller = new AdminController();

            string whereUserName = "";
            string whereMobile = "";

            if (string.IsNullOrEmpty(id))
            {
                whereUserName = " and (username='" + userName + "')";
                whereMobile = " and ( mobile= '" + userMobile + "')";
            }
            else
            {
                whereUserName = " and ( t1.username='" + userName + "' and t1.id <> '" + id + "')";
                whereMobile = " and ( t1.mobile= '" + userMobile + "'  and t1.id <> '" + id + "')";
            }

            Model.Admin checkName = controller.GetModel(whereUserName);
            if (checkName != null)
            {
                errMsg = "用户名已经存在";
                return null;
            }
            Model.Admin checkMobile = controller.GetModel(whereMobile);
            if (checkMobile != null)
            {
                errMsg = "手机号码已被使用";
                return null;
            }
            // admin类赋值操作
            Model.Admin model = new Model.Admin();
            model.name = realName;
            model.username = userName;
            model.mobile = userMobile;
            model.avatar = HeadIcon;
            model.roleId = roleId;
            model.deptId = deptId;
            model.description = description;
            model.gender = int.Parse(gender);
            model.nation = int.Parse(nation == null ? "0" : nation);
            model.nativeplace = nativePlace;
            model.entryDate = entryDate;
            model.certificatesType = certificatesType;
            model.certificatesNumber = certificatesNumber;
            model.education = int.Parse(education == null ? "0" : education);
            model.birthday = birthday;
            model.telephone = telePhone;
            model.landLine = landLine;
            model.email = email;
            model.postalAddress = postalAddress;
            model.userpwd = Utils.MD5(password).ToLower();
            model.rightIds = "";
            model.modifyTime = DateTime.Now;
            //model.isAdmin = int.Parse(isAdmin);
            return model;
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