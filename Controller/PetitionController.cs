using Business;
using Common;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Controller
{
    /// <summary>
    /// 通知公告操作类
    /// </summary>
    public class PetitionController
    {
        private readonly PetitionBll dal;

        public PetitionController()
        {
            dal = new PetitionBll();
        }


        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 将DataRow转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JObject GetJsonObj(DataRow dataRow, string warning = "")
        {
            JObject item = new JObject();
            item["id"] = dataRow["id"].ToString();
            item["createDate"] = dataRow["createDate"].ToString();
            item["pName"] = dataRow["pName"].ToString();
            item["pIdCard"] = dataRow["pIdCard"].ToString();
            item["pAddress"] = dataRow["pAddress"].ToString();
            item["caseType"] = dataRow["caseType"].ToString();
            item["caseName"] = dataRow["caseName"].ToString();
            item["caseCategory"] = dataRow["caseCategory"].ToString();
            item["caseSource"] = dataRow["caseSource"].ToString();
            item["channels"] = dataRow["channels"].ToString();
            item["status"] = int.Parse(dataRow["status"].ToString());
            item["receiver"] = dataRow["receiver"].ToString();
            item["rerm"] = int.Parse(dataRow["rerm"].ToString());
            item["ext1"] = dataRow["EXT1"].ToString();
            item["ext2"] = dataRow["EXT2"].ToString();
            item["ext3"] = dataRow["EXT3"].ToString();
            item["ext4"] = dataRow["EXT4"].ToString();
            item["ext5"] = dataRow["EXT5"].ToString();
            if (warning == "warning")
            {
                item["rermDate"] = dataRow["rermDate"].ToString();
                item["warning"] = dataRow["warning"].ToString();
            }

            item["modifyTime"] = dataRow["modifyTime"].ToString();
            return item;
        }
        public JObject GetJsonObj(Petition dataRow)
        {
            JObject item = new JObject();
            item["id"] = dataRow.id;
            item["createDate"] = DateTime.Parse(dataRow.createDate).ToString("yyyy-MM-dd");
            item["pName"] = dataRow.pName;
            item["pIdCard"] = dataRow.pIdCard;
            item["pAddress"] = dataRow.pAddress;
            item["caseType"] = dataRow.caseType;
            item["caseName"] = dataRow.caseName;
            item["caseCategory"] = dataRow.caseCategory;
            item["caseSource"] = dataRow.caseSource;
            item["channels"] = dataRow.channels;
            item["receiver"] = dataRow.receiver;
            item["rerm"] = dataRow.rerm;
            item["ext1"] = dataRow.ext1;
            item["ext2"] = dataRow.ext2;
            item["ext3"] = dataRow.ext3;
            item["ext4"] = dataRow.ext4;
            item["ext5"] = dataRow.ext5;
            item["status"] = dataRow.status;
            item["modifyTime"] = dataRow.modifyTime.ToString();
            return item;
        }
        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetJsonList(DataTable dt)
        {
            JArray items = new JArray();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(GetJsonObj(dt.Rows[i]));
            }
            return items;
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        public bool Release(string id, int status)
        {
            return dal.Release(id, status);
        }

        /// <summary>
        /// 新增培训记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public string Add(Petition Model)
        {
            return dal.Add(Model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Petition model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 获得培训信息的实体类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Petition GetModel(string id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 创建PDF文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string CreatePetitionPdf(DataTable dt, string keys)
        {
            //案件来源数据字典
            List<DictionaryData> Source = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseSource' ");
            //案件类型数据字典
            List<DictionaryData> Type = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseType' ");
            //案件种类数据字典
            List<DictionaryData> Category = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseCategory' ");
            //来访渠道数据字典
            List<DictionaryData> channels = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'channels' ");

            // 生成PDF数据
            PdfController controller = new PdfController();

            controller.AddColumn("日期");
            controller.AddColumn("当事人姓名");
            controller.AddColumn("身份证号");
            controller.AddColumn("家庭住址");
            controller.AddColumn("案件类型");
            controller.AddColumn("案件名称");
            controller.AddColumn("案件种类");
            controller.AddColumn("案件来源");
            controller.AddColumn("来访渠道");
            controller.AddColumn("接访人");
            controller.AddColumn("办理期限");
            controller.AddColumn("扩展字段1");
            controller.AddColumn("扩展字段2");
            controller.AddColumn("扩展字段3");
            controller.AddColumn("扩展字段4");
            controller.AddColumn("扩展字段5");

            DataRow dataRow;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataRow = dt.Rows[i];

                //案件类型
                DictionaryData typeData = null;
                string typeId = dataRow["caseType"].ToString();
                if (!string.IsNullOrEmpty(typeId))
                {
                    typeData = Type.Find(a => a.ID.Equals(typeId));
                }
                //案件种类
                DictionaryData categoryData = null;
                string categoryId = dataRow["caseCategory"].ToString();
                if (!string.IsNullOrEmpty(categoryId))
                {
                    categoryData = Category.Find(a => a.ID.Equals(categoryId));
                }
                //案件来源
                DictionaryData sourceData = null;
                string sourceId = dataRow["caseSource"].ToString();
                if (!string.IsNullOrEmpty(sourceId))
                {
                    sourceData = Source.Find(a => a.ID.Equals(sourceId));
                }
                //来访渠道
                DictionaryData channelData = null;
                string channelId = dataRow["channels"].ToString();
                if (!string.IsNullOrEmpty(channelId))
                {
                    channelData = channels.Find(a => a.ID.Equals(channelId));
                }

                controller.AddCell(Utils.ObjectToDateTime(dataRow["createDate"]).ToString("yyyy-MM-dd"));
                controller.AddCell(dataRow["pName"].ToString());
                controller.AddCell(dataRow["pIdCard"].ToString(), "left");
                controller.AddCell(dataRow["pAddress"].ToString(), "left");
                controller.AddCell(typeData == null ? "" : typeData.FULLNAME);
                controller.AddCell(dataRow["caseName"].ToString());
                controller.AddCell(categoryData == null ? "" : categoryData.FULLNAME);
                controller.AddCell(sourceData == null ? "" : sourceData.FULLNAME);
                controller.AddCell(channelData == null ? "" : channelData.FULLNAME);
                controller.AddCell(dataRow["receiver"].ToString());
                controller.AddCell(dataRow["rerm"].ToString() + "天");
                controller.AddCell(dataRow["EXT1"].ToString());
                controller.AddCell(dataRow["EXT2"].ToString());
                controller.AddCell(dataRow["EXT3"].ToString());
                controller.AddCell(dataRow["EXT4"].ToString());
                controller.AddCell(dataRow["EXT5"].ToString());

            }
            return controller.CreatePdf("信访案件列表", keys);
        }

        /// <summary>
        /// DataTable转List
        /// </summary>
        public List<Petition> TableToList(DataTable dt)
        {
            List<Petition> list = new List<Petition>();
            Petition model;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                model = GetTransferModel(dt.Rows[i]);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获得实体类
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public Petition GetTransferModel(DataRow row)
        {
            Petition model = new Petition();
            model.createDate = row["createDate"].ToString();
            model.pName = row["pName"].ToString();
            model.pIdCard = row["pIdCard"].ToString();
            model.pAddress = row["pAddress"].ToString();
            model.caseType = row["caseType"].ToString();
            model.caseCategory = row["caseCategory"].ToString();
            model.caseName = row["caseName"].ToString();
            model.caseSource = row["caseSource"].ToString();
            model.channels = row["channels"].ToString();
            model.receiver = row["receiver"].ToString();
            model.rerm = int.Parse(row["rerm"].ToString());
            model.ext1 = row["EXT1"].ToString();
            model.ext2 = row["EXT2"].ToString();
            model.ext3 = row["EXT3"].ToString();
            model.ext4 = row["EXT4"].ToString();
            model.ext5 = row["EXT5"].ToString();

            return model;
        }

        /// <summary>
        /// 单元格内容类
        /// </summary>
        public class ExcelCell
        {
            /// <summary>
            /// 单元格内容
            /// </summary>
            public string value { get; set; }
            /// <summary>
            /// 位置
            /// </summary>
            public string align { get; set; }
        }

        /// <summary>
        /// 单元格内容集合
        /// </summary>
        public List<ExcelCell> listCells;

        /// <summary>
        /// 添加单元格内容
        /// </summary>
        /// <param name="value">单元格内容</param>
        /// <param name="align">位置（左:left,右:right,默认居中：center）</param>
        public void AddCell(string value, string align = "center")
        {
            if (listCells == null)
            {
                listCells = new List<ExcelCell>();
            }
            listCells.Add(new ExcelCell()
            {
                value = value,
                align = align
            });
        }

        /// <summary>
        /// 导出Excel  
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public string ExportExcelPetition(string adminName, List<Petition> list)
        {
            string templateName = "export_petition.xlsx";
            string fileName = "信访案件列表"; //  

            //案件来源数据字典
            List<DictionaryData> Source = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseSource' ");
            //案件类型数据字典
            List<DictionaryData> Type = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseType' ");
            //案件类型数据字典
            List<DictionaryData> Category = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseCategory' ");
            //来访渠道数据字典
            List<DictionaryData> channels = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'channels' ");

            ExcelController controller = new ExcelController();
            int i = 0;
            foreach (Petition model in list)
            {
                //案件类型
                DictionaryData typeData = null;
                if (!string.IsNullOrEmpty(model.caseType))
                {
                    typeData = Type.Find(a => a.ID.Equals(model.caseType));
                }
                //案件种类
                DictionaryData categoryData = null;
                if (!string.IsNullOrEmpty(model.caseCategory))
                {
                    categoryData = Category.Find(a => a.ID.Equals(model.caseCategory));
                }
                //案件来源
                DictionaryData sourceData = null;
                if (!string.IsNullOrEmpty(model.caseSource))
                {
                    sourceData = Source.Find(a => a.ID.Equals(model.caseSource));
                }
                //来访渠道
                DictionaryData channelData = null;
                if (!string.IsNullOrEmpty(model.channels))
                {
                    channelData = channels.Find(a => a.ID.Equals(model.channels));
                }

                controller.AddCell((i + 1) + "");
                controller.AddCell(Utils.ObjectToDateTime(model.createDate).ToString("yyyy-MM-dd"));
                controller.AddCell(model.pName);
                controller.AddCell(model.pIdCard, "left");
                controller.AddCell(model.pAddress, "left");
                controller.AddCell(typeData == null ? "" : typeData.FULLNAME);
                controller.AddCell(model.caseName, "left");
                controller.AddCell(categoryData == null ? "" : categoryData.FULLNAME);
                controller.AddCell(sourceData == null ? "" : sourceData.FULLNAME);
                controller.AddCell(channelData == null ? "" : channelData.FULLNAME);
                controller.AddCell(model.receiver);
                controller.AddCell(model.rerm.ToString() + "天");
                controller.AddCell(model.ext1);
                controller.AddCell(model.ext2);
                controller.AddCell(model.ext3);
                controller.AddCell(model.ext4);
                controller.AddCell(model.ext5);
                i++;
            }
            string filePath = controller.ExportExcel(templateName, fileName, adminName, 17);
            return filePath;
        }
        /// <summary>
        /// 查询预警信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataSet GetWarnPetition(string strWhere)
        {
            return dal.GetWarnPetition(strWhere);
        }
        /// <summary>
        /// 查询过期信息
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataSet GetOverduePetition(string strWhere)
        {
            return dal.GetOverduePetition(strWhere);
        }
        /// <summary>
        /// 将DataTable转为JSON格式数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public JArray GetWarnPetitionJson(DataTable dt)
        {
            JArray items = new JArray();
            JObject item;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = GetJsonObj(dt.Rows[i], "warning");
                items.Add(item);
            }
            return items;
        }
        /// <summary>
        /// 查询图表数据
        /// </summary>
        /// <returns></returns>
        public JObject GetPetitionCharts(string strWhere = "")
        {
            JObject result = new JObject();

            // 饼图数据 
            DataTable dtPie = dal.PieReport(strWhere).Tables[0];

            JArray jsonPie = new JArray();
            DataRow row;
            JObject obj;
            for (int i = 0; i < dtPie.Rows.Count; i++)
            {
                row = dtPie.Rows[i];
                obj = new JObject();
                obj["name"] = row["FULLNAME"].ToString();
                obj["value"] = int.Parse(row["total"].ToString());
                jsonPie.Add(obj);
            }

            result["pie"] = jsonPie;
            return result;
        }

        /// <summary>
        /// 获取excel内容
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns></returns>
        public List<Petition> GetPetitionExcel(string filePath, ref string errMsg)
        {
            try
            {
                List<Petition> list = new List<Petition>();
                using (FileStream fsRead = File.OpenRead(filePath))
                {
                    IWorkbook wk = null;
                    //获取后缀名
                    string fileExt = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                    //判断是否是excel文件
                    if (fileExt == ".xlsx" || fileExt == ".xls")
                    {
                        //判断excel的版本
                        if (fileExt == ".xlsx")
                        {
                            wk = new XSSFWorkbook(fsRead);
                        }
                        else
                        {
                            wk = new HSSFWorkbook(fsRead);
                        }
                        //案件来源数据字典
                        List<DictionaryData> Source = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseSource' ");
                        //案件类型数据字典
                        List<DictionaryData> Type = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseType' ");
                        //案件种类数据字典
                        List<DictionaryData> Category = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'caseCategory' ");
                        //来访渠道数据字典
                        List<DictionaryData> channels = new DictionaryDataController().GetListModel(" AND t2.ENCODE = 'channels' ");
                        ISheet sheet = wk.GetSheetAt(0);
                        Petition model;
                        // 行
                        for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                        {

                            // 获得日期
                            string createDate = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(0));
                            // 当编号内容为空值时，结束操作
                            if (string.IsNullOrEmpty(createDate))
                            {
                                errMsg += " 第" + i + "条，日期不能为空;";
                                continue;
                            }
                            //判断日期格式
                            if (!Utils.IsDateTime(createDate))
                            {
                                errMsg += " 第" + i + "条，日期格式不正确;";
                                continue;
                            }
                            // 获得姓名
                            string pName = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(1));
                            if (string.IsNullOrEmpty(pName))
                            {
                                errMsg += " 第" + i + "条，当事人姓名不能为空;";
                                continue;
                            }

                            //获得身份证号
                            string pIdCard = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(2));
                            if (string.IsNullOrEmpty(pIdCard))
                            {
                                errMsg += " 第" + i + "条，身份证号码不能为空;";
                                continue;
                            }
                            //验证身份证号
                            if (!Utils.CheckIDCard(pIdCard))
                            {
                                errMsg += " 第" + i + "条，身份证号码不正确;";
                                continue;
                            }

                            // 获得地址
                            string pAddress = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(3));
                            if (string.IsNullOrEmpty(pAddress))
                            {
                                pAddress += "";
                            }
                            // 获得案件类型
                            string caseType = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(4));
                            if (string.IsNullOrEmpty(caseType))
                            {
                                errMsg += " 第" + i + "条，案件类型不能为空;";
                                continue;
                            }
                            DictionaryData typeData = Type.Find(a => a.FULLNAME.Equals(caseType));
                            if (typeData == null)
                            {
                                errMsg += " 第" + i + "条，字典中不存在该案件类型;";
                                continue;
                            }

                            // 案件名称
                            string caseName = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(5));
                            if (string.IsNullOrEmpty(caseName))
                            {
                                errMsg += " 第" + i + "条，案件名称不能为空;";
                                continue;
                            }
                            // 获得案件种类
                            string caseCategory = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(6));
                            if (string.IsNullOrEmpty(caseCategory))
                            {
                                errMsg += " 第" + i + "条，案件种类不能为空;";
                                continue;
                            }
                            DictionaryData categoryData = Category.Find(a => a.FULLNAME.Equals(caseCategory));
                            if (categoryData == null)
                            {
                                errMsg += " 第" + i + "条，字典中不存在该案件种类;";
                                continue;
                            }
                            // 获得案件来源
                            string caseSource = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(7));
                            if (string.IsNullOrEmpty(caseSource))
                            {
                                errMsg += " 第" + i + "条，案件来源不能为空;";
                                continue;
                            }
                            DictionaryData sourceData = Source.Find(a => a.FULLNAME.Equals(caseSource));
                            if (sourceData == null)
                            {
                                errMsg += " 第" + i + "条，字典中不存在该案件来源;";
                                continue;
                            }

                            // 获得来访渠道
                            string channel = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(8));
                            if (string.IsNullOrEmpty(channel))
                            {
                                errMsg += " 第" + i + "条，来访渠道不能为空;";
                                continue;
                            }
                            DictionaryData channelData = channels.Find(a => a.FULLNAME.Equals(channel));
                            if (channelData == null)
                            {
                                errMsg += " 第" + i + "条，字典中不存在该来访渠道;";
                                continue;
                            }

                            // 接访人
                            string receiver = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(9));
                            if (string.IsNullOrEmpty(receiver))
                            {
                                errMsg += " 第" + i + "条，接访人不能为空;";
                                continue;
                            }

                            //办理期限
                            string rerm = ExcelController.GetCellValue(sheet.GetRow(i).GetCell(10));
                            if (string.IsNullOrEmpty(rerm))
                            {
                                errMsg += " 第" + i + "条，办理期限不能为空;";
                                continue;
                            }
                            string last = rerm.Substring(rerm.Length - 1, 1);
                            if (last.Equals("天"))
                            {
                                rerm = rerm.Substring(0, rerm.Length - 1);
                            }
                            //判断办理期限是否为数字
                            if (!Utils.IsNumeric(rerm))
                            {
                                errMsg += " 第" + i + "条，办理期限必须为数字天数;";
                                continue;
                            }

                            model = new Petition();
                            model.createDate = createDate;
                            model.pName = pName;
                            model.pIdCard = pIdCard;
                            model.pAddress = pAddress;
                            model.caseName = caseName;
                            model.receiver = receiver;
                            model.rerm = int.Parse(rerm);
                            model.caseSource = sourceData.ID;
                            model.caseType = typeData.ID;
                            model.caseCategory = categoryData.ID;
                            model.channels = channelData.ID;
                            model.modifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            list.Add(model);
                        }
                    }

                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public bool AddList(List<Petition> listModel)
        {
            return dal.AddList(listModel);
        }
    }
}
