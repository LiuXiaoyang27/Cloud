using Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Controller
{
    /// <summary>
    /// 导出PDF操作类
    /// </summary>
    public class PdfController
    {
        // 表格表头
        private List<string> listColumn = null;
        // 表格内容
        private List<PdfCell> listCell = null;

        /// <summary>
        /// 添加表头
        /// </summary>
        /// <param name="column">表头内容</param>
        public void AddColumn(string column)
        {
            if (listColumn == null)
            {
                listColumn = new List<string>();
            }

            listColumn.Add(column);
        }

        /// <summary>
        /// 添加表头
        /// </summary>
        /// <param name="columns">表头内容</param>
        public void AddColumn(List<string> columns)
        {
            if (listColumn == null)
            {
                listColumn = new List<string>();
            }

            listColumn.AddRange(columns);
        }

        /// <summary>
        /// 表格内容类
        /// </summary>
        public class PdfCell
        {
            /// <summary>
            /// 表格内容
            /// </summary>
            public string value { get; set; }
            /// <summary>
            /// 水平方位
            /// </summary>
            public string align { get; set; }
        }
        /// <summary>
        /// 添加表格内容
        /// </summary>
        /// <param name="cells">表格内容</param>
        public void AddCell(List<string> cells)
        {
            if (listCell == null)
            {
                listCell = new List<PdfCell>();
            }
            for (int i = 0; i < cells.Count; i++)
            {
                listCell.Add(new PdfCell()
                {
                    value = cells[i],
                    align = "center"
                });
            }
        }

        /// <summary>
        /// 新增表格内容
        /// </summary>
        /// <param name="value">表格内容</param>
        /// <param name="align">水平方位</param>
        public void AddCell(string value, string align = "center")
        {
            if (listCell == null)
            {
                listCell = new List<PdfCell>();
            }

            listCell.Add(new PdfCell()
            {
                value = value,
                align = align
            });
        }

        /// <summary>
        /// 获得创建的文件名字
        /// </summary>
        /// <returns></returns>
        public string GetFileName(string name)
        {
            return name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
        }

        /// <summary>
        /// 创建PDF文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string CreatePdf(string name, string keys)
        {
            if (listCell == null || listColumn == null)
            {
                return "";
            }
            //创建一个pdf文档的对象，设置纸张大小为A4，页边距为0
            Document document = new Document(PageSize.A4.Rotate(), 5, 5, 5, 5);
            // 设置文件名称
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            string fileName = GetFileName(name);
            string downloadPath = Utils.GetMapPath("/data/download/");
            // 如果不存在文件，创建
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            string filePath = downloadPath + fileName;
            //创建一个写入PDF的对象
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write));
            document.Open();//打开

            // 设置字体类
            Font cn = new Font(baseFont, 12, Font.NORMAL);
            Font titleFont = new Font(baseFont, 14, Font.BOLD);

            PdfPCell cell;

            // 添加标题
            Paragraph title = new Paragraph(name + "\n\n", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // 条件部分Table
            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.WidthPercentage = 100; // 宽度100%填充

            // 条件
            cell = GetPdfCell(keys, "left", cn);
            cell.Border = 0;
            tableHeader.AddCell(cell);

            // 日期
            string createDate = "创建日期:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            cell = GetPdfCell(createDate, "right", cn);
            cell.Border = 0;
            tableHeader.AddCell(cell);

            document.Add(tableHeader);

            int columnCount = listColumn.Count;
            // 内容部分Table
            PdfPTable table = new PdfPTable(columnCount);
            table.WidthPercentage = 100; // 宽度100%填充

            // 创建表格列名
            for (int i = 0; i < columnCount; i++)
            {
                //正文第1行
                cell = GetPdfCell(listColumn[i], "center", titleFont);
                table.AddCell(cell);
            }
            // 创建表格内容
            PdfCell pdfCell;
            for (int i = 0; i < listCell.Count; i += columnCount)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    pdfCell = listCell[i + j];
                    cell = GetPdfCell(pdfCell.value, pdfCell.align, cn);
                    table.AddCell(cell);
                }

            }
            document.Add(table);
            //关闭文档
            document.Close();

            // 关闭书写器
            writer.Close();

            return "/data/download/" + fileName;
        }

        /// <summary>
        /// 创建PDF文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string CreatePdf(string name, List<PdfCell> header, List<PdfCell> footer)
        {
            if (listCell == null || listColumn == null)
            {
                return "";
            }
            //创建一个pdf文档的对象，设置纸张大小为A4，页边距为0
            Document document = new Document(PageSize.A4.Rotate(), 5, 5, 5, 5);
            // 设置文件名称
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            string fileName = GetFileName(name);
            string downloadPath = Utils.GetMapPath("/data/download/");
            // 如果不存在文件，创建
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            string filePath = downloadPath + fileName;
            //创建一个写入PDF的对象
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write));
            document.Open();//打开

            // 设置字体类
            Font cn = new Font(baseFont, 12, Font.NORMAL);
            Font titleFont = new Font(baseFont, 14, Font.BOLD);

            PdfPCell cell;

            // 添加标题
            Paragraph title = new Paragraph(name + "\n\n", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            // 条件部分Table
            PdfPTable tableHeader = new PdfPTable(header.Count);
            tableHeader.WidthPercentage = 100; // 宽度100%填充

            for (int i = 0; i < header.Count; i++)
            {
                //正文第1行
                cell = GetPdfCell(header[i].value, header[i].align, cn);
                cell.Border = 0;
                tableHeader.AddCell(cell);
            }

            document.Add(tableHeader);

            int columnCount = listColumn.Count;
            // 内容部分Table
            PdfPTable table = new PdfPTable(columnCount);
            table.WidthPercentage = 100; // 宽度100%填充

            // 创建表格列名
            for (int i = 0; i < columnCount; i++)
            {
                //正文第1行
                cell = GetPdfCell(listColumn[i], "center", titleFont);
                table.AddCell(cell);
            }
            // 创建表格内容
            PdfCell pdfCell;
            for (int i = 0; i < listCell.Count; i += columnCount)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    pdfCell = listCell[i + j];
                    cell = GetPdfCell(pdfCell.value, pdfCell.align, cn);
                    table.AddCell(cell);
                }

            }
            document.Add(table);

            // 底部部分Table
            PdfPTable tableFooter = new PdfPTable(footer.Count);
            tableFooter.WidthPercentage = 100; // 宽度100%填充

            for (int i = 0; i < footer.Count; i++)
            {
                //正文第1行
                cell = GetPdfCell(footer[i].value, footer[i].align, cn);
                cell.Border = 0;
                tableFooter.AddCell(cell);
            }

            document.Add(tableFooter);

            //关闭文档
            document.Close();

            // 关闭书写器
            writer.Close();

            return "/data/download/" + fileName;
        }

        /// <summary>
        /// 获得PDF单元格类
        /// </summary>
        /// <param name="content"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public PdfPCell GetPdfCell(string content, string align, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.MinimumHeight = 28; // 设置单元格高度
            cell.UseAscender = true; // 设置可以居中
            switch (align)
            {
                case "left":
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;      // 设置水平居左
                    break;
                case "right":
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;     // 设置水平居右
                    break;
                default:
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;    // 设置水平居中
                    break;
            }

            cell.VerticalAlignment = Element.ALIGN_MIDDLE; // 设置垂直居中
            return cell;
        }
    }
}
