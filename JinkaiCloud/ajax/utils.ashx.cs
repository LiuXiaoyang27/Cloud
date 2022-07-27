using Common;
using Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace JinkaiCloud.ajax
{
    /// <summary>
    /// 系统设置相关接口 的摘要说明
    /// </summary>
    public class utils : IHttpHandler, IRequiresSessionState
    {
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
                    // 清理缓存
                    case "clear":
                        context.Response.Write(ClearCache(context));
                        break;
                    // 首页统计信息
                    case "index":
                        context.Response.Write(IndexReport(context));
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
        /// 首页统计信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string IndexReport(HttpContext context)
        {
            UtilsController controller = new UtilsController();
            JObject obj = controller.GetIndexReport();
            // 添加操作日志
            return JsonHelp.SuccessJson(obj);
        }
        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string ClearCache(HttpContext context)
        {
            // 删除生成的下载文件
            Utils.DeleteDirectory("/data/download/");
            // 删除上传的excel文件
            Utils.DeleteDirectory("/data/upfile/excel/");

            // 添加操作日志
            new ManagePage().AddAdminLog("清理上传缓存文件！", Constant.ActionEnum.Edit);
            return JsonHelp.SuccessJson("清理成功");
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