using Common;
using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// 上传文件的接口 的摘要说明
    /// </summary>
    public class upload : IHttpHandler, IRequiresSessionState
    {
        private static string petitionModule = "petitionQuery";
        private static string verifyMsg = "您没有上传文件的权限";
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
                    // 上传信访案件
                    case "uploadPetition":
                        context.Response.Write(UploadPetition(context));
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
        /// 上传培训文档方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string UploadPetition(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(petitionModule, Constant.ActionEnum.Upload);
            if (!right)
            {
                return JsonHelp.ErrorJson(verifyMsg);
            }
            HttpPostedFile postedFile = context.Request.Files[0];
            string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
           
            // 描述：上传文档信息增加视频和图片类型
            Regex regex = new Regex(@"pdf|doc|docx|xlsx|xls|gif|jpg|jpeg|png|bmp|mp4|avi|rmvb|ts");
            bool isMatch = regex.IsMatch(fileExt);
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }
            string uploadPath = "/data/upfile/petitionDoc/";
            PetitionFiles model = UploadHelper.SaveFileDoc(postedFile,uploadPath);
            // 文档类型
            model.type = fileExt;
            if (model != null)
            {
                PetitionFileController controller = new PetitionFileController();
                string result = controller.Add(model);
                if (!string.IsNullOrEmpty(result))
                {
                    model.id = result;
                    return JsonHelp.SuccessJson(model);
                }
                else
                {
                    return JsonHelp.ErrorJson("上传失败，稍后重试");
                }
            }
            else
            {
                return JsonHelp.ErrorJson("上传失败，稍后重试");
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