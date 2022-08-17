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
                    // 添加案件信息
                    case "upload":
                        context.Response.Write(UploadFile(context));
                        break; 
                    // 添加案件信息
                    case "getUpload":
                        context.Response.Write(GetUpload());
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                context.Response.Write(JsonHelp.ErrorJson(msg));
            }
        } 

        #region GetUpload 获取所有文件信息 
        public string GetUpload()
        {
            OldPetitionController controller = new OldPetitionController();
            DataTable dataTable = controller.GetList().Tables[0];
            JArray listDocs = new JArray();
            DataRow dataRow;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataRow = dataTable.Rows[i];

                listDocs.Add(GetUploadJson(dataRow));

            }
            JObject data = new JObject();
            data["listDocs"] = listDocs;
            JObject result = new JObject();
            result["status"] = 200;
            result["msg"] = "success";
            result["data"] = data;
            return result.ToString();
        }
        #endregion

        /// <summary>
        /// 将DataRow转为JObject类型
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public JObject GetUploadJson(DataRow dataRow)
        {
            JObject obj = new JObject();
            obj["id"] = dataRow["ID"].ToString();
            obj["name"] = dataRow["FILENAME"].ToString();
            
            obj["type"] = dataRow["FILETYPE"].ToString();
            obj["url"] = dataRow["FILEPATH"].ToString();
            obj["size"] = int.Parse(dataRow["FILESIZE"].ToString());
            return obj;
        }


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
            OldPetitionController oController = new OldPetitionController();
            //string uploadPath = "/data/upfile/oldPetition/";
            string uploadPath = "/data/upfile/petitionDoc/";
            HttpPostedFile postedFile = context.Request.Files[0];

            string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
            Regex regex = new Regex(@"doc|docx");
            bool isMatch = regex.IsMatch(fileExt);
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }
            //OldPetition oldModel = UploadHelper.SaveFile(postedFile, uploadPath);
            PetitionFiles fileModel = UploadHelper.SaveFileDoc(postedFile, uploadPath);
            OldPetition oldModel = new OldPetition();
            oldModel.fileName = fileModel.name;
            oldModel.fileSize = fileModel.size;
            oldModel.filePath = fileModel.url;

            // 文档类型
            oldModel.fileType = fileExt;
            fileModel.type = fileExt;
            oldModel.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (fileModel != null)
            {
                ////判断文件是否存在
                //string strWhere = " and FilePath = '" + model.filePath + "'";

                //bool hasNum = controller.Exists(strWhere);
                //if (hasNum)
                //{
                //    return JsonHelp.ErrorJson("文件已存在！");
                //}

                //将文件插入到案件表中
                Petition pModel = new Petition();
                pModel.caseName = fileModel.name;
                pModel.status = 1;
                pModel.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                PetitionController pController = new PetitionController();
                string result = pController.Add(pModel);
                if (!string.IsNullOrEmpty(result))
                {
                    pModel.id = result;
                    //新增陈年旧案表信息
                    string oldRes = oController.Add(oldModel);
                    oldModel.id = oldRes;
                    //新增文件
                    PetitionFileController fileController = new PetitionFileController();
                    string fileRes = fileController.Add(fileModel);
                    fileModel.id = fileRes;

                    //更新文件信息
                    List<PetitionFiles> listPdf = new List<PetitionFiles>();
                    listPdf.Add(fileModel);

                    if (listPdf != null && listPdf.Count > 0)
                    {
                        fileController.Update(pModel.id, listPdf);
                    }
                    new ManagePage().AddAdminLog("上传案件 文件名称：" + pModel.caseName, Constant.ActionEnum.Add);
                    return JsonHelp.SuccessJson(oldModel);
                }
                else
                {
                    return JsonHelp.ErrorJson("上传案件失败");
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