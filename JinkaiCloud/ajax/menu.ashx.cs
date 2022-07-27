using Common;
using Controller;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    ///  菜单管理接口
    /// </summary>
    public class menu : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "menu";
        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            if (!new ManagePage().IsAdminLogin())
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }

            string action = context.Request["action"];
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            try
            {
                switch (action)
                {
                    // 根据条件查询所有的类别信息
                    case "list":
                        context.Response.Write(GetListJson(context));
                        break;
                    // 根据条件查询所有的类别信息
                    case "tree":
                        context.Response.Write(GetTreeJson(context));
                        break;
                    // 新增类别
                    case "add":
                        context.Response.Write(Add(context));
                        break;
                    // 更新类别信息
                    case "update":
                        context.Response.Write(Update(context));
                        break;
                    // 设置类别是否可用状态
                    case "disable":
                        context.Response.Write(UpdateStatus(context));
                        break;
                    // 检查调用别名是否存在
                    case "checkModule":
                        context.Response.Write(CheckModule(context));
                        break;
                    // 删除一条类别数据
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 根据角色权限获得用户菜单
                    case "userMenu":
                        context.Response.Write(GetUserMenu());
                        break;
                    // 根据id查询菜单信息
                    case "query":
                        context.Response.Write(GetModelJson(context));
                        break;
                    // 获取树形结构数据
                    case "treeView":
                        context.Response.Write(GetTree(context));
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
        /// 根据ID查询菜单信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetModelJson(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("请求参数错误");
            }
            MenuController controller = new MenuController();
            Menu model = controller.GetModel(id);
            if (model == null)
            {
                return JsonHelp.ErrorJson("不存在该人员信息");
            }
            return SuccessJson(model);
        }
        /// <summary>
        /// 返回的JSON数据
        /// </summary>
        public string SuccessJson(Menu model)
        {
            JObject data = JObject.Parse(JsonConvert.SerializeObject(model));
            JObject result = new JObject();
            result["status"] = 200;
            result["msg"] = "success";
            result["data"] = data;
            return result.ToString();
        }
        /// <summary>
        /// 获取树形结构数据 --liu 20210429
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetTree(HttpContext context)
        {
            MenuController controller = new MenuController();
            List<Menu> list = controller.GetModelList();
            return JsonHelp.SuccessJson(controller.ListToTreeJson(list));
        }
        /// <summary>
        /// 将获得的结果转换为json数据 
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string status = context.Request["status"];
            string strWhere = "";
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "1")
                {
                    strWhere += " and status = 1";
                }
            }
            MenuController controller = new MenuController();
            List<Menu> list = controller.GetModelList(strWhere);
            return JsonHelp.SuccessJson(controller.ListToGridTreeJson(list));

        }

        /// <summary>
        /// 将获得的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson(HttpContext context)
        {
            string strWhere = " and cLevel < 3 ";
            MenuController controller = new MenuController();
            DataSet data = controller.GetList(strWhere);
            JArray items = controller.GetJsonList(data.Tables[0]);
            return JsonHelp.SuccessJson(items, 1, 1);
        }

        /// <summary>
        /// 新增一条类别数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Add(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Add");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            // 获得请求信息
            string errMsg = "";
            Model.Menu model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            // 插入操作
            Model.Menu result = new MenuController().Add(model);

            if (result == null)
            {
                return JsonHelp.ErrorJson("新增频道失败");
            }
            else
            {
                new ManagePage().AddAdminLog("新增频道:" + model.name, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson("新增频道成功");
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Update(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string strId = context.Request["id"];
            if (string.IsNullOrEmpty(strId))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            MenuController controller = new MenuController();
            // 获得请求信息
            string errMsg = "";
            Model.Menu model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }

            // 判断是否存在该记录
            Model.Menu temp = controller.GetModel(strId);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }
            model.id = strId;
            bool result = controller.Update(model);

            if (result)
            {
                new ManagePage().AddAdminLog("更新频道,频道名称：" + model.name, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("更新频道成功");
            }
            else
            {
                return JsonHelp.ErrorJson("更新频道失败，稍后再试！");
            }
        }

        /// <summary>
        /// 获得请求信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private Model.Menu GetRequestModel(HttpContext context, ref string errMsg)
        {
            string strId = context.Request["id"];
            string name = Utils.UrlDecode(context.Request["name"]);
            string type = context.Request["typeNumber"];
            string parentId = context.Request["parentId"];
            string module = Utils.UrlDecode(context.Request["module"]);
            string ordnum = context.Request["ordnum"];
            string fontIcon = Utils.UrlDecode(context.Request["fontIcon"]);
            string remark = Utils.UrlDecode(context.Request["remark"]);
            string linkUrl = Utils.UrlDecode(context.Request["linkUrl"]);
            string LinkTarget = Utils.UrlDecode(context.Request["LinkTarget"]);
            //新增导航类型
            string navType = Utils.UrlDecode(context.Request["navType"]);

            // 检查参数是否正确
            if (string.IsNullOrEmpty(name))
            {
                errMsg = "频道名称不能为空";
                return null;
            }
            if (name.Length > 10)
            {
                errMsg = "频道名称最多10字符";
                return null;
            }
            if (string.IsNullOrEmpty(type))
            {
                errMsg = "权限名称不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(parentId))
            {
                errMsg = "父导航不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(module))
            {
                errMsg = "别名不能为空";
                return null;
            }
            if (module.Length > 50)
            {
                errMsg = "别名最长为50字符";
                return null;
            }
            if (!Utils.IsValidName(module))
            {
                errMsg = "别名只能由数字、字母、-或_等字符组成";
                return null;
            }
            if (string.IsNullOrEmpty(ordnum))
            {
                errMsg = "序号不能为空";
                return null;
            }
            if (!Utils.isNumber(ordnum))
            {
                errMsg = "序号只能输入数字";
                return null;
            }
            MenuController controller = new MenuController();

            bool hasUsed = false;
            if (!string.IsNullOrEmpty(strId))
            {
                hasUsed = controller.Exists(" module='" + name + "' and id <> '" + strId + "' ");
            }
            else
            {
                hasUsed = controller.Exists(" module='" + name + "' ");
            }
            // 名称重复判断
            if (hasUsed)
            {
                errMsg = "该调用别名已经存在";
                return null;
            }

            Model.Menu model = new Model.Menu();
            model.name = name;
            model.typeNumber = type;
            model.parentId = parentId;
            model.module = module;
            model.ordnum = int.Parse(ordnum);
            model.fontIcon = fontIcon;
            model.remark = remark;
            model.linkUrl = linkUrl;
            model.LinkTarget = LinkTarget;
            model.navType = navType;
            return model;

        }

        /// <summary>
        /// 更新辅助类别状态使其不可用（status 字段）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string UpdateStatus(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Enabled");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string strId = context.Request["id"];
            string status = context.Request["disable"];

            if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(status))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            MenuController controller = new MenuController();
            //int intId = int.Parse(strId);
            int intStatus = int.Parse(status);

            // 判断是否存在该记录
            Model.Menu model = controller.GetModel(strId);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 执行更新操作，并添加日志文件
            if (controller.UpdateStatus(strId, intStatus))
            {
                new ManagePage().AddAdminLog("更新频道状态， Id:" + strId + "状态为：" + status, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("更新状态成功");
            }
            else
            {
                return JsonHelp.ErrorJson("更新状态失败");
            }
        }

        /// <summary>
        /// 检查调用别名是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CheckModule(HttpContext context)
        {
            string id = context.Request["id"];
            string module = context.Request["module"];

            if (string.IsNullOrEmpty(module))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            MenuController controller = new MenuController();
            bool check = controller.CheckModule(module, id);
            return JsonHelp.SuccessJson(check);
        }

        /// <summary>
        /// 删除一条辅助类别数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete(HttpContext context)
        {
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
            MenuController controller = new MenuController();
            //int intId = int.Parse(strId);
            // 判断是否存在该记录
            Model.Menu model = controller.GetModel(strId);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 该类别是否有子类
            DataTable list = controller.GetList(" and parentID = '" + strId + "'").Tables[0];

            if (list.Rows.Count > 0) {
                return JsonHelp.ErrorJson("请先删除子频道");
            }
            //for (int i = 0; i < list.Rows.Count; i++)
            //{
            //    DataRow dataRow = list.Rows[i];
            //    string[] path = dataRow["cPath"].ToString().Split(',');
            //    if (path.Contains(strId))
            //    {
            //        return JsonHelp.ErrorJson("请先删除子频道");
            //    }
            //}

            // 执行删除操作，并添加日志文件
            if (controller.Delete(strId))
            {
                new ManagePage().AddAdminLog("删除频道， Id:" + strId + ",名称:" + model.name, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除频道成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除频道失败");
            }

        }

        /// <summary>
        /// 获得用户的菜单信息
        /// </summary>
        /// <returns></returns>
        private string GetUserMenu()
        {

            //获得当前登录管理员信息
            Model.Admin adminModel = new ManagePage().GetAdminInfo();
            if (adminModel == null)
            {
                return JsonHelp.ErrorJson("请先登录！");
            }

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
            if (listMenus == null)
            {
                return JsonHelp.ErrorJson("获取菜单信息失败！");
            }
            JObject result = new JObject();
            result["status"] = 200;
            result["msg"] = "success";
            JObject data = new JObject();
            data["items"] = controller.GetJsonList(listMenus.Tables[0]);
            result["data"] = data;
            return result.ToString();
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