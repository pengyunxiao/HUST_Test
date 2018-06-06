using System;
using System.Data;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Text;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace ExportTools
{
    /// <summary>
    /// 该类使用NPOI 2.1库和OpenXML把DataTable和DataGridView导出为03或者07版本的excel表格。
    /// </summary>
    public class ExportToExcel
    {
        /// <summary>
        /// 导出一组sheet表到某个2003版本的Excel文件中。
        /// </summary>
        /// <param name="dts">一簇表格</param>
        /// <param name="filename">excel文件路径</param>
        /// <param name="sheetNames">每个sheet表的名字</param>
        /// <param name="titleList">保存所有Sheet表标题的列表</param>
        /// <param name="units">数字单位描述字符串</param>
        /// <param name="descs">额外的描述信息</param>
        public static void export03SheetGroups(DataTable[] dts, string filename, 
            string[] sheetNames, string[] titleList,
            string[] units = null, string[] descs = null)
        {
            if (dts.Length != sheetNames.Length)
            {
                throw new Exception("将要被导出的DataTable的个数" + dts.Length +
                    "与表哥名数组的长度" + sheetNames.Length + "不匹配");
            }
            IWorkbook workbook = new HSSFWorkbook();
            for (int i = 0, size = dts.Length; i < size; i++)
                exportSingleSheet(dts[i], sheetNames[i], titleList[i], units[i], descs[i], workbook);
            writeToFile(filename, workbook);
        }

        /// <summary>
        /// 导出一组sheet表到某个2007版本的Excel文件中。
        /// </summary>
        /// <param name="dts">一簇表格</param>
        /// <param name="filename">excel文件路径</param>
        /// <param name="sheetNames">每个sheet表的名字</param>
        /// <param name="titleList">保存所有Sheet表标题的列表</param>
        /// <param name="units">数字单位描述字符串</param>
        /// <param name="desc">额外的描述信息</param>
        public static void export07SheetGroups(DataTable[] dts, string filename, 
            string[] sheetNames, string[] titleList,
            string[] units = null, string[] descs = null)
        {
            if (dts.Length != sheetNames.Length)
            {
                throw new Exception("将要被导出的DataTable的个数" + dts.Length +
                    "与表哥名数组的长度" + sheetNames.Length + "不匹配");
            }

            writeTo07Excel(convertDGVToDT(dts, sheetNames, titleList, units, descs), filename);
        }

        /// <summary>
        /// 导出一组sheet表到某个2003版本的Excel文件中。
        /// </summary>
        /// <param name="dgvs">一簇表格</param>
        /// <param name="filename">excel文件路径</param>
        /// <param name="sheetNames">每个sheet表的名字</param>
        /// <param name="titles">保存所有Sheet表标题的列表</param>
        /// <param name="unit">数字单位描述字符串</param>
        /// <param name="desc">额外的描述信息</param>
        public static void export03SheetGroups(DataGridView[] dgvs, string filename,
            string[] sheetNames, string[] titles, string[] units = null, string[] descs = null)
        {
            if (dgvs.Length != sheetNames.Length)
            {
                throw new Exception("将要被导出的DataTable的个数" + dgvs.Length +
                    "与表哥名数组的长度" + sheetNames.Length + "不匹配");
            }
            IWorkbook workbook = new HSSFWorkbook();
            for (int i = 0, size = dgvs.Length; i < size; i++)
                exportSingleSheet(dgvs[i], sheetNames[i], titles[i], units[i], descs[i], workbook);
            writeToFile(filename, workbook);
        }

        /// <summary>
        /// 导出一组sheet表到某个2007版本的Excel文件中。
        /// </summary>
        /// <param name="dgvs">一簇表格</param>
        /// <param name="filename">excel文件路径</param>
        /// <param name="sheetNames">每个sheet表的名字</param>
        /// <param name="titles">保存所有Sheet表标题的列表</param>
        /// <param name="units">数字单位描述字符串</param>
        /// <param name="descs">额外的描述信息</param>
        public static void export07SheetGroups(DataGridView[] dgvs, string filename,
            string[] sheetNames,
            string[] titles,
            string[] units = null,
            string[] descs = null)
        {
            writeTo07Excel(convertDGVToDT(dgvs, sheetNames, titles, units, descs), filename);
        }

        /// <summary>
        /// 导出一个DataTable到指定的2003版本的Excel文件中，单元表名由参数指定。
        /// </summary>
        /// <param name="dt">被导出的表</param>
        /// <param name="filename">Excel文件路径</param>
        /// <param name="sheetName">sheet表名</param>
        /// <param name="title">Sheet表的标题</param>
        /// <param name="unit">数字单位</param>
        /// <param name="desc">描述信息</param>
        public static void export03(DataTable dt, string filename, 
            string sheetName, string title,
            string unit = "", string desc = "")
        {
            IWorkbook workbook = new HSSFWorkbook();
            exportSingleSheet(dt, sheetName, title, unit, desc, workbook);
            writeToFile(filename, workbook);
        }

        /// <summary>
        /// 导出一个DataGridView到指定的2003版本的Excel文件中，单元表名由参数指定。
        /// </summary>
        /// <param name="dgv">被导出的表</param>
        /// <param name="filename">Excel文件路径</param>
        /// <param name="sheetName">sheet表名</param>
        /// <param name="title">Sheet表的标题</param>
        /// <param name="unit">数字单位</param>
        /// <param name="desc">描述信息</param>
        public static void export03(DataGridView dgv, string filename, 
            string sheetName, string title, string unit = "", string desc = "")
        {
            IWorkbook workbook = new HSSFWorkbook();
            exportSingleSheet(dgv, sheetName, title, unit, desc, workbook);
            writeToFile(filename, workbook);
        }

        /// <summary>
        /// 导出一个DataTable到指定的2007版本的Excel文件中，单元表名由参数指定。
        /// </summary>
        /// <param name="dt">被导出的表</param>
        /// <param name="filename">Excel文件路径</param>       
        /// <param name="sheetName">sheet表名</param>
        /// <param name="title">Sheet表的标题</param>
        /// <param name="unit">数字单位</param>
        /// <param name="desc">描述信息</param>
        public static void export07(DataTable dt, string filename, 
            string sheetName, string title, string unit = "", string desc = "")
        {
            var sheetTables = convertDGVToDT(new DataTable[] { dt }, new string[] { sheetName},
                new string[] { title}, new string[] { unit }, new string[] { desc });
            writeTo07Excel(sheetTables, filename);
        }

        /// <summary>
        /// 导出一个DataGridView到指定的2007版本的Excel文件中，单元表名由参数指定。
        /// </summary>
        /// <param name="dgv">被导出的表</param>
        /// <param name="filename">Excel文件路径</param>
        /// <param name="sheetName">sheet表名</param>
        /// <param name="title">Sheet表的标题</param>
        /// <param name="unit">数字单位</param>
        /// <param name="desc">描述信息</param>
        public static void export07(DataGridView dgv, string filename, 
            string sheetName, string title, string unit = "", string desc = "")
        {
            writeTo07Excel(convertDGVToDT(new DataGridView[] { dgv }, new string[] { sheetName },
                new string[] { title}, new string[] { unit }, new string[] { desc }), filename);
        }

        private static CustomSheetTable[] convertDGVToDT(DataTable[] dts, string[] sheetNames,
            string[] titles, string[] unitStrings, string[] descs)
        {
            CustomSheetTable[] res = new CustomSheetTable[dts.Length];
            for (int k = 0; k < dts.Length; k++)
            {
                var dgv = dts[k];
                List<string> columnNames = new List<string>();
                for (int i = 0, cols = dgv.Columns.Count; i < cols; i++)
                {
                    columnNames.Add(dgv.Columns[i].ColumnName);
                }

                object[][] datas = new object[dgv.Rows.Count][];
                for (int i = 0, rows = dgv.Rows.Count; i < rows; i++)
                {
                    object[] r = new object[dgv.Columns.Count];
                    for (int j = 0, cols = dgv.Columns.Count; j < cols; j++)
                    {
                        r[j] = dgv.Rows[i][j].ToString();
                    }
                    datas[i] = r;
                }
                CustomSheetTable dt = new CustomSheetTable(datas, sheetNames[k], titles[k],
                    columnNames.ToArray(), unitStrings[k], descs[k]);
                res[k] = dt;
            }
            return res;
        }

        private static CustomSheetTable[] convertDGVToDT(DataGridView[] dgvs,
            string[] sheetNames, string[] titles,
            string[] unitStrings, string[] descs)
        {
            CustomSheetTable[] dts = new CustomSheetTable[dgvs.Length];
            for (int k = 0; k < dgvs.Length; k++)
            {
                var dgv = dgvs[k];
                List<string> columnNames = new List<string>();
                for (int i = 0, cols = dgv.Columns.Count; i < cols; i++)
                {
                    columnNames.Add(dgv.Columns[i].Name);
                }

                object[][] datas = new object[dgv.Rows.Count][];
                for (int i = 0, rows = dgv.Rows.Count; i < rows; i++)
                {
                    object[] r = new object[dgv.Columns.Count];
                    for (int j = 0, cols = dgv.Columns.Count; j < cols; j++)
                    {
                        r[j] = dgv.Rows[i].Cells[j].Value;
                    }
                    datas[i] = r;
                }
                CustomSheetTable dt = new CustomSheetTable(datas, sheetNames[k], titles[k],
                    columnNames.ToArray(),
                    unitStrings[k], descs[k]);
                dts[k] = dt;
            }
            return dts;
        }

        /// <summary>
        /// 将单个DataTable中的数据转换为一个object[][]二维数组。
        /// 该函数的目的是转换DataTable和DataGridView的公共接口。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetName"></param>
        /// <param name="title"></param>
        /// <param name="unit"></param>
        /// <param name="desc"></param>
        /// <param name="workbook"></param>
        private static void exportSingleSheet(DataTable dt, string sheetName,
            string title, string unit, string desc, IWorkbook workbook)
        {
            string[] columnNames = new string[dt.Columns.Count];
            for (int i = 0, e = dt.Columns.Count; i < e; i++)
                columnNames[i] = dt.Columns[i].ColumnName;
            object[][] datas = new object[dt.Rows.Count][];
            for (int i = 0, rows = dt.Rows.Count; i < rows; i++)
            {
                object[] r = new object[dt.Columns.Count];
                for (int j = 0, cols = dt.Columns.Count; j < cols; j++)
                {
                    r[j] = dt.Rows[i][j].ToString();
                }
                datas[i] = r;
            }
            writeOneSheetTable(datas, columnNames, sheetName, title, unit, desc, workbook);
        }

        private static void exportSingleSheet(DataGridView dgv, string sheetName,
            string title, string unit, string desc, IWorkbook workbook)
        {
            string[] columnNames = new string[dgv.Columns.Count];
            for (int i = 0, e = dgv.Columns.Count; i < e; i++)
                columnNames[i] = dgv.Columns[i].Name;
            object[][] datas = new object[dgv.Rows.Count][];
            for (int i = 0, rows = dgv.Rows.Count; i < rows; i++)
            {
                object[] r = new object[dgv.Columns.Count];
                for (int j = 0, cols = dgv.Columns.Count; j < cols; j++)
                {
                    r[j] = dgv.Rows[i].Cells[j].Value.ToString();
                }
                datas[i] = r;
            }
            writeOneSheetTable(datas, columnNames, sheetName, title, unit, desc, workbook);
        }

        private static void writeOneSheetTable(object[][] datas, string[] columnNames,
            string sheetName, string title, string unit,
            string desc, IWorkbook workbook)
        {
            if (datas == null || datas.Length <= 0 || columnNames.Length <= 0)
                return;     // terminates early if there is no any datarows in datatable.

            if (datas[0].Length != columnNames.Length)
            {
                throw new Exception("表格数据datas的列数" + datas[0].Length 
                    + "和列名数组" + columnNames.Length + "的长度不一致");
            }

            // obtains the number of rows and columns.
            int rowNum = datas.Length;
            int columnNum = datas[0].Length;
            int contentOffset = 0;   // record the offset of content row away from the first row.

            // 创建一个sheet表  
            ISheet sheet1 = workbook.CreateSheet(sheetName);

            var titleStyle = createCellStyle(workbook, "Calibri", 14, true, false, 
                NPOI.SS.UserModel.HorizontalAlignment.Center);
            // create a cell for holding the title.
            createCell(sheet1, title, titleStyle, contentOffset, contentOffset, 0, columnNum - 1, true);

            contentOffset++;

            // create a cell for description info.
            if (desc != null && desc.Length > 0)
            {
                var descStyle = createCellStyle(workbook, "Calibri", 12, false, 
                    false, NPOI.SS.UserModel.HorizontalAlignment.Right);
                createCell(sheet1, desc, descStyle, contentOffset, contentOffset, 0, columnNum - 1, true);
                contentOffset++;
            }
            // create a cell for unit string.
            if (unit != null && unit.Length > 0)
            {
                var unitStyle = createCellStyle(workbook, "Calibri", 12, false,
                    false, NPOI.SS.UserModel.HorizontalAlignment.Center);
                createCell(sheet1, unit, unitStyle, contentOffset, contentOffset, 0, columnNum - 1, true);
                contentOffset++;
            }

            // make a header row  
            // adjust the style for the header row.            
            var headerCellStyle = createCellStyle(workbook, "Calibri", 12, false,
                    false, NPOI.SS.UserModel.HorizontalAlignment.Center);

            HSSFPalette palette = ((HSSFWorkbook)workbook).GetCustomPalette();
            var customColorIndex = NPOI.HSSF.Util.HSSFColor.LightBlue.Index;
            palette.SetColorAtIndex(customColorIndex, 146, 205, 220);

            headerCellStyle.FillForegroundColor = customColorIndex;
            headerCellStyle.FillPattern = FillPattern.SolidForeground;

            IRow row1 = sheet1.CreateRow(contentOffset++);
            for (int j = 0; j < columnNum; j++)
            {
                ICell cell = row1.CreateCell(j);
                cell.SetCellValue(columnNames[j]);
                cell.CellStyle = headerCellStyle;
                //int len = Utility.GetStringLength(columnNames[j]);
            }

            // adjust the cell style for data cell.
            var dataCellStyle = createCellStyle(workbook, "Calibri", 11, false,
                    false, NPOI.SS.UserModel.HorizontalAlignment.Right);           
            //loops through data  
            for (int i = 0; i < rowNum; i++)
            {
                IRow row = sheet1.CreateRow(i + contentOffset);
                for (int j = 0; j < columnNum; j++)
                {
                    ICell cell = row.CreateCell(j);
                    cell.CellStyle = dataCellStyle;
                    cell.SetCellValue(datas[i][j].ToString());
                }
            }

            // Enable the auto-resize for each columns contained in the table.
            // Note that: This must be called after populating all data, otherwise
            // there will no effect.
            for (int i = 0; i < columnNum; i++)
            {
                sheet1.AutoSizeColumn(i);
            }
        }

        private static ICellStyle createCellStyle(IWorkbook workbook, string fontName, 
            short fontSizeInPoints, bool isBold, bool isItalic, 
            NPOI.SS.UserModel.HorizontalAlignment horizAlign)
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = fontSizeInPoints;
            font.FontName = fontName;            
            font.IsBold = isBold;
            font.IsItalic = isItalic;
            ICellStyle style = workbook.CreateCellStyle();
            style.SetFont(font);
            style.Alignment = horizAlign;
            return style;
        }

        /// <summary>
        /// 创建一个单元格，并使用参数对其进行初始化。
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="content"></param>
        /// <param name="style"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        /// <param name="startCol"></param>
        /// <param name="endCol"></param>
        /// <param name="isMerged"></param>
        private static void createCell(ISheet sheet, string content,
            ICellStyle style, int startRow, int endRow,
            int startCol, int endCol, bool isMerged = false)
        {
            // add the row for sheet name.
            IRow titleRow = sheet.CreateRow(startRow);
            ICell titleCell = titleRow.CreateCell(startCol);

            titleCell.SetCellValue(content);

            // update the style.
            titleCell.CellStyle = style;

            // merge all of cells in first row.
            if (isMerged)
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, endRow, startCol, endCol));
        }

        /// <summary>
        /// 将workbook中的数据写入到由filename指定的文件中。
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="workbook"></param>
        private static void writeToFile(string filename, IWorkbook workbook)
        {
            FileStream os = null;
            try
            {
                os = new FileStream(filename, FileMode.Create);
                workbook.Write(os);
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入Excel文件失败:" + ex.Message);
            }
            finally
            {
                if (os != null)
                    os.Close();
            }
        }

        /// <summary>
        /// 导出DataSet到Excel文件中的内部函数
        /// </summary>
        /// <param name="dts">DataTables</param>
        /// <param name="filePath">Excel文件路径</param>
        private static void writeTo07Excel(CustomSheetTable[] dts, string filePath)
        {
            //populate the data into the spreadsheet  
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.
                Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                if (dts != null && dts.Length > 0)
                {
                    //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
                    //  to a file, or writing to a MemoryStream.
                    spreadsheet.AddWorkbookPart();
                    spreadsheet.WorkbookPart.Workbook = new Workbook();

                    FileVersion fileVersion1 = new FileVersion() { ApplicationName = "xl", LastEdited = "4", LowestEdited = "4", BuildVersion = "4506" };
                    spreadsheet.WorkbookPart.Workbook.Append(fileVersion1);

                    //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
                    spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

                    //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
                    WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
                    Stylesheet styles = new CustomStylesheet();
                    workbookStylesPart.Stylesheet = styles;

                    //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
                    uint worksheetNumber = 1;
                    for (int i = 0, sz = dts.Length; i < sz; i++)
                    {
                        CustomSheetTable dt = dts[i];
                        string unitString = dt.unitString == null ? "" : dt.unitString;
                        string desc = dt.desc == null ? "" : dt.desc;

                        //  For each worksheet you want to create
                        string workSheetID = "rId" + worksheetNumber.ToString();
                        string worksheetName = dt.tableName;

                        WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                        newWorksheetPart.Worksheet = new Worksheet();

                        // for merge the first three rows in sheetData.
                        MergeCells mergeCells;

                        // create sheet data
                        SheetData sheetData =
                            CreateSheetData(dt, workbookStylesPart, out mergeCells);

                        //auto resize the width of column in sheetData.
                        Columns columns = AutoSize(sheetData);
                        newWorksheetPart.Worksheet.Append(columns);

                        newWorksheetPart.Worksheet.AppendChild(sheetData);

                        // merges the first three rows in the sheetData.
                        newWorksheetPart.Worksheet.InsertAfter(mergeCells,
                            newWorksheetPart.Worksheet.Elements<SheetData>().First());

                        newWorksheetPart.Worksheet.Save();

                        // create the worksheet to workbook relation
                        if (worksheetNumber == 1)
                            spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                        spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
                        {
                            Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                            SheetId = (uint)worksheetNumber,
                            Name = dt.tableName
                        });

                        worksheetNumber++;
                    }

                    spreadsheet.WorkbookPart.Workbook.Save();
                }
            }
        }

        /// <summary>
        /// 自动设置WorkSheet表的列宽，返回一个列集。
        /// </summary>
        /// <param name="sheetData"></param>
        /// <returns></returns>
        private static Columns AutoSize(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            // reduce the column width of the first column by half.
           /// maxColWidth[0] /= 3;

            Columns columns = new Columns();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {             
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)charWidth*1.1 };
                columns.Append(col);
            }

            return columns;
        }

        /// <summary>
        /// 获取每列相应的最大列宽，返回一个列序号与列宽之间的映射集。
        /// </summary>
        /// <param name="sheetData"></param>
        /// <returns></returns>
        private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            UInt32[] numberStyles = new UInt32[] { 5, 6, 7, 8 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 1, 2, 3, 4, 6, 7, 8 }; //styles that will bold
            int j= 0;
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                   
                    var cell = cells[i];
                    var cellValue = cell.InnerText == null ? string.Empty : cell.InnerText;
                    var cellTextLength = Utility.GetStringLength(cellValue);
                    
                    if (cell.StyleIndex != null)
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);

                        //add an extra char for bold - not 100% acurate but good enough for what i need.
                        cellTextLength += 1;
                    }
                    var st = cells[i].InnerText.ToString();

                    if (j == 0&&i==0)
                    {

                        continue;
                     }
                    if (j == 1 && i == 0)
                    {
                        continue;
                    }
                    if (j == 2 && i == 0)
                    {
                        continue;
                    }
                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength;
                        }
                    }
                    else
                    {
                        maxColWidth[i] = cellTextLength;
                    }
                }
                j++;
            }

            return maxColWidth;
        }

        private static SheetData CreateSheetData(CustomSheetTable table,
            WorkbookStylesPart stylesPart, out MergeCells mergeCells)
        {
            SheetData sheetData = new SheetData();
            string[] headerNames = table.columnNames;
            string unitString = table.unitString;
            string desc = table.desc;

            object[][] datas = table.data;

            // create a mergeCells for holding each MergeCell.
            mergeCells = new MergeCells();
            
            if (datas != null && datas.Length > 0)
            {

                int numRows = datas.Length;
                int numCols = datas[0].Length;

                //行索引
                int index = 1;

                Row titleRow = new Row();
                titleRow.RowIndex = (uint)index;
                #region 生成sheet标题
                // the name of column index, like "A", "B", etc...
                string colIndexName = getColumnName(1);
                System.Drawing.Color fillColor = System.Drawing.Color.White;

                HeaderCell cell = new TitleCell(colIndexName, table.title, index, 
                    stylesPart.Stylesheet, fillColor, 14, true, 
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center});
                new CellFormat();
                titleRow.AppendChild(cell);
                sheetData.AppendChild(titleRow);
                
                // merge all of cells in the first row.
                mergeCells.Append(new MergeCell()
                {
                    Reference = 
                    new StringValue(colIndexName+index+":"+ getColumnName(numCols)+index)
                });
                index++;
                #endregion

                #region 生成数字单位说明行
                if (unitString != null && unitString.Length > 0)
                {
                    Row unitStringRow = new Row();
                    unitStringRow.RowIndex = (uint)index;
                    cell = new TitleCell(colIndexName, unitString, index,
                        stylesPart.Stylesheet, fillColor, 12, false, 
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center });
                    unitStringRow.AppendChild(cell);
                    sheetData.AppendChild(unitStringRow);

                    // merge all of cells in the second row.
                    mergeCells.Append(new MergeCell()
                    {
                        Reference =
                        new StringValue(colIndexName + index + ":" + getColumnName(numCols) + index)
                    });
                    index++;
                }
                #endregion

                #region 生成描述信息行
                if (desc != null && desc.Length > 0)
                {
                    Row descRow = new Row();
                    descRow.RowIndex = (uint)index;
                    cell = new TitleCell(colIndexName, desc, index,
                        stylesPart.Stylesheet, fillColor, 12, false, 
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Right });
                    descRow.AppendChild(cell);
                    sheetData.AppendChild(descRow);
                    // merge all of cells in the thrid row.
                    mergeCells.Append(new MergeCell()
                    {
                        Reference =
                        new StringValue(colIndexName + index + ":" + getColumnName(numCols) + index)
                    });
                    index++;
                }
                #endregion

                #region 生成列标题栏
                Row header = new Row();
                header.RowIndex = (uint)index;
                //生成工作表中的列标题栏
                for (int col = 0; col < numCols; col++)
                {
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(0xff, 0x92, 0xcd, 0xdc);

                    HeaderCell c = new HeaderCell(getColumnName(col + 1),
                        headerNames[col], index, stylesPart.Stylesheet,
                       color, 12, true);

                    header.AppendChild(c);
                }
                sheetData.AppendChild(header);
                #endregion

                #region 生成数据
                for (int i = 0; i < numRows; i++)
                {
                    index++;
                    object[] dr = datas[i];
                    continue;
                    var r = new Row { RowIndex = (uint)index };

                    for (int col = 0; col < numCols; col++)
                    {
                        var obj = dr[col];
                        if (obj != null)
                        {
                            if (obj.GetType() == typeof(string))
                            {

                                TextCell c = new TextCell(getColumnName(col + 1), obj.ToString(), index);
                                r.AppendChild(c);

                            }
                            else if (obj.GetType() == typeof(bool))
                            {
                                string value = (bool)obj ? "是" : "否";
                                TextCell c = new TextCell(getColumnName(col + 1), value, index);
                                r.AppendChild(c);
                            }
                            else if (obj.GetType() == typeof(DateTime))
                            {
                                string value = ((DateTime)obj).ToString();

                                // stylesPart.Stylesheet is retrieved reference for the appropriate worksheet.
                                //使用DateCell类型单元格生成的Excel文档打开时，会出现“发现不可读取的内容”
                                //DateCell c = new DateCell(getColumnName(col + 1), (DateTime)obj, index);
                                TextCell c = new TextCell(getColumnName(col + 1), value, index);
                                r.AppendChild(c);
                            }
                            else if (obj.GetType() == typeof(decimal) || obj.GetType() == typeof(double))
                            {
                                //FormatedNumberCell c = new FormatedNumberCell(getColumnName(col + 1), obj.ToString(), index);
                                //r.AppendChild(c);
                                string str = obj.ToString();
                                if (str.Length > 6) str = str.Substring(0, 6);
                                TextCell c = new TextCell(getColumnName(col + 1), str, index);
                                r.AppendChild(c);
                            }
                            else
                            {
                                long value;
                                if (long.TryParse(obj.ToString(), out value))
                                {
                                    NumberCell c = new NumberCell(getColumnName(col + 1), obj.ToString(), index);
                                    r.AppendChild(c);
                                }
                                else
                                {
                                    TextCell c = new TextCell(getColumnName(col + 1), obj.ToString(), index);
                                    r.AppendChild(c);
                                }
                            }
                        }
                    }
                    sheetData.AppendChild(r);
                }

                #endregion
            }
            return sheetData;
        }

        /// <summary>
        /// Convert a zero-based column index into an Excel column reference  
        /// (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private static string getColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    System.Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }

        private static SheetData CreateSheetData<T>
            (List<T> objects, List<string> headerNames, WorkbookStylesPart stylesPart)
        {
            SheetData sheetData = new SheetData();

            if (objects != null)
            {
                //Fields sheetNames of object
                List<string> fields = GetPropertyInfo<T>();

                var az = new List<Char>(Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (Char)i).ToArray());
                List<Char> headers = az.GetRange(0, fields.Count);

                int numRows = objects.Count;
                int numCols = fields.Count;
                Row header = new Row();
                int index = 1;
                header.RowIndex = (uint)index;
                for (int col = 0; col < numCols; col++)
                {
                    //Cell c = CreateHeaderCell(headers[col].ToString(), headerNames[col], index, stylesPart.Stylesheet);
                    HeaderCell c = new HeaderCell(headers[col].ToString(), headerNames[col], index, stylesPart.Stylesheet, System.Drawing.Color.DodgerBlue, 12, true);
                    header.Append(c);
                }
                sheetData.Append(header);

                for (int i = 0; i < numRows; i++)
                {
                    index++;
                    var obj1 = objects[i];
                    var r = new Row { RowIndex = (uint)index };

                    for (int col = 0; col < numCols; col++)
                    {
                        string fieldName = fields[col];
                        PropertyInfo myf = obj1.GetType().GetProperty(fieldName);
                        if (myf != null)
                        {
                            object obj = myf.GetValue(obj1, null);
                            if (obj != null)
                            {
                                if (obj.GetType() == typeof(string))
                                {
                                    // Cell c = CreateTextCell(headers[col].ToString(), obj.ToString(), index);
                                    TextCell c = new TextCell(headers[col].ToString(), obj.ToString(), index);
                                    r.Append(c);
                                }
                                else if (obj.GetType() == typeof(bool))
                                {
                                    string value = (bool)obj ? "Yes" : "No";
                                    //Cell c = CreateTextCell(headers[col].ToString(), value, index);
                                    TextCell c = new TextCell(headers[col].ToString(), value, index);
                                    r.Append(c);
                                }
                                else if (obj.GetType() == typeof(DateTime))
                                {
                                    //string value = GetExcelSerialDate((DateTime) obj).ToString();
                                    string value = ((DateTime)obj).ToOADate().ToString();

                                    // stylesPart.Stylesheet is retrieved reference for the appropriate worksheet.
                                    //Cell c = CreateDateCell(headers[col].ToString(), value, index, stylesPart.Stylesheet);
                                    DateCell c = new DateCell(headers[col].ToString(), (DateTime)obj, index);
                                    r.Append(c);
                                }
                                else if (obj.GetType() == typeof(decimal) || obj.GetType() == typeof(double))
                                {
                                    //Cell c = CreateDecimalCell(headers[col].ToString(), obj.ToString(), index, stylesPart.Stylesheet);
                                    FormatedNumberCell c = new FormatedNumberCell(headers[col].ToString(), obj.ToString(), index);
                                    r.Append(c);
                                }
                                else
                                {
                                    long value;
                                    if (long.TryParse(obj.ToString(), out value))
                                    {
                                        //Cell c = CreateIntegerCell(headers[col].ToString(), obj.ToString(), index);
                                        NumberCell c = new NumberCell(headers[col].ToString(), obj.ToString(), index);
                                        r.Append(c);
                                    }
                                    else
                                    {
                                        //Cell c = CreateTextCell(headers[col].ToString(), obj.ToString(), index);
                                        TextCell c = new TextCell(headers[col].ToString(), obj.ToString(), index);

                                        r.Append(c);
                                    }
                                }
                            }
                        }
                    }

                    sheetData.Append(r);

                }

                index++;
                Row total = new Row();
                total.RowIndex = (uint)index;
                for (int col = 0; col < numCols; col++)
                {
                    var obj1 = objects[0];
                    string fieldName = fields[col];
                    PropertyInfo myf = obj1.GetType().GetProperty(fieldName);
                    if (myf != null)
                    {
                        object obj = myf.GetValue(obj1, null);
                        if (obj != null)
                        {

                            if (col == 0)
                            {
                                //c = CreateTextCell(headers[col].ToString(), "Total", index);
                                TextCell c = new TextCell(headers[col].ToString(), "Total", index);
                                c.StyleIndex = 10;
                                total.Append(c);
                            }
                            else if (obj.GetType() == typeof(decimal) || obj.GetType() == typeof(double))
                            {
                                string headerCol = headers[col].ToString();
                                string firstRow = headerCol + "2";
                                string lastRow = headerCol + (numRows + 1);
                                string formula = "=SUM(" + firstRow + " : " + lastRow + ")";
                                Console.WriteLine(formula);

                                //c = CreateFomulaCell(headers[col].ToString(), formula, index, stylesPart.Stylesheet);
                                FomulaCell c = new FomulaCell(headers[col].ToString(), formula, index);
                                c.StyleIndex = 9;
                                total.Append(c);
                            }
                            else
                            {
                                TextCell c = new TextCell(headers[col].ToString(), string.Empty, index);
                                c.StyleIndex = 10;
                                total.Append(c);
                            }


                        }
                    }
                }
                sheetData.Append(total);
            }

            return sheetData;
        }

        private static Cell CreateIntegerCell(string header, string text, int index)
        {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;

            DocumentFormat.OpenXml.Spreadsheet.CellValue v = new DocumentFormat.OpenXml.Spreadsheet.CellValue();
            v.Text = text;
            c.AppendChild(v);
            return c;
        }

        private static Cell CreateDecimalCell(string header, string text, int index, Stylesheet styles)
        {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;
            UInt32Value fontId = CreateFont(styles, "Arial", 11, false, System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles, System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles, fontId, fillId, 171);
            c.StyleIndex = formatId;

            DocumentFormat.OpenXml.Spreadsheet.CellValue v = new DocumentFormat.OpenXml.Spreadsheet.CellValue();
            v.Text = text;
            c.AppendChild(v);
            return c;
        }

        private static Cell CreateFomulaCell(string header, string formula, int index, Stylesheet styles)
        {
            Cell c = new Cell();
            c.DataType = CellValues.Number;
            c.CellReference = header + index;
            UInt32Value fontId = CreateFont(styles, "Arial", 11, false, System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles, System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles, fontId, fillId, 171);
            c.StyleIndex = formatId;

            CellFormula f = new CellFormula();
            f.CalculateCell = true;
            f.Text = formula;
            c.Append(f);

            DocumentFormat.OpenXml.Spreadsheet.CellValue v = new DocumentFormat.OpenXml.Spreadsheet.CellValue();
            c.AppendChild(v);
            return c;
        }


        private static Cell CreateDateCell(string header, string text, int index, Stylesheet styles)
        {
            Cell c = new Cell();
            c.DataType = CellValues.Date;
            c.CellReference = header + index;

            UInt32Value fontId = CreateFont(styles, "Arial", 11, false, System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles, System.Drawing.Color.White);
            UInt32Value formatId = CreateCellFormat(styles, fontId, fillId, 14);
            c.StyleIndex = formatId;

            DocumentFormat.OpenXml.Spreadsheet.CellValue v = new DocumentFormat.OpenXml.Spreadsheet.CellValue();
            v.Text = text;
            c.CellValue = v;


            return c;
        }

        private static Cell CreateTextCell(string header, string text, int index)
        {

            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = header + index;

            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            return c;
        }

        private static Cell CreateHeaderCell(string header, string text, int index, Stylesheet styles)
        {
            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = header + index;
            Console.WriteLine(header + index);

            UInt32Value fontId = CreateFont(styles, "Arial", 12, true, System.Drawing.Color.Black);
            UInt32Value fillId = CreateFill(styles, System.Drawing.Color.ForestGreen);
            UInt32Value formatId = CreateCellFormat(styles, fontId, fillId, 0);
            c.StyleIndex = formatId;

            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            return c;
        }
        private static List<string> GetPropertyInfo<T>()
        {

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            // write property sheetNames
            return propertyInfos.Select(propertyInfo => propertyInfo.Name).ToList();
        }

        private static Stylesheet CreateStylesheet()
        {
            var ss = new Stylesheet();

            var fts = new Fonts();
            var ftn = new FontName { Val = "Arial" };
            var ftsz = new FontSize { Val = 11 };
            var ft = new DocumentFormat.OpenXml.Spreadsheet.Font { FontName = ftn, FontSize = ftsz };
            fts.Append(ft);
            fts.Count = (uint)fts.ChildElements.Count;


            var fills = new Fills();
            var fill = new Fill();
            var patternFill = new PatternFill { PatternType = PatternValues.None };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fill = new Fill();
            patternFill = new PatternFill { PatternType = PatternValues.Gray125 };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            fills.Count = (uint)fills.ChildElements.Count;

            var borders = new Borders();
            var border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);
            borders.Count = (uint)borders.ChildElements.Count;

            var csfs = new CellStyleFormats();
            var cf = new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0 };
            csfs.Append(cf);
            csfs.Count = (uint)csfs.ChildElements.Count;

            // dd/mm/yyyy is also Excel style index 14

            uint iExcelIndex = 164;
            var nfs = new NumberingFormats();
            var cfs = new CellFormats();

            cf = new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 };
            cfs.Append(cf);

            var nf = new NumberingFormat { NumberFormatId = iExcelIndex, FormatCode = "dd/mm/yyyy hh:mm:ss" };
            nfs.Append(nf);

            cf = new CellFormat
            {
                NumberFormatId = nf.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = true
            };
            cfs.Append(cf);


            iExcelIndex = 165;
            nfs = new NumberingFormats();
            cfs = new CellFormats();

            cf = new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 };
            cfs.Append(cf);

            nf = new NumberingFormat { NumberFormatId = iExcelIndex, FormatCode = "MMM yyyy" };
            nfs.Append(nf);

            cf = new CellFormat
            {
                NumberFormatId = nf.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = true
            };
            cfs.Append(cf);


            iExcelIndex = 170;
            nf = new NumberingFormat { NumberFormatId = iExcelIndex, FormatCode = "#,##0.0000" };
            nfs.Append(nf);
            cf = new CellFormat
            {
                NumberFormatId = nf.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = true
            };
            cfs.Append(cf);

            // #,##0.00 is also Excel style index 4
            iExcelIndex = 171;
            nf = new NumberingFormat { NumberFormatId = iExcelIndex, FormatCode = "#,##0.00" };
            nfs.Append(nf);
            cf = new CellFormat
            {
                NumberFormatId = nf.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = true
            };
            cfs.Append(cf);

            // @ is also Excel style index 49
            iExcelIndex = 172;
            nf = new NumberingFormat { NumberFormatId = iExcelIndex, FormatCode = "@" };
            nfs.Append(nf);
            cf = new CellFormat
            {
                NumberFormatId = nf.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = true
            };
            cfs.Append(cf);

            nfs.Count = (uint)nfs.ChildElements.Count;
            cfs.Count = (uint)cfs.ChildElements.Count;

            ss.Append(nfs);
            ss.Append(fts);
            ss.Append(fills);
            ss.Append(borders);
            ss.Append(csfs);
            ss.Append(cfs);

            var css = new CellStyles();
            var cs = new CellStyle { Name = "Normal", FormatId = 0, BuiltinId = 0 };
            css.Append(cs);
            css.Count = (uint)css.ChildElements.Count;
            ss.Append(css);

            var dfs = new DifferentialFormats { Count = 0 };
            ss.Append(dfs);

            var tss = new TableStyles
            {
                Count = 0,
                DefaultTableStyle = "TableStyleMedium9",
                DefaultPivotStyle = "PivotStyleLight16"
            };
            ss.Append(tss);

            return ss;
        }

        private static UInt32Value CreateCellFormat(
            Stylesheet styleSheet,
            UInt32Value fontIndex,
            UInt32Value fillIndex,
            UInt32Value numberFormatId)
        {
            CellFormat cellFormat = new CellFormat();

            if (fontIndex != null)
                cellFormat.FontId = fontIndex;

            if (fillIndex != null)
                cellFormat.FillId = fillIndex;

            if (numberFormatId != null)
            {
                cellFormat.NumberFormatId = numberFormatId;
                cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
            }

            styleSheet.CellFormats.Append(cellFormat);

            UInt32Value result = styleSheet.CellFormats.Count;
            styleSheet.CellFormats.Count++;
            return result;
        }

        private static UInt32Value CreateFill(
            Stylesheet styleSheet,
            System.Drawing.Color fillColor)
        {


            PatternFill patternFill =
                new PatternFill(
                    new ForegroundColor()
                    {
                        Rgb = new HexBinaryValue()
                        {
                            Value =
                            System.Drawing.ColorTranslator.ToHtml(
                                System.Drawing.Color.FromArgb(
                                    fillColor.A,
                                    fillColor.R,
                                    fillColor.G,
                                    fillColor.B)).Replace("#", "")
                        }
                    });

            patternFill.PatternType = fillColor ==
                        System.Drawing.Color.White ? PatternValues.None : PatternValues.LightDown;

            Fill fill = new Fill(patternFill);

            styleSheet.Fills.Append(fill);

            UInt32Value result = styleSheet.Fills.Count;
            styleSheet.Fills.Count++;
            return result;
        }

        private static UInt32Value CreateFont(
            Stylesheet styleSheet,
            string fontName,
            double? fontSize,
            bool isBold,
            System.Drawing.Color foreColor)
        {

            Font font = new Font();

            if (!string.IsNullOrEmpty(fontName))
            {
                FontName name = new FontName()
                {
                    Val = fontName
                };
                font.Append(name);
            }

            if (fontSize.HasValue)
            {
                FontSize size = new FontSize()
                {
                    Val = fontSize.Value
                };
                font.Append(size);
            }

            if (isBold == true)
            {
                Bold bold = new Bold();
                font.Append(bold);
            }


            Color color = new Color()
            {
                Rgb = new HexBinaryValue()
                {
                    Value =
                        System.Drawing.ColorTranslator.ToHtml(
                            System.Drawing.Color.FromArgb(
                                foreColor.A,
                                foreColor.R,
                                foreColor.G,
                                foreColor.B)).Replace("#", "")
                }
            };
            font.Append(color);

            styleSheet.Fonts.Append(font);
            UInt32Value result = styleSheet.Fonts.Count;
            styleSheet.Fonts.Count++;
            return result;
        }

        private static Column CreateColumnData(UInt32 startColumnIndex, UInt32 endColumnIndex, double columnWidth)
        {
            Column column;
            column = new Column();
            column.Min = startColumnIndex;
            column.Max = endColumnIndex;
            column.Width = columnWidth;
            column.CustomWidth = true;
            return column;
        }

        private static int GetExcelSerialDate(DateTime input)
        {
            int nDay = input.Day;
            int nMonth = input.Month;
            int nYear = input.Year;
            // Excel/Lotus 123 have a bug with 29-02-1900. 1900 is not a
            // leap year, but Excel/Lotus 123 think it is...
            if (nDay == 29 && nMonth == 02 && nYear == 1900)
                return 60;

            // DMY to Modified Julian calculatie with an extra substraction of 2415019.
            long nSerialDate =
                    (int)((1461 * (nYear + 4800 + (int)((nMonth - 14) / 12))) / 4) +
                    (int)((367 * (nMonth - 2 - 12 * ((nMonth - 14) / 12))) / 12) -
                    (int)((3 * ((int)((nYear + 4900 + (int)((nMonth - 14) / 12)) / 100))) / 4) +
                    nDay - 2415019 - 32075;

            if (nSerialDate < 60)
            {
                // Because of the 29-02-1900 bug, any serial date 
                // under 60 is one off... Compensate.
                nSerialDate--;
            }

            return (int)nSerialDate;

        }
    }

    /// <summary>
    /// 实用工具类
    /// </summary>
    class Utility
    {
        ///<summary>
        ///获取字符串的长度，这不是指字符串中字符的个数 
        /// 这里是要获取字符串要占位的单元数，英文字符占一位，中文字符占两位 
        /// </summary>
        public static int GetStringLength(string input)
        {
            //中文字符 的 范围
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);

            int count = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int code = Char.ConvertToUtf32(input, i);
                if (code >= chfrom && code <= chend)
                    count += 2;
                else
                    count += 1;
            }
            return count;
        }

        /// <summary>
        /// 提取输入字符串中的中文字符。
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static string GetChineseCharacter(string input)
        {
            StringBuilder sb = new StringBuilder();
            //中文字符 的 范围
            //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chfrom = Convert.ToInt32("4e00", 16);
            int chend = Convert.ToInt32("9fff", 16);
            for (int i = 0; i < input.Length; i++)
            {
                int code = Char.ConvertToUtf32(input, i);
                if (code >= chfrom && code <= chend)
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }      
    }

    /// <summary>
    /// 该类用于保存从DataTable和DataGridView中抽取出来的数据，使得ExportTo07Excel函数不依赖与
    /// 具体的DataTable或者DataGridView。
    /// </summary>
    public class CustomSheetTable
    {
        public object[][] data;
        public string tableName;
        public string title;
        public string[] columnNames;
        public string unitString;
        public string desc;

        public CustomSheetTable(object[][] Data, string TableName,
            string Title,
            string[] ColumnNames,
            string UnitString, string Desc)
        {
            data = Data;
            tableName = TableName;
            title = Title;
            columnNames = ColumnNames;
            unitString = UnitString;
            desc = Desc;
        }
    }
}
