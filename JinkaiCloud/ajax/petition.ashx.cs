using Common;
using Controller;
using Model;
using Newtonsoft.Json;
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
    /// 信访案件
    /// </summary>
    public class petition : IHttpHandler, IRequiresSessionState
    {
        // 频道别名
        private static string module = "petitionQuery";

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
                    // 添加案件信息
                    case "add":
                        context.Response.Write(Add(context));
                        break;
                    // 初始化修改数据
                    case "query":
                        context.Response.Write(Query(context));
                        break;
                    // 获得上传文件
                    case "getUpload":
                        context.Response.Write(GetUploadById(context));
                        break;
                    // 修改
                    case "update":
                        context.Response.Write(Update(context));
                        break;
                    // 删除
                    case "delete":
                        context.Response.Write(Delete(context));
                        break;
                    // 批量删除
                    case "batchDelete":
                        context.Response.Write(DeleteBatch(context));
                        break;
                    // 导出PDF
                    case "pdf":
                        context.Response.Write(ExportPdf(context));
                        break;
                    // 导出excel
                    case "export":
                        context.Response.Write(ExportExcel(context));
                        break;
                    // 点击导入Excel执行的方法
                    case "import":
                        context.Response.Write(ImportExcel(context));
                        break;
                    // 点击导入Word执行的方法
                    case "impWord":
                        context.Response.Write(ImportWord(context));
                        break;
                    // 修改状态
                    case "release":
                        context.Response.Write(Release(context));
                        break;
                    // 库存警告
                    case "warnlist":
                        context.Response.Write(GetPetitionWarn(context));
                        break;
                    // 过期信息
                    case "overdue":
                        context.Response.Write(GetPetitionOverdue(context));
                        break;
                    // 查询报表
                    case "charts":
                        context.Response.Write(GetPetitionCharts(context));
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
            string caseSource = context.Request["caseSource"];
            string caseType = context.Request["caseType"];
            string caseCategory = context.Request["caseCategory"];
            string channels = context.Request["channels"];

            string strWhere = "";
            if (!string.IsNullOrEmpty(matchCon))
            {
                strWhere += "and CASENAME like '%" + matchCon + "%' OR PNAME like '%" + matchCon + "%' OR RECEIVER like '%" + matchCon + "%' ";
            }
            strWhere += !string.IsNullOrEmpty(beginDate) ? " and CREATEDATE >= '" + beginDate + " 00:00:00" + "'" : "";
            strWhere += !string.IsNullOrEmpty(endDate) ? " and CREATEDATE <= '" + endDate + " 23:59:59 " + "'" : "";
            strWhere += !string.IsNullOrEmpty(caseSource) ? " and CASESOURCE = '" + caseSource + "' " : "";
            strWhere += !string.IsNullOrEmpty(caseType) ? " and CASETYPE = '" + caseType + "' " : "";
            strWhere += !string.IsNullOrEmpty(caseCategory) ? " and CASECATEGORY = '" + caseCategory + "' " : "";
            strWhere += !string.IsNullOrEmpty(channels) ? " and CHANNELS = '" + channels + "' " : "";

            PetitionController controller = new PetitionController();
            int records;
            DataSet data = controller.GetList(int.Parse(rows), int.Parse(page), strWhere, " CREATEDATE DESC ", out records);
            int total = Utils.GetPageCount(int.Parse(rows), records);
            string json = JsonHelp.SuccessJson(controller.GetJsonList(data.Tables[0]), total, int.Parse(page), records);
            return json;
        }
        #endregion

        /// <summary>
        /// 预警信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetPetitionWarn(HttpContext context)
        {
            PetitionController controller = new PetitionController();
            DataSet data = controller.GetWarnPetition("");
            JArray items = controller.GetWarnPetitionJson(data.Tables[0]);
            return JsonHelp.SuccessJson(items, 1, 1);
        }
        /// <summary>
        /// 过期信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetPetitionOverdue(HttpContext context)
        {
            string matchCon = context.Request["matchCon"];
            string strWhere = "";
            if (!string.IsNullOrEmpty(matchCon))
            {
                strWhere += " and CASENAME like '%" + matchCon + "%' OR PNAME like '%" + matchCon + "%' OR RECEIVER like '%" + matchCon + "%' ";
            }
            PetitionController controller = new PetitionController();
            DataSet data = controller.GetOverduePetition(strWhere);
            JArray items = controller.GetWarnPetitionJson(data.Tables[0]);
            return JsonHelp.SuccessJson(items, 1, 1);
        }
        #region Add:新增案件
        /// <summary>
        /// 新增信访案件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Add(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Add");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string errMsg = "";
            Model.Petition model = GetRequestModel(context, ref errMsg);

            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }
            string filePdfs = context.Request["filePdfs"];
            PetitionController controller = new PetitionController();
            model.status = 0;
            string result = controller.Add(model);
            if (!string.IsNullOrEmpty(result))
            {
                model.id = result;
                //更新文件信息
                List<PetitionFiles> listPdf = JsonConvert.DeserializeObject<List<PetitionFiles>>(filePdfs);
                PetitionFileController controller1 = new PetitionFileController();
                if (listPdf != null && listPdf.Count > 0)
                {
                    controller1.Update(model.id, listPdf);
                }
                new ManagePage().AddAdminLog("新增案件信息 案件名称：" + model.caseName, Constant.ActionEnum.Add);
                return SuccessJson(model);
            }
            else
            {
                return JsonHelp.ErrorJson("新增案件失败");
            }

        }
        #endregion

        #region 添加/修改之后，返回的JSON数据
        /// <summary>
        /// 添加/修改之后，返回的JSON数据
        /// </summary>
        public string SuccessJson(Model.Petition model)
        {
            JObject data = JObject.Parse(JsonConvert.SerializeObject(model));
            JObject result = new JObject();
            result["status"] = 200;
            result["msg"] = "success";
            result["data"] = data;
            return result.ToString();
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
            PetitionController controller = new PetitionController();
            Petition model = controller.GetModel(id);
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

        #region GetUploadById:获得上传文件
        /// <summary>
        /// 获得上传文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetUploadById(HttpContext context)
        {
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }
            else
            {
                PetitionFileController controller = new PetitionFileController();
                DataTable dataTable = controller.GetList("'" + id + "'").Tables[0];
                JArray listDocs = new JArray();
                DataRow dataRow;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataRow = dataTable.Rows[i];
                    string type = dataRow["typenum"].ToString();
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
        }
        #endregion

        #region GetUploadJson:将DataRow转为JObject类型
        /// <summary>
        /// 将DataRow转为JObject类型
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public JObject GetUploadJson(DataRow dataRow)
        {
            JObject obj = new JObject();
            obj["id"] = dataRow["id"].ToString();
            obj["name"] = dataRow["name"].ToString();
            obj["petitionId"] = dataRow["petitionId"].ToString();
            obj["type"] = dataRow["typenum"].ToString();
            obj["url"] = dataRow["url"].ToString();
            obj["size"] = int.Parse(dataRow["csize"].ToString());
            obj["deleteUrl"] = dataRow["deleteUrl"].ToString();
            obj["deleteType"] = dataRow["deleteType"].ToString();
            obj["isDelete"] = int.Parse(dataRow["isDelete"].ToString());
            return obj;
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
            PetitionController controller = new PetitionController();
            Model.Petition temp = controller.GetModel(id);
            if (temp == null)
            {
                return JsonHelp.ErrorJson("不存在该案件信息");
            }
            string errMsg = "";
            Petition model = GetRequestModel(context, ref errMsg);
            if (model == null)
            {
                return JsonHelp.ErrorJson(errMsg);
            }

            model.id = id;
            string filePdfs = context.Request["filePdfs"];
            model.status = temp.status;
            if (controller.Update(model))
            {
                // 更新文档信息
                List<PetitionFiles> listPdf = JsonConvert.DeserializeObject<List<PetitionFiles>>(filePdfs);
                if (listPdf != null && listPdf.Count > 0)
                {
                    PetitionFileController controller1 = new PetitionFileController();
                    controller1.Update(model.id, listPdf);
                }
                JObject result = new JObject();
                result["status"] = 200;
                result["msg"] = "success";
                JObject obj = controller.GetJsonObj(model); ;
                result["data"] = obj;
                new ManagePage().AddAdminLog("修改案件信息 案件名称：" + model.caseName, Constant.ActionEnum.Add);
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
        public Petition GetRequestModel(HttpContext context, ref string errMsg)
        {
            string createDate = context.Request["createDate"];
            string pName = context.Request["pName"];
            string pIdCard = context.Request["pIdCard"];
            string pAddress = context.Request["pAddress"];
            string caseType = context.Request["caseType"];
            string caseName = context.Request["caseName"];
            string caseCategory = context.Request["caseCategory"];
            string caseSource = context.Request["caseSource"];
            string channels = context.Request["channels"];
            string receiver = context.Request["receiver"];
            string rerm = context.Request["rerm"];
            string ext1 = context.Request["ext1"];
            string ext2 = context.Request["ext2"];
            string ext3 = context.Request["ext3"];
            string ext4 = context.Request["ext4"];
            string ext5 = context.Request["ext5"];
            Petition model = new Petition();
            if (string.IsNullOrEmpty(createDate))
            {
                errMsg = "时间不能为空";
                return null;
            }
            if (!Utils.IsDateTime(createDate))
            {
                errMsg = "时间格式不正确";
                return null;
            }
            if (string.IsNullOrEmpty(pName))
            {
                errMsg = "当事人姓名不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(pIdCard))
            {
                errMsg = "身份证号不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(caseType))
            {
                errMsg = "案件类型不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(caseName))
            {
                errMsg = "案件名称不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(caseCategory))
            {
                errMsg = "案件种类不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(caseSource))
            {
                errMsg = "案件来源不能为空";
                return null;
            }

            if (string.IsNullOrEmpty(channels))
            {
                errMsg = "来访渠道不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(receiver))
            {
                errMsg = "接待人不能为空";
                return null;
            }
            if (string.IsNullOrEmpty(rerm))
            {
                errMsg = "办理期限不能为空";
                return null;
            }

            model.createDate = createDate;
            model.pName = pName;
            model.pIdCard = pIdCard;
            model.pAddress = pAddress;
            model.caseType = caseType;
            model.caseName = caseName;
            model.caseCategory = caseCategory;
            model.caseSource = caseSource;
            model.channels = channels;
            model.receiver = receiver;
            model.rerm = int.Parse(rerm);
            model.ext1 = ext1;
            model.ext2 = ext2;
            model.ext3 = ext3;
            model.ext4 = ext4;
            model.ext5 = ext5;
            model.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return model;
        }
        #endregion

        #region Delete:删除一条数据
        /// <summary>
        /// 删除一条数据
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
            string id = context.Request["id"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            PetitionController controller = new PetitionController();
            Petition dataRow = controller.GetModel(id);
            if (dataRow != null)
            {
                bool result = controller.Delete("'" + id + "'");
                if (result)
                {
                    result = new PetitionFileController().Delete("'" + id + "'");
                    if (result)
                    {
                        new ManagePage().AddAdminLog("删除案件信息 案件名称:" + dataRow.caseName, Constant.ActionEnum.Delete);
                        return JsonHelp.SuccessJson("删除成功");
                    }
                    else
                    {
                        return JsonHelp.ErrorJson("删除信访案件文档失败");
                    }

                }
                else
                {
                    return JsonHelp.ErrorJson("删除失败");
                }
            }

            return JsonHelp.ErrorJson("该单据不存在,刷新后重试");
        }
        #endregion

        #region DeleteBatch:批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteBatch(HttpContext context)
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

            PetitionController controller = new PetitionController();

            string[] ids = strId.Split(',');
            //string delIds = "";
            //for (int i = 0; i < ids.Length; i++)
            //{
            //    delIds += ",'" + ids[i] + "'";
            //}
            //delIds = delIds.Substring(1);
            List<string> delIds = new List<string>(ids);

            bool result = controller.DeleteBatch(delIds);
            if (result)
            {
                result = new PetitionFileController().DeleteBatch(delIds);
                if (result)
                {
                    new ManagePage().AddAdminLog("删除案件信息 id:" + delIds, Constant.ActionEnum.Delete);
                    return JsonHelp.SuccessJson("删除成功");
                }
                else
                {
                    return JsonHelp.ErrorJson("删除信访案件文档失败");
                }
            }
            else
            {
                return JsonHelp.ErrorJson("删除失败");
            }

        }
        #endregion

        /// <summary>
        /// 根据请求条件查询数据 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public DataTable SearchData(HttpContext context)
        {
            string ids = context.Request["id"];
            string matchCon = context.Request["matchCon"];
            string beginDate = context.Request["beginDate"];
            string endDate = context.Request["endDate"];
            string caseSource = context.Request["caseSource"];
            string caseType = context.Request["caseType"];
            string caseCategory = context.Request["caseCategory"];
            string channels = context.Request["channels"];

            string strWhere = "";
            if (!string.IsNullOrEmpty(matchCon))
            {
                strWhere += "and CASENAME like '%" + matchCon + "%' OR PNAME like '%" + matchCon + "%' OR RECEIVER like '%" + matchCon + "%' ";
            }
            strWhere += !string.IsNullOrEmpty(beginDate) ? " and CREATEDATE >= '" + beginDate + " 00:00:00" + "'" : "";
            strWhere += !string.IsNullOrEmpty(endDate) ? " and CREATEDATE <= '" + endDate + " 23:59:59 " + "'" : "";
            strWhere += !string.IsNullOrEmpty(caseSource) ? " and CASESOURCE = '" + caseSource + "' " : "";
            strWhere += !string.IsNullOrEmpty(caseType) ? " and CASETYPE = '" + caseType + "' " : "";
            strWhere += !string.IsNullOrEmpty(caseCategory) ? " and CASECATEGORY = '" + caseCategory + "' " : "";
            strWhere += !string.IsNullOrEmpty(channels) ? " and CHANNELS = '" + channels + "' " : "";
            if (!string.IsNullOrEmpty(ids))
            {
                strWhere += " and id in ('" + ids + "')";
            }
            PetitionController controller = new PetitionController();
            DataTable dataTable = controller.GetList(strWhere).Tables[0];
            return dataTable;
        }


        #region Pdf:导出PDF
        /// <summary>
        /// 导出pdf
        /// </summary>
        /// <param name="context"></param>
        public string ExportPdf(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Export");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            PetitionController controller = new PetitionController();
            DataTable dataTable = SearchData(context);
            if (dataTable.Rows.Count == 0)
            {
                return JsonHelp.ErrorJson("请先选择导出数据");
            }
            //string implDate = dataTable.Rows[0]["implDate"].ToString();
            string fileName = controller.CreatePetitionPdf(dataTable, "");
            new ManagePage().AddAdminLog("导出信访案件(PDF)：" + fileName, Constant.ActionEnum.Export);
            return JsonHelp.SuccessJson(fileName);

        }
        #endregion

        #region export:导出Excel
        /// <summary>
        /// 导出excel表格 
        /// </summary>
        /// <param name="context"></param>
        public string ExportExcel(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, Constant.ActionEnum.Export);
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }

            PetitionController controller = new PetitionController();
            DataTable dataTable = SearchData(context);
            if (dataTable.Rows.Count == 0)
            {
                return JsonHelp.ErrorJson("请先选择导出数据");
            }
            List<Petition> list = controller.TableToList(dataTable);
            Admin userInfo = new ManagePage().GetAdminInfo();
            string userName = userInfo.name;

            string path = controller.ExportExcelPetition(userName, list);
            if (string.IsNullOrEmpty(path))
            {
                return JsonHelp.ErrorJson("导出失败");
            }
            else
            {
                new ManagePage().AddAdminLog("导出信访案件列表(Excel)", Constant.ActionEnum.Export);
                return JsonHelp.SuccessJson(path);
            }

        }
        #endregion

        #region ImportExcel:导入文件
        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string ImportExcel(HttpContext context)
        {

            bool right = new ManagePage().VerifyRight(module, "Import");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            HttpPostedFile postedFile = context.Request.Files[0];
            string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
            Regex regex = new Regex(@"xlsx");
            bool isMatch = regex.IsMatch(fileExt);
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }
            string filePath = UploadHelper.FileSaveAs(postedFile);
            PetitionController controller = new PetitionController();
            string errMsg = "";
            List<Petition> list = controller.GetPetitionExcel(filePath, ref errMsg);
            bool res = new PetitionController().AddList(list);
            if (res)
            {
                new ManagePage().AddAdminLog("导入信访案件：" + postedFile.FileName, Constant.ActionEnum.Import);
                return JsonHelp.SuccessJson(200, "成功导入" + list.Count + "条信息！", errMsg);
            }
            else
            {
                return JsonHelp.SuccessJson(-1, "导入失败", errMsg);
            }

        }
        #endregion

        #region ImportWord:导入Word文件
        /// <summary>
        /// 导入文件 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string ImportWord(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Import");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            HttpPostedFile postedFile = context.Request.Files[0];
            string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
            Regex regex = new Regex(@"docx");
            bool isMatch = regex.IsMatch(fileExt);
            if (!isMatch)
            {
                return JsonHelp.ErrorJson("文件类型不匹配");
            }
            string filePath = UploadHelper.FileSaveAs(postedFile);

            WordController controller = new WordController();
            string errMsg = "";

            List<Model.Petition> listModel = controller.GetPetitionWord(filePath, ref errMsg);
            if (listModel == null || listModel.Count < 1)
            {
                return JsonHelp.ErrorJson("请填写有效的数据");
            }
            bool result = new PetitionController().AddList(listModel);
            if (result)
            {
                new ManagePage().AddAdminLog("导入信访案件：" + postedFile.FileName, Constant.ActionEnum.Import);
                return JsonHelp.SuccessJson("导入成功");
            }
            else
            {
                return JsonHelp.ErrorJson("导入失败，请稍后再试。");
            }

        }
        #endregion

        #region Release:更改状态
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Release(HttpContext context)
        {
            bool right = new ManagePage().VerifyRight(module, "Enabled");
            if (!right)
            {
                return JsonHelp.ErrorJson("权限不足");
            }
            string id = context.Request["id"];
            string status = context.Request["status"];
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(status))
            {
                return JsonHelp.ErrorJson("参数错误");
            }

            PetitionController controller = new PetitionController();
            // 判断数据是否存在
            Model.Petition model = controller.GetModel(id);
            if (model == null)
            {
                return JsonHelp.ErrorJson("数据不存在，刷新重试");
            }


            if (controller.Release(id, int.Parse(status)))
            {
                string action = int.Parse(status) == 1 ? "已办理" : "未办理";
                new ManagePage().AddAdminLog("案件名称：" + model.caseName + " " + action, Constant.ActionEnum.Enabled);
                return JsonHelp.SuccessJson("状态修改成功");
            }
            else
            {
                return JsonHelp.ErrorJson("状态修改失败");
            }

        }
        #endregion

        #region charts:查询图表数据
        /// <summary>
        /// 查询图表数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetPetitionCharts(HttpContext context)
        {
            string beginDate = context.Request["beginDate"];
            string endDate = context.Request["endDate"];
            string strWhere = "";
            strWhere += !string.IsNullOrEmpty(beginDate) ? " and t1.CREATEDATE >= '" + beginDate + " 00:00:00" + "'" : "";
            strWhere += !string.IsNullOrEmpty(endDate) ? " and t1.CREATEDATE <= '" + endDate + " 23:59:59 " + "'" : "";
            PetitionController controller = new PetitionController();
            JObject obj = controller.GetPetitionCharts(strWhere);

            return JsonHelp.SuccessJson(obj);
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