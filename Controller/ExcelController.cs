using Common;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Controller
{
    /// <summary>
    /// Excel操作类
    /// </summary>
    public class ExcelController
    {
        /// <summary>
        /// 获得单元格的内容
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string GetCellValue(ICell cell)
        {
            string content = string.Empty;

            if (cell == null)
            {
                return string.Empty;
            }
            switch (cell.CellType)
            {
                //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                case CellType.Blank:
                    content = string.Empty;
                    break;
                case CellType.Boolean: //bool类型
                    content = cell.BooleanCellValue.ToString();
                    break;
                case CellType.Error:
                    content = cell.ErrorCellValue.ToString();
                    break;
                case CellType.Numeric: //数字类型
                    if (DateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        content = cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        content = cell.NumericCellValue.ToString();
                    }
                    break;
                case CellType.String: //string 类型
                    content = cell.StringCellValue;
                    break;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        content = cell.ToString();
                    }
                    catch
                    {
                        content = cell.NumericCellValue.ToString();
                    }
                    break;
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    content = cell.ToString();
                    break;
            }

            return content;
        }

        public static string GetCellValue(ISheet sheet, int rowNum, int cellNum)
        {
            IRow iRow = sheet.GetRow(rowNum);
            if (iRow == null)
            {
                return "";
            }

            ICell iCell = iRow.GetCell(cellNum);
            if (iCell == null)
            {
                return string.Empty;
            }

            return GetCellValue(iCell);
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
        /// 导出Export通用方法，仅使用统一模版样式
        /// </summary>
        /// <param name="templateName">模版名称（需加后缀）</param>
        /// <param name="fileName">导出文件名（不需要后缀）</param>
        /// <param name="adminName">操作员姓名</param>
        /// <param name="columnCount">列数</param>
        /// <returns></returns>
        public string ExportExcel(string templateName, string fileName, string adminName, int columnCount)
        {
            if (listCells == null)
            {
                return "";
            }
            // 模板文件绝对地址
            string templatePath = Utils.GetMapPath("/data/templates/" + templateName);
            // 获得文件操作类
            FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            // 此处默认xlsx，如果是xls需要进行其他判断。
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            ISheet sheet = workbook.GetSheetAt(0);

            // 一般单元格样式
            ICellStyle normalStyle = GetNormalStyle(workbook);
            ICellStyle leftStyle = GetNormalStyle(workbook, "left");
            ICellStyle rightStyle = GetNormalStyle(workbook, "right");

            // 标题单元格样式
            ICellStyle cellTitle = GetTitleStyle(workbook);

            // 标题内容
            string headerInfo = "操作人：" + adminName + "             操作时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // 创建标题栏
            IRow row = sheet.CreateRow(1);
            row.Height = 24 * 20;
            ICell cell = row.CreateCell(0);

            cell.SetCellValue(headerInfo);
            cell.CellStyle = cellTitle;

            IRow iRow;
            ICell iCell;
            ExcelCell excelCell;

            int rowCount = listCells.Count / columnCount;
            // 行
            for (int i = 0; i < rowCount; i++)
            {

                iRow = sheet.CreateRow(i + 3);
                iRow.Height = 22 * 20;
                for (int j = 0; j < columnCount; j++)
                {
                    excelCell = listCells[i * columnCount + j];
                    iCell = iRow.CreateCell(j);
                    iCell.SetCellValue(excelCell.value);
                    switch (excelCell.align)
                    {
                        case "left":
                            iCell.CellStyle = leftStyle;
                            break;
                        case "right":
                            iCell.CellStyle = rightStyle;
                            break;
                        default:
                            iCell.CellStyle = normalStyle;
                            break;
                    }
                }
            }
            sheet.ForceFormulaRecalculation = true;
            string downloadPath = Utils.GetMapPath("/data/download/");
            // 如果不存在文件，创建
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            // 导出文件路径
            string downloadName = GetFileName(fileName);
            string filePath = downloadPath + downloadName;

            using (FileStream filess = File.OpenWrite(filePath))
            {
                workbook.Write(filess);
            }

            return "/data/download/" + downloadName;
        }

        private void RegionBorder(ISheet sheet, ICellStyle style, int startRow, int endRow, int cell)
        {
            //IRow iRow;
            //ICell iCell;
            //for (int i = startRow; i < endRow; i++)
            //{
            //    // 补全合并单元格边框
            //    iRow = sheet.GetRow(i);
            //    iCell = iRow.CreateCell(cell);
            //    iCell.CellStyle = style;
            //}

        }
        /// <summary>
        /// 获得创建的文件名字
        /// </summary>
        /// <returns></returns>
        private string GetFileName(string name)
        {
            return name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
        }
        /// <summary>
        /// 设置一般单元格样式
        /// </summary>
        /// <param name="cellStyle"></param>
        public static ICellStyle GetNormalStyle(IWorkbook workbook, string alignment = "")
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            // 上下垂直居中
            switch (alignment)
            {
                case "left":
                    cellStyle.Alignment = HorizontalAlignment.Left;
                    break;
                case "right":
                    cellStyle.Alignment = HorizontalAlignment.Right;
                    break;
                default:
                    cellStyle.Alignment = HorizontalAlignment.Center;
                    break;
            }

            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            // 日期格式
            //cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-MM-dd HH:mm:ss");
            //设置单元格字体
            IFont font = workbook.CreateFont();
            font.FontHeight = 10;//设置字体大小
            font.FontName = "微软雅黑";//设置字体为黑体
            cellStyle.SetFont(font);
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;
            return cellStyle;
        }

        /// <summary>
        /// 设置标题单元格样式
        /// </summary>
        /// <param name="cellStyle"></param>
        public static ICellStyle GetTitleStyle(IWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            // 上下垂直居中
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            // 日期格式
            cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-MM-dd HH:mm:ss");
            // 设置标题样式
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeight = 10;
            font.FontName = "微软雅黑";
            cellStyle.SetFont(font);
            return cellStyle;
        }
    }
}

