/*--------------------------------------------------
 * 作用：对Excel进行处理
 -------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;

namespace Utility
{
    /// <summary>
    /// Excel处理类
    /// </summary>
    public class ExcelHandel
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataSource">数据源，Excel名字为Table名</param>
        public static void ExportExcel(DataTable dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentException("dataSource is null");
            }
            ExportExcel(dataSource, dataSource.TableName);
        }
        /// <summary>
        /// 导出Excel(HTTP)
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="sheetName">Excel名字</param>
        public static void ExportExcel(DataTable dataSource, string sheetName)
        {
            MemoryStream _stream = GetExcelData(dataSource, sheetName);

            HttpResponse _response = HttpContext.Current.Response;
            _response.ContentType = "application/vnd.ms-excel";
            _response.Charset = "GB2312";
            _response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", string.Concat(sheetName, ".xls")));
            _response.ContentEncoding = System.Text.Encoding.Default;
            _response.Clear();
            _response.BinaryWrite(_stream.GetBuffer());
            _response.Flush();
            _response.End();
        }
        /// <summary>
        /// 导出Excel(HTTP)
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="sheetName">Excel名字</param>
        public static void ExportExcel(DataTable dataSource, string sheetName, HttpContext context)
        {
            MemoryStream _stream = ExportExcelMVC(dataSource, sheetName);

            HttpResponse _response = context.Response;
            _response.ContentType = "application/vnd.ms-excel";
            _response.Charset = "GB2312";
            _response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", string.Concat(sheetName, ".xls")));
            _response.ContentEncoding = System.Text.Encoding.Default;
            _response.Clear();
            _response.BinaryWrite(_stream.GetBuffer());
            _response.Flush();
        }

        /// <summary>
        /// 导出Excel(MVC)
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="sheetName">Excel名字</param>
        public static MemoryStream ExportExcelMVC(DataTable dataSource, string sheetName)
        {
            MemoryStream _stream = GetExcelData(dataSource, sheetName);
            _stream.Seek(0, SeekOrigin.Begin);

            return _stream;
        }

        #region 数据获取
        /// <summary>
        /// 获取Excel内存数据
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="sheetName">Excel名字</param>
        /// <returns></returns>
        private static MemoryStream GetExcelData(DataTable dataSource, string sheetName)
        {
            if (dataSource == null)
            {
                throw new ArgumentException("dataSource is null");
            }
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = "UnKown";
            }
            //打开Excel对象
            HSSFWorkbook _book = new HSSFWorkbook();

            DocumentSummaryInformation _dom_summary_info = PropertySetFactory.CreateDocumentSummaryInformation();
            _dom_summary_info.Company = "";
            _book.DocumentSummaryInformation = _dom_summary_info;

            SummaryInformation _summary_info = PropertySetFactory.CreateSummaryInformation();
            _summary_info.Subject = "";
            _book.SummaryInformation = _summary_info;

            //Excel的Sheet对象
            ISheet _sheet = _book.CreateSheet(sheetName);
            DataColumnCollection _columns = dataSource.Columns;
            DataRowCollection _rows = dataSource.Rows;
            IRow _row = _sheet.CreateRow(0);

            // 用于格式化单元格的数据
            HSSFDataFormat format = (HSSFDataFormat)_book.CreateDataFormat();

            // 设置列头字体
            HSSFFont font = (HSSFFont)_book.CreateFont();
            font.FontHeightInPoints = 18; //字体高度
            //font.Color = (short)FontColor.RED;
            font.FontName = "隶书"; //字体
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.IsItalic = true; //是否使用斜体
            //font.IsStrikeout = true; //是否使用划线
            //设置文本字体
            HSSFFont fontContent = (HSSFFont)_book.CreateFont();
            fontContent.FontHeightInPoints = 10;
            fontContent.FontName = "宋体";

            // 设置单元格类型
            HSSFCellStyle cellStyle = (HSSFCellStyle)_book.CreateCellStyle();
            cellStyle.SetFont(font);
            cellStyle.Alignment = HorizontalAlignment.Center; //水平布局：居中
            cellStyle.WrapText = false;

            // 添加单元格注释
            // 创建HSSFPatriarch对象,HSSFPatriarch是所有注释的容器.
            HSSFPatriarch patr = (HSSFPatriarch)_sheet.CreateDrawingPatriarch();
            // 定义注释的大小和位置,详见文档
            HSSFComment comment = patr.CreateComment(new HSSFClientAnchor(0, 0, 0, 0, (short)4, 2, (short)6, 5));
            // 设置注释内容
            comment.String = new HSSFRichTextString("列标题");
            // 设置注释作者. 当鼠标移动到单元格上是可以在状态栏中看到该内容.
            comment.Author = "天蓝海";

            //set date format
            ICellStyle cellStyleDate = _book.CreateCellStyle();
            cellStyleDate.SetFont(fontContent);
            cellStyleDate.Alignment = HorizontalAlignment.Center; //水平布局：居中
            cellStyleDate.DataFormat = format.GetFormat("yyyy-m-d HH:MM:SS");
            //数据格式
            ICellStyle cellStyleNumber = _book.CreateCellStyle();
            cellStyleNumber.SetFont(fontContent);
            cellStyleNumber.Alignment = HorizontalAlignment.Center; //水平布局：居中
            cellStyleNumber.DataFormat = format.GetFormat("0.00");


            //生成列名，根据DataTable的列名
            for (int i = 0; i < _columns.Count; i++)
            {
                // 创建单元格
                ICell cell = _row.CreateCell(i);
                HSSFRichTextString hssfString = new HSSFRichTextString(_columns[i].ColumnName);
                cell.SetCellValue(hssfString);//设置单元格内容
                cell.CellStyle = cellStyle;//设置单元格样式
                cell.SetCellType(CellType.String);//指定单元格格式：数值、公式或字符串
                cell.CellComment = comment;//添加注释
            }
            //填充数据
            for (int j = 0; j < _rows.Count; j++)
            {
                _row = _sheet.CreateRow(j + 1);
                for (int k = 0; k < _columns.Count; k++)
                {
                    ICell cell = _row.CreateCell(k);
                    switch (_columns[k].DataType.ToString())
                    {
                        case "System.String":
                            HSSFRichTextString hssfString = new HSSFRichTextString(_rows[j][k].ToString());
                            cell.SetCellValue(hssfString);
                            cell.SetCellType(CellType.String);
                            break;
                        case "System.Decimal":
                            cell.SetCellValue((double)(decimal)_rows[j][k]);
                            cell.CellStyle = cellStyleNumber;
                            break;
                        case "System.DateTime":
                            cell.SetCellValue((DateTime)_rows[j][k]);
                            cell.CellStyle = cellStyleDate;
                            break;
                    }
                    //自动换行
                    if (_rows[j][k].ToString().Contains("\r\n"))
                    {
                        cell.CellStyle.WrapText = true;
                    }
                }
            }
            //设置自动列宽
            for (int i = 0; i < _columns.Count; i++)
            {
                _sheet.AutoSizeColumn(i);
            }
            //保存excel文档
            _sheet.ForceFormulaRecalculation = true;

            MemoryStream _stream = new MemoryStream();
            _book.Write(_stream);

            return _stream;
        }
        #endregion
    }
}
