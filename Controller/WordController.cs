using Model;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.IO;
using Spire.Doc.Collections;
using Spire.Doc.Interface;
using System.Data;
using Common;
using System.Text;

namespace Controller
{
    /// <summary>
    /// Word操作类
    /// </summary>
    public class WordController
    {
        #region GetPetitionWord:获取Word内容
        /// <summary>
        /// 获取Word内容
        /// </summary>
        /// <param name="filePath">Word文件路径</param>
        /// <returns></returns>
        public List<Petition> GetPetitionWord(string filePath, ref string errMsg)
        {
            try
            {
                List<Petition> listModel = new List<Petition>();               
                using (FileStream fsRead = File.OpenRead(filePath))
                {
                    Document doc = null;
                    //获取后缀名
                    string fileExt = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                    //判断是否是Word文件
                    if (fileExt == ".docx")
                    {
                        doc = new Document(fsRead);

                        //获得培训信息
                        Section section = null;//doc.Sections[0];                  
                        //标题名
                        string title = "";
                        string col1 = "";
                        string col2 = "";
                        string col3 = "";
                        string col4 = "";
                        string col5 = "";
                        string col6 = "";
                        string col7 = "";
                        //字段名
                        string name = "";
                        string trainDate = "";
                        string target = "";
                        string content = "";
                        string plan = "";
                        string report = "";
                        string briefNews = "";

                        for (int j = 0; j < doc.Sections.Count; j++)
                        {
                            section = doc.Sections[j];
                            //当前页的表格
                            ITable table = section.Tables[0];
                            if (table.Rows.Count != 12)
                            {
                                continue;
                            }
                            //标题名
                            title = table.Rows[0].Cells[0].Paragraphs[0].Text.Trim();
                            col1 = table.Rows[1].Cells[0].Paragraphs[0].Text.Trim();
                            col2 = table.Rows[2].Cells[0].Paragraphs[0].Text.Trim();
                            col3 = table.Rows[3].Cells[0].Paragraphs[0].Text.Trim();
                            col4 = table.Rows[4].Cells[0].Paragraphs[0].Text.Trim();
                            col5 = table.Rows[6].Cells[0].Paragraphs[0].Text.Trim();
                            col6 = table.Rows[8].Cells[0].Paragraphs[0].Text.Trim();
                            col7 = table.Rows[10].Cells[0].Paragraphs[0].Text.Trim();
                            //字段名
                            name = "";
                            trainDate = "";
                            target = "";
                            content = "";
                            plan = "";
                            report = "";
                            briefNews = "";
                            if (title == "培训" && col1 == "培训名称" && col2 == "培训日期" && col3 == "培训对象"
                                && col4 == "培训内容" && col5 == "培训计划" && col6 == "培训报告" && col7 == "培训简讯")
                            {
                                
                                for (int i = 0; i < table.Rows[1].Cells[1].Paragraphs.Count; i++)
                                {
                                    name += table.Rows[1].Cells[1].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[2].Cells[1].Paragraphs.Count; i++)
                                {
                                    trainDate += table.Rows[2].Cells[1].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[3].Cells[1].Paragraphs.Count; i++)
                                {
                                    target += table.Rows[3].Cells[1].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[5].Cells[0].Paragraphs.Count; i++)
                                {
                                    content += table.Rows[5].Cells[0].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[7].Cells[0].Paragraphs.Count; i++)
                                {
                                    plan += table.Rows[7].Cells[0].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[9].Cells[0].Paragraphs.Count; i++)
                                {
                                    report += table.Rows[9].Cells[0].Paragraphs[i].Text;
                                }
                                for (int i = 0; i < table.Rows[11].Cells[0].Paragraphs.Count; i++)
                                {
                                    briefNews += table.Rows[11].Cells[0].Paragraphs[i].Text;
                                }
                            }
                            else
                            {
                                continue;
                            }

                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(trainDate) || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(content))
                            {
                                continue;
                            }
                            else if (!Utils.IsDateTime(trainDate))
                            {
                                continue;
                            }
                            else if (name.Length > 30)
                            {
                                continue;
                            }
                            else if (target.Length > 30)
                            {
                                continue;
                            }
                            else
                            {
                                Petition model = new Petition();
                                //model.name = name;
                                //model.target = target;
                                //model.trainDate = trainDate;
                                //model.content = content;
                                //model.plan = plan;
                                //model.report = report;
                                //model.briefNews = briefNews;
                                model.modifyTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                listModel.Add(model);
                            }
                        }
                       
                    }
                }

                return listModel;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;
            }
        }
        #endregion        

        #region ExportWord:导出到Word
        /// <summary>
        /// 导出到word
        /// </summary>
        /// <param name="dataTable"></param>
        public string ExportWord(DataTable dataTable, string fileName)
        {
            try
            {
                //获取word模版的绝对路径
                string templatePath = Utils.GetMapPath("/data/templates/petition.docx");

                //导出文件的路径
                string savePath = "";

                DataRow dataRow;

                Document doc = new Document(templatePath);
                //获取Word第一页  
                Section section1 = doc.Sections[0];
                //第一页的表格
                ITable table = section1.Tables[0];

                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows.Count >= 1)
                    {
                        //复制表格                                           
                        for (int i = 1; i < dataTable.Rows.Count; i++)
                        {
                            //新增一页
                            Section section = doc.AddSection();
                            //复制表格
                            ITable tab = (ITable)table.Clone();
                            //将第一页的表插入到当前页
                            section.Tables.Insert(0, tab);

                        }
                        //导出
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            //导出第 i 页
                            table = doc.Sections[i].Tables[0];//当前页的表格
                            dataRow = dataTable.Rows[i];

                            ITextRange range1;
                            range1 = table.Rows[1].Cells[1].Paragraphs[0].AppendText(dataRow["name"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[2].Cells[1].Paragraphs[0].AppendText(dataRow["trainDate"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[3].Cells[1].Paragraphs[0].AppendText(dataRow["target"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[5].Cells[0].Paragraphs[0].AppendText(dataRow["detail"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[7].Cells[0].Paragraphs[0].AppendText(dataRow["plandetail"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[9].Cells[0].Paragraphs[0].AppendText(dataRow["report"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[11].Cells[0].Paragraphs[0].AppendText(dataRow["briefNews"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                        }
                        //文件下载路径
                        string downloadPath = Utils.GetMapPath("/data/download/");
                        // 如果路径不存在，创建
                        if (!Directory.Exists(downloadPath))
                        {
                            Directory.CreateDirectory(downloadPath);
                        }
                        //导出文件的路径
                        savePath = downloadPath + fileName;
                        //保存文档
                        doc.SaveToFile(savePath);

                    }
                    return savePath;
                }
                else
                {
                    //没有数据
                    return null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region ExportDrillWord:导出到Word
        /// <summary>
        /// 导出到word
        /// </summary>
        /// <param name="dataTable"></param>
        public string ExportDrillWord(DataTable dataTable, string fileName)
        {
            try
            {
                //获取word模版的绝对路径
                string templatePath = Utils.GetMapPath("/data/templates/drill.docx");

                //导出文件的路径
                string savePath = "";

                DataRow dataRow;

                Document doc = new Document(templatePath);
                //获取Word第一页  
                Section section1 = doc.Sections[0];
                //第一页的表格
                ITable table = section1.Tables[0];

                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows.Count >= 1)
                    {
                        //复制表格                                           
                        for (int i = 1; i < dataTable.Rows.Count; i++)
                        {
                            //新增一页
                            Section section = doc.AddSection();
                            //复制表格
                            ITable tab = (ITable)table.Clone();
                            //将第一页的表插入到当前页
                            section.Tables.Insert(0, tab);

                        }
                        //导出
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            //导出第 i 页
                            table = doc.Sections[i].Tables[0];//当前页的表格
                            dataRow = dataTable.Rows[i];

                            ITextRange range1;
                            range1 = table.Rows[1].Cells[1].Paragraphs[0].AppendText(dataRow["item"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[2].Cells[1].Paragraphs[0].AppendText(dataRow["drillDate"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[3].Cells[1].Paragraphs[0].AppendText(dataRow["linkMan"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[4].Cells[1].Paragraphs[0].AppendText(dataRow["address"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[6].Cells[0].Paragraphs[0].AppendText(dataRow["project"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[8].Cells[0].Paragraphs[0].AppendText(dataRow["report"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[10].Cells[0].Paragraphs[0].AppendText(dataRow["briefNews"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[12].Cells[0].Paragraphs[0].AppendText(dataRow["remark"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                        }
                        //文件下载路径
                        string downloadPath = Utils.GetMapPath("/data/download/");
                        // 如果路径不存在，创建
                        if (!Directory.Exists(downloadPath))
                        {
                            Directory.CreateDirectory(downloadPath);
                        }
                        //导出文件的路径
                        savePath = downloadPath + fileName;
                        //保存文档
                        doc.SaveToFile(savePath);

                    }
                    return savePath;
                }
                else
                {
                    //没有数据
                    return null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region ExportAgreeWord:导出到Word
        /// <summary>
        /// 导出到word
        /// </summary>
        /// <param name="dataTable"></param>
        public string ExportAgreeWord(DataTable dataTable, string fileName)
        {
            try
            {
                //获取word模版的绝对路径
                string templatePath = Utils.GetMapPath("/data/templates/agree.docx");

                //导出文件的路径
                string savePath = "";

                DataRow dataRow;

                Document doc = new Document(templatePath);
                //获取Word第一页  
                Section section1 = doc.Sections[0];
                //第一页的表格
                ITable table = section1.Tables[0];

                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows.Count >= 1)
                    {
                        //复制表格                                           
                        for (int i = 1; i < dataTable.Rows.Count; i++)
                        {
                            //新增一页
                            Section section = doc.AddSection();
                            //复制表格
                            ITable tab = (ITable)table.Clone();
                            //将第一页的表插入到当前页
                            section.Tables.Insert(0, tab);

                        }
                        //导出
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            //导出第 i 页
                            table = doc.Sections[i].Tables[0];//当前页的表格
                            dataRow = dataTable.Rows[i];

                            ITextRange range1;
                            range1 = table.Rows[1].Cells[1].Paragraphs[0].AppendText(dataRow["item"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[2].Cells[1].Paragraphs[0].AppendText(dataRow["accidentName"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[3].Cells[1].Paragraphs[0].AppendText(dataRow["deptName"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[4].Cells[1].Paragraphs[0].AppendText(dataRow["customerName"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[5].Cells[1].Paragraphs[0].AppendText(dataRow["linkManName"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[6].Cells[1].Paragraphs[0].AppendText(dataRow["drillDate"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[8].Cells[0].Paragraphs[0].AppendText(dataRow["project"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                            range1 = table.Rows[10].Cells[0].Paragraphs[0].AppendText(dataRow["remark"].ToString());
                            range1.CharacterFormat.FontName = "宋体";
                            range1.CharacterFormat.FontSize = 11;
                        }
                        //文件下载路径
                        string downloadPath = Utils.GetMapPath("/data/download/");
                        // 如果路径不存在，创建
                        if (!Directory.Exists(downloadPath))
                        {
                            Directory.CreateDirectory(downloadPath);
                        }
                        //导出文件的路径
                        savePath = downloadPath + fileName;
                        //保存文档
                        doc.SaveToFile(savePath);

                    }
                    return savePath;
                }
                else
                {
                    //没有数据
                    return null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}
