using Common;
using Controller;
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
    public class role : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "role";


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
                    case "batch":
                        context.Response.Write(GetBatchJson(context));
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
                    case "checkName":
                        context.Response.Write(CheckName(context));
                        break;
                    // 查询所有的用户角色组
                    case "query":
                        context.Response.Write(RoleDetailJson(context));
                        break;
                    // 删除一条类别数据
                    case "delete":
                        context.Response.Write(Delete(context));
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
        /// 将获得的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string page = context.Request["page"]; // 当前页码
            string rowNum = context.Request["rows"]; // 每页显示行数
            string skey = context.Request["skey"];
            Model.Admin authUser = new ManagePage().GetAdminInfo();
            string strWhere = "";
            if (!string.IsNullOrEmpty(skey))
            {
                strWhere = " and ( NAME like '%" + skey + "%')";
            }
            RoleController controller = new RoleController();
            int records;
            DataSet data = controller.GetList(int.Parse(rowNum), int.Parse(page), strWhere, " id asc ", out records);
            int total = Common.Utils.GetPageCount(int.Parse(rowNum), records);
            return JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
        }

        /// <summary>
        /// 将获得的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetBatchJson(HttpContext context)
        {
            string strWhere = " and status = 1 ";
            RoleController controller = new RoleController();
            DataSet data = controller.GetList(strWhere);
            JArray items = controller.GetJsonList(data.Tables[0]);
            return JsonHelp.SuccessJson(items);
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
            string errMsg = "";
            Model.Role model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            RoleController controller = new RoleController();
            bool result = controller.Add(model);
            if (result)
            {
                new ManagePage().AddAdminLog("新增用户角色:" + model.name, Constant.ActionEnum.Add);
                return JsonHelp.SuccessJson("新增用户角色成功");
            }
            else
            {
                return JsonHelp.ErrorJson("添加用户角色失败！");
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

            // 检查参数是否正确
            if (string.IsNullOrEmpty(strId))
            {
                return JsonHelp.ErrorJson("角色编号不能为空！");
            }
            string errMsg = "";
            Model.Role model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            RoleController controller = new RoleController();

            // 判断是否存在该记录
            Model.Role tempModel = controller.GetModel(strId);
            if (tempModel == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 该类别是否是系统角色
            if (tempModel.isAdmin == 1)
            {
                model.type = 1;
                model.isAdmin = 1;
            }
            model.id = strId;
            bool result = controller.Update(model);

            if (result)
            {
                new ManagePage().AddAdminLog("修改用户角色:" + model.name, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("修改用户角色成功");
            }
            else
            {
                return JsonHelp.ErrorJson("修改用户角色失败！");
            }
        }

        /// <summary>
        ///  获得请求的数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Model.Role GetRequestModel(HttpContext context, ref string errMsg)
        {
            string name = Utils.UrlDecode(context.Request["name"]);
            string type = context.Request["type"];
            string menus = context.Request["menus"];
            string strId = context.Request["id"];

            // 检查参数是否正确
            if (string.IsNullOrEmpty(name))
            {
                errMsg = "角色名称不能为空！";
                return null;
            }

            if (name.Length > 20)
            {
                errMsg = "角色名称最多20字符！";
                return null;
            }

            if (string.IsNullOrEmpty(type))
            {
                errMsg = "角色类型不能为空！";
                return null;
            }

            int intType = int.Parse(type);

            if (!(intType == 1 || intType == 2))
            {
                errMsg = "角色类型错误！";
                return null;
            }

            RoleController controller = new RoleController();
            // 判断用户角色名称是否重复
            bool hasUsed = false;
            if (!string.IsNullOrEmpty(strId))
            {
                hasUsed = controller.CheckName(name, strId);
            }
            else
            {
                hasUsed = controller.CheckName(name);
            }
            if (hasUsed)
            {
                errMsg = "角色名称已经存在！";
                return null;
            }
            Model.Role model = new Model.Role();
            model.name = name;
            model.type = intType;
            if (intType == 1)
            {
                model.roleValues = null;
            }
            else
            {
                model.roleValues = GetRoleValues(menus);
            }
            return model;
        }

        /// <summary>
        /// 将json格式字符串转为RoleValue类
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        private List<Model.RoleValue> GetRoleValues(string menus)
        {
            List<Model.RoleValue> roles = new List<Model.RoleValue>();
            JArray jarray = JArray.Parse(menus);
            for (int i = 0; i < jarray.Count; i++)
            {
                JObject obj = (JObject)jarray[i];
                roles.Add(new Model.RoleValue
                {
                    menuId = obj["menuId"].ToString(),
                    typeNumber = obj["action"].ToString(),
                });
            }
            return roles;
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

            RoleController controller = new RoleController();
            int intStatus = int.Parse(status);

            // 判断是否存在该记录
            Model.Role model = controller.GetModel(strId);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 执行更新操作，并添加日志文件
            if (controller.UpdateStatus(strId, intStatus))
            {
                new ManagePage().AddAdminLog("更新用户角色状态， Id:" + strId + "状态为：" + status, Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("更新用户角色状态成功");
            }
            else
            {
                return JsonHelp.ErrorJson("更新用户角色状态失败");
            }
        }

        /// <summary>
        /// 检查调用别名是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CheckName(HttpContext context)
        {
            string id = context.Request["id"];
            string name = Utils.UrlDecode(context.Request["name"]);

            if (string.IsNullOrEmpty(name))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            RoleController controller = new RoleController();
            bool check = true;
            if (string.IsNullOrEmpty(id))
            {
                check = controller.CheckName(name);
            }
            else
            {
                check = controller.CheckName(name, id);
            }

            return JsonHelp.SuccessJson(check);
        }

        /// <summary>
        /// 获得角色信息详情
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string RoleDetailJson(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            RoleController controller = new RoleController();
            Model.Role model = controller.GetModel(id);
            JObject data = new JObject();
            data["name"] = model.name;
            data["id"] = model.id;
            data["type"] = model.type;
            data["isAdmin"] = model.isAdmin;
            JObject result = new JObject();
            List<Model.RoleValue> role_values = model.roleValues;
            JArray items = new JArray();
            if (role_values != null)
            {
                for (int i = 0; i < role_values.Count; i++)
                {
                    JObject item = new JObject();
                    string roleId = role_values[i].roleId;
                    string menuId = role_values[i].menuId;
                    string action = role_values[i].typeNumber;
                    item["roleId"] = roleId;
                    item["menuId"] = menuId;
                    item["action"] = action;
                    items.Add(item);
                }
                data["items"] = items;
            }
            result["data"] = data;
            result["status"] = 200;
            result["msg"] = "success";
            return result.ToString();
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
            RoleController controller = new RoleController();
            // 判断是否存在该记录
            Model.Role model = controller.GetModel(strId);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }

            // 该类别是否是系统角色
            if (model.isAdmin == 1)
            {
                return JsonHelp.ErrorJson("系统角色不允许删除");
            }

            // 判断角色是否被使用
            if (HasUsed(strId))
            {
                return JsonHelp.ErrorJson("角色已经被使用");
            }

            // 执行删除操作，并添加日志文件
            if (controller.Delete(strId))
            {
                new ManagePage().AddAdminLog("删除用户角色， Id:" + strId + ",名称:" + model.name, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除用户角色成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除用户角色失败");
            }

        }

        /// <summary>
        /// 检查角色是否已经被使用
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private bool HasUsed(string roleId)
        {
            return new AdminController().Exists(" roleId= '" + roleId + "'");
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