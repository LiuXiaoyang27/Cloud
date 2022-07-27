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

namespace Cloud.ajax
{
    /// <summary>
    /// 系统设置相关接口 的摘要说明
    /// </summary>
    public class options : IHttpHandler, IRequiresSessionState
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
                    case "clear":
                        context.Response.Write(ClearCache(context));
                        break;
                    // 更新参数
                    case "updateOptions":
                        context.Response.Write(UpdateOptions(context));
                        break;
                    case "getOptions":
                        context.Response.Write(GetOptions(context));
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
        /// <summary>
        /// 更新系统参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetOptions(HttpContext context)
        {
            string module = "parameter";
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }

            OptionsController controller = new OptionsController();
            JObject result = controller.GetOptions();
            if (result != null)
            {
                return JsonHelp.SuccessJson(result);
            }
            else {
                return JsonHelp.ErrorJson("获取系统参数失败");
            }
            
        }
        /// <summary>
        /// 更新系统参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string UpdateOptions(HttpContext context)
        {
            string module = "options";
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string root_key1 = Utils.UrlDecode(context.Request["root_key1"]);
            string root_key2 = Utils.UrlDecode(context.Request["root_key2"]);
            string root_key3 = Utils.UrlDecode(context.Request["root_key3"]);

            if (string.IsNullOrEmpty(root_key1))
            {
                return JsonHelp.ErrorJson("root_key1不能为空");
            }

            if (string.IsNullOrEmpty(root_key2))
            {
                return JsonHelp.ErrorJson("root_key2不能为空");
            }
            if (string.IsNullOrEmpty(root_key3))
            {
                return JsonHelp.ErrorJson("root_key3不能为空");
            }
           

            List<Model.Options> list = new List<Model.Options>();
            list.Add(new Model.Options() { op_name = "root_key1", op_value = root_key1 });
            list.Add(new Model.Options() { op_name = "root_key2", op_value = root_key2 });
            list.Add(new Model.Options() { op_name = "root_key3", op_value = root_key3 });

            OptionsController controller = new OptionsController();
            bool result = controller.UpdateOptions(list);
            if (result)
            {
                // 添加操作日志
                new ManagePage().AddAdminLog("更新系统参数成功", Constant.ActionEnum.Edit);
                return JsonHelp.SuccessJson("更新系统参数成功");
            }
            else
            {
                return JsonHelp.ErrorJson("更新系统参数失败");
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