using Common;
using Controller;
using Cloud.ajax;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// 日志信息的摘要说明
    /// </summary>
    public class logs : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "operationLog";
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
                    // 分页显示日志的所有信息
                    case "list":
                        context.Response.Write(GetListJson(context));
                        break;
                    // 分页显示日志的所有信息
                    case "userLogs":
                        context.Response.Write(GetListJson(context, true));
                        break;
                    // 删除7天前的日志信息
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 查询所有的操作类型
                    case "types":
                        context.Response.Write(GetActions(context));
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
        /// <remarks>edit 20190428 liuyan</remarks>
        public string GetListJson(HttpContext context, bool userLogs = false)
        {
            string page = context.Request["page"]; // 当前页码
            string rowNum = context.Request["rows"]; // 每页显示行数
            string strWhere = GetListWhere(context, userLogs);
            LogsController controller = new LogsController();
            int records;
            string order = " t1.modifyTime desc";
            DataSet data = controller.GetList(int.Parse(rowNum), int.Parse(page), strWhere, order, out records);
            int total = Utils.GetPageCount(int.Parse(rowNum), records);
            return JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
        }

        /// <summary>
        /// 获得查询条件
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>查询条件</returns>
        /// <remarks>edit 20190428 liuyan</remarks>
        public string GetListWhere(HttpContext context, bool userLogs = false)
        {
            string fromDate = context.Request["fromDate"]; // 开始时间
            string toDate = context.Request["toDate"]; // 结束时间
            string user = context.Request["user"]; // 用户名
            string type = context.Request["type"]; // 操作类型
            string keyword = context.Request["keyword"]; // 关键字

            string strWhere = "";
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                strWhere += " t1.modifyTime between '" + fromDate + " 00:00:00'" + " and '" +toDate + " 23:59:59'";
            }
            else if (!string.IsNullOrEmpty(fromDate))
            {
                strWhere += " t1.modifyTime >= '" + fromDate + " 00:00:00'";
            }
            else if (!string.IsNullOrEmpty(toDate))
            {
                strWhere += " t1.modifyTime <= '" + toDate + " 23:59:59'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {

                if (string.IsNullOrEmpty(strWhere))
                {
                    strWhere += " t1.detail like '%" + keyword + "%' ";
                }
                else
                {
                    strWhere += " and t1.detail like '%" + keyword + "%' ";
                }

                
            }
            if (!string.IsNullOrEmpty(user))
            {
                if (string.IsNullOrEmpty(strWhere))
                {
                    strWhere += " t1.userId= '" + user + "'";
                }
                else
                {
                    strWhere += " and t1.userId= '" + user + "'";
                }
            }

            if (userLogs)
            {
                if (string.IsNullOrEmpty(strWhere))
                {
                    strWhere += " t1.userId= '" + new ManagePage().GetUserID() + "'";
                }
                else
                {
                    strWhere += " and t1.userId= '" + new ManagePage().GetUserID() + "'";
                }
            }
            if (!string.IsNullOrEmpty(type))
            {
                if (string.IsNullOrEmpty(strWhere))
                {
                    strWhere += " t1.typeNum= '" + type + "'";
                }
                else
                {
                    strWhere += " and t1.typeNum= '" + type + "'";
                }
            }
            return strWhere;
        }

        /// <summary>
        /// 删除7天前的日志信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>edit 20190428 liuyan</remarks>
        public string Delete(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Delete");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            LogsController controller = new LogsController();
            int ret = controller.Delete();
            if (ret > 0)
            {
                string msg = "成功删除" + ret + "条日志信息";
                new ManagePage().AddAdminLog(msg, Constant.ActionEnum.Delete);
                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = msg;
                return result.ToString();
            }
            else
            {
                JObject result = new JObject();
                result["status"] = -1;
                result["msg"] = "没有可以删除的日志信息";
                return result.ToString();
            }
        }

        /// <summary>
        /// 获得所有的操作类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>edit 20190428 liuyan</remarks>
        public string GetActions(HttpContext context)
        {
            JArray items = new JArray();
            JObject obj = new JObject();
            foreach (Constant.ActionEnum action in Enum.GetValues(typeof(Constant.ActionEnum)))
            {
                obj = new JObject();
                obj["id"] = (int)action;
                obj["name"] = action.ToString();
                items.Add(obj);
            }
            // data
            JObject data = new JObject();
            data["items"] = items;
            data["records"] = items.Count;
            // result
            JObject result = new JObject();
            result["status"] = 200;
            result["data"] = data;
            result["msg"] = "success";
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