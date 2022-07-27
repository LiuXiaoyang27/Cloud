using Common;
using Controller;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace Cloud.ajax
{
    /// <summary>
    /// Summary description for petitionRules
    /// </summary>
    public class oldPetition : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string verifyMsg = "您没有上传文件的权限";
        private static string module = "oldPetition";
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
                    // 显示案件列表
                    case "list":
                        context.Response.Write(GetListJson(context));
                        break;
                    // 初始化修改数据
                    case "query":
                        context.Response.Write(Query(context));
                        break;
                    // 编辑
                    case "update":
                        context.Response.Write(Update(context));
                        break;
                    // 添加案件信息
                    case "upload":
                        context.Response.Write(UploadFile(context));
                        break;
                    // 批量删除
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

        #region GetListJson:将获得的结果转换为json数据
        /// <summary>
        /// 将获得的结果转换为json数据
        /// </summary>
        /// <returns></returns>
        public string GetListJson(HttpContext context)
        {
            string page = context.Request["page"];
            string rows = context.Request["rows"];
            string matchCon = context.Request["matchCon"];
            string beginDate = context.Request["beginDate"];
            string endDate = context.Request["endDate"];

            string strWhere = "";
            if (!string.IsNullOrEmpty(matchCon))
            {
                strWhere += "and FILENAME like '%" + matchCon + "%' OR TITLE like '%" + matchCon + "%' OR PNAME like '%" + matchCon + "%' OR ATTENDNAME like '%" + matchCon + "%' ";
            }
            strWhere += !string.IsNullOrEmpty(beginDate) ? " and PDATE >= '" + beginDate + " 00:00:00" + "'" : "";
            strWhere += !string.IsNullOrEmpty(endDate) ? " and PDATE <= '" + endDate + " 23:59:59 " + "'" : "";

            OldPetitionController controller = new OldPetitionController();
            int records;
            DataSet data = controller.GetList(int.Parse(rows), int.Parse(page), strWhere, " MODIFYTIME DESC ", out records);
            int total = Utils.GetPageCount(int.Parse(rows), records);
            string json = JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
            return json;
        }
        #endregion

        #region Query:根据id 查询信息
        /// <summary>
        ///  根据id 查询信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Query(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数Id错误");
            }
            OldPetitionController controller = new OldPetitionController();
            OldPetition model = controller.GetModel(id);
            if (model != null)
            {
                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = "success";
                JObject obj = controller.GetJsonObj(model);
                result["data"] = obj;
                return result.ToString();
            }

            return JsonHelp.ErrorJson("该数据不存在,刷新后重试");

        }
        #endregion

        #region Update:修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Update(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Edit");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string id = context.Request["id"];

            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("id不能为空");
            }
            OldPetitionController controller = new OldPetitionController();
            OldPetition temp = controller.GetModel(id);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("不存在该案件信息");
            }
            string errMsg = "";
            OldPetition model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            model.id = id;

            if (controller.Update(model))
            {

                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = "success";
                new ManagePage().AddAdminLog("编辑陈年旧案 名称：" + model.fileName, Constant.ActionEnum.Add);
                return result.ToString();
            }
            else
            {
                return JsonHelp.ErrorJson("更新失败");
            }
        }
        #endregion

        #region GetRequestModel:获得表单信息
        /// <summary>
        /// 获得表单信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public OldPetition GetRequestModel(HttpContext context, ref string errMsg)
        {
            string fileName = context.Request["fileName"];
            string title = context.Request["title"];
            string pName = context.Request["pName"];
            string pDate = context.Request["pDate"];
            string attendName = context.Request["attendName"];

            OldPetition model = new OldPetition();
            if (!string.IsNullOrEmpty(pDate))
            {
                if (!Utils.IsDateTime(pDate))
                {
                    errMsg = "时间格式不正确";
                    return null;
                }
            }

            model.fileName = fileName;
            model.title = title;
            model.pName = pName;
            model.pDate = pDate;
            model.attendName = attendName;
            return model;
        }
        #endregion

        #region Delete:删除数据
        /// <summary>
        /// 批量删除数据
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

            OldPetitionController controller = new OldPetitionController();

            string[] ids = strId.Split(',');
            string delIds = "";
            for (int i = 0; i < ids.Length; i++)
            {
                delIds += ",'" + ids[i] + "'";
            }
            delIds = delIds.Substring(1);

            bool result = controller.Delete(delIds);
            if (result)
            {
                new ManagePage().AddAdminLog("删除信访条例 id:" + delIds, Constant.ActionEnum.Delete);
                return JsonHelp.SuccessJson("删除成功");
            }
            else
            {
                return JsonHelp.ErrorJson("删除失败");
            }

        }
        #endregion

        #region UploadFile:上传文件方法
        /// <summary>
        /// 上传文件的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string UploadFile(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, Constant.ActionEnum.Upload);
            if (!right)
            {
                return JsonHelp.ErrorJson(verifyMsg);
            }
            OldPetitionController controller = new OldPetitionController();
            string uploadPath = "/data/upfile/oldPetition/";
            HttpPostedFile postedFile = context.Request.Files[0];

            string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
            Regex regex = new Regex(@"doc|docx");
            bool isMatch = regex.IsMatch(fileExt);
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }
            OldPetition model = UploadHelper.SaveFile(postedFile, uploadPath);
            // 文档类型
            model.fileType = fileExt;
            model.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (model != null)
            {
                //判断文件是否存在
                string strWhere = " and FilePath = '" + model.filePath + "'";

                bool hasNum = controller.Exists(strWhere);
                if (hasNum)
                {
                    return JsonHelp.ErrorJson("文件已存在！");
                }
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
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}