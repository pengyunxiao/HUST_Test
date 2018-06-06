using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VBprinter40;                  //添加by孙凯 2015.4.23
using System.Threading;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using LPSP_MergeDGV;

/*
 *   2013-10-30 将本文件的所有DataGridViewX  改为标准控件  DataGridView   ----BY Liu----
 */


namespace HUST_OutPut
{
    public partial class TableView : DevComponents.DotNetBar.Office2007Form
    {
        public Form parentForm;
        private progress myprogress;
        DataGridView dgv = null;
        public void progressB()
        {
            this.myprogress = new progress();
            myprogress.Start();
            myprogress.ShowDialog();
        }
        public TableView()
        {
            InitializeComponent();
        }
        public void newTab(System.Data.DataTable dd,List<MergeLink> listMerge=null)
        {
            string[] arrStr = dd.TableName.Split('*');
            string[] tableName=arrStr[0].Split(' ');//将title分割来获取表名
            TabItem tp = this.tabControl1.CreateTab(tableName[0]);
            for (int i = 0; i < arrStr.Length-1; i++)
                //if(arrStr[i].Trim()!="")
                tp.Tooltip += arrStr[i].Trim() + "\n";
            TabControlPanel tcp = new TabControlPanel();
            tcp.Visible = false;
            tcp.TabItem = tp;
            tcp.Dock = DockStyle.Fill;
            this.tabControl1.Controls.Add(tcp);
            tp.AttachedControl = tcp;

            //DataGridView dgv=null;
            if (listMerge != null)//是合并表格
                dgv = new LPSP_MergeDGV.MergeDataGridView();
            else 
                dgv = new DataGridView();             //修改by孙凯 2015.4.23 原句为dgv = new DataGridView(); 
            this.listMerge = listMerge;//保存合并序列

            dgvSet(tcp, dd, arrStr);
        }
        /*
         * 将原newTab函数中负责DataGridView的配置工作部分单独写为函数 编写by孙凯 2015.4.26
         */
        void dgvSet(TabControlPanel tcp, System.Data.DataTable dd, string[] arrStr)
        {
            tcp.Controls.Add(dgv);
            dgv.AllowUserToAddRows = false;
            dgv.CellMouseClick += new DataGridViewCellMouseEventHandler(dgv_CellMouseClick);

            //表中只要装机容量为0就不显示出来   ---2014.07.23  By GAO Yang
            if (dd.Columns.Contains("装机容量"))
            {
                //dd.Columns["装机容量"].DataType = typeof(Int32);
                DataTable dt = dd.Clone();
                dt.Columns["装机容量"].DataType = typeof(Double);
                foreach (DataRow row in dd.Rows)
                {
                    DataRow nr = dt.NewRow();
                    for (int i = 0; i < dd.Columns.Count; i++)
                        nr[i] = row[i];
                    dt.Rows.Add(nr);
                }
                DataView dv = new DataView();
                dv.Table = dt;
                dv.RowFilter = "装机容量 > 0";
                dd = dv.ToTable();
            }



            dgv.DataSource = dd;
            //MessageBox.Show(dd.TableName);

            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            foreach (string subStr in arrStr)
                dgv.Name += subStr;


            this.tabControl1.Refresh();
            dgv.AllowUserToResizeRows = false;
            dgv.ContextMenuStrip = contextMenuStrip1;

            int n = dgv.Columns.Count;
            int m = dgv.Rows.Count;
            dgv.ColumnHeadersHeight = Convert.ToInt32(dgv.Font.Size * 2.1) + 4;

            //禁用列排序
            DisableDataColumnsSort(dgv);

            for (int i = 0; i < m; ++i)
            {
                dgv.Rows[i].Height = Convert.ToInt32(dgv.Font.Size * 2.1);
            }

            if (dgv.Columns.Contains("名  称"))
            {
                dgv.Columns.Remove("名  称");
                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.Name = "名  称";
                col.DataPropertyName = "名  称";
                dgv.Columns.Insert(0, col);
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            #region 修改程序，让表等宽显示 by 孙凯
            // 第一列不需要设为最大宽度
            int k = this.GetStringLength(dgv.Columns[0].HeaderText);
            for (int g = 0; g < m - 1; g++)
            {
                if (this.GetStringLength(Convert.ToString(dd.Rows[g][0]).Trim()) > k)
                    k = this.GetStringLength(Convert.ToString(dd.Rows[g][0]).Trim());

            }

            if (k < 8) k = 8;
            dgv.Columns[0].Width = Convert.ToInt32(k * Convert.ToInt32(dgv.Font.Size));
            Int32 maxWide = getMaxWideth(n, m, dd);
            for (int i = 1; i < n; ++i)
            {
                dgv.Columns[i].Width = Convert.ToInt32(maxWide * Convert.ToInt32(dgv.Font.Size));
            }
            #endregion

            //文本居中显示  添加by孙凯
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // //第一列左对齐
            dgv.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            for (int i = 1; i < n; ++i)
            {
                dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        /***********************************************     
        * 函数功能: 获取此表中最大的宽度   
        * 函数说明：用于让表等宽显示
        * 返 回 值: System.Int32     
        * 参    数: Int32 columns     列数
        * 参    数: Int32 rows        行数
        * 作 成 者：孙凯     
        * 作成日期：2015/01/31    
        ************************************************/
        Int32 getMaxWideth(Int32 columns, Int32 rows, DataTable dd)
        {
            int max = 0;
            for (int col = 2; col < columns; ++col)
            {
                int k = this.GetStringLength(dgv.Columns[col].HeaderText);
                for (int row = 0; row < rows-1; ++row)
                {
                    if (this.GetStringLength(Convert.ToString(dd.Rows[row][col]).Trim()) > k)
                        k = this.GetStringLength(Convert.ToString(dd.Rows[row][col]).Trim());
                }
                if (k > max)
                    max = k;
            }
            return max;
        }
        //在表格显示时增加的单击某行的第一个单元格或者某列的第一个单元格 就默认把整行选中 ---2014.07.18 by GAOYang
        void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                this.dgv.ClearSelection();
                if (e.RowIndex == -1 )//&& e.ColumnIndex != 0)
                {
                    for (int i = 0; i < dgv.Rows.Count; i++)
                        this.dgv.Rows[i].Cells[e.ColumnIndex].Selected = true;
                    //this.dgv.Columns[e.ColumnIndex].Selected = true;
                }

                else if (e.RowIndex != -1  && e.ColumnIndex == 0)
                {
                    for(int i = 0;i<dgv.Columns.Count;i++)
                        this.dgv.Rows[e.RowIndex].Cells[i].Selected = true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void newTab(System.Data.DataTable[] d)
        {
            foreach (System.Data.DataTable dd in d)
            {
                newTab(dd);
            }
        }
        /*
         * MAP表采用多表头控件故添加此函数 编写by孙凯 2015.4.26
         */
        public void newMAPTab(System.Data.DataTable[] d)
        {
            foreach (System.Data.DataTable dd in d)
            {
                newMAPTab(dd);
            }
        }
        /*
         * 设置MAP表多表头显示内容
         */
        private void initMAPMulHeaderDataGridView()
        {
            MulHeaderDataGridView mhdg = dgv as MulHeaderDataGridView;

            TreeView myTreee = new TreeView();
            TreeNode node = new TreeNode("电站工作位置 [上界，下界]");
            node.Nodes.Add("最大负荷");
            node.Nodes.Add("最小负荷");
            node.Nodes.Add(" ");
            node.Nodes.Add(" ");            
            node.Nodes.Add(" ");
            node.Nodes.Add(" ");

            myTreee.Nodes.Add("电站名称");
            myTreee.Nodes.Add(node);

            mhdg.ColHeaderTreeView = myTreee;
            mhdg.ColumnHeadersHeight = 50;
        }
        public void newMAPTab(System.Data.DataTable dd)
        {
            string[] arrStr = dd.TableName.Split(new string[] { "^" }, StringSplitOptions.None);

            TabItem tp = this.tabControl1.CreateTab(arrStr[0]);//+ arrStr[2] + arrStr[3] + arrStr[4] + arrStr[5]);
            for (int i = 0; i < arrStr.Length; i++)
                //if(arrStr[i].Trim()!="")
                tp.Tooltip += arrStr[i].Trim() + "\n";
            TabControlPanel tcp = new TabControlPanel();
            tcp.Visible = false;
            tcp.TabItem = tp;
            tcp.Dock = DockStyle.Fill;
            this.tabControl1.Controls.Add(tcp);
            tp.AttachedControl = tcp;

            dgv = new MulHeaderDataGridView();            
            dgvSet(tcp, dd, arrStr);
            initMAPMulHeaderDataGridView();
        }
        //获取字符串的长度，这不是指字符串中字符的个数
        //这里是要获取字符串要占位的单元数，英文字符占一位，中文字符占两位
        private int GetStringLength(string input)
        {
            //中文字符 的 范围
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend =  Convert.ToInt32("9fff", 16);

            double count = 0;
            for (int i=0;i<input.Length;i++)
            {
                int code=Char.ConvertToUtf32(input, i);
                if (code >= chfrom && code <= chend)
                    count += 1.65;                              //修改by孙凯 调整显示字符宽度 原来为count += 2;  
                else
                    count += 0.7;                              //修改by孙凯 调整显示字符宽度 原来为count += 1;  
            }
            return (int)count+1;                             //修改by孙凯 调整显示字符宽度 原来为return count count为int  
        }

        //根据合并序列，合并单元格
        public void Merge(DataGridView dgv,List<MergeLink> list)
        {
            if (dgv == null || list == null)
                return;
            //对每一个合并序列 设置合并
            foreach (MergeLink link in list)
            {
                dgv.ClearSelection();//初始化取消所有选中
                //选中单元格合并
                if (link.count >1)
                {//合并序列 单元格数 大于1
                    for (int i = 0; i < link.count; i++)
                    {
                        if (link.mergeDir == LPSP_MergeDGV.MergeDirector.Row)
                            dgv[link.columnIndex + i + 1, link.rowIndex].Selected = true;
                        else if (link.mergeDir == LPSP_MergeDGV.MergeDirector.Column)
                            dgv[link.columnIndex + 1, link.rowIndex + i].Selected = true;
                    }
                    ((LPSP_MergeDGV.MergeDataGridView)dgv).MergeDataGridViewCell();
                    //下面的语句证明，单元格是 进行了合并，但不知道为什么没有重绘,也就是没有触发单元格重绘事件
                    //MessageBox.Show(((MergeDataGridView.CellTagMsg)dgv[link.columnIndex + 1, link.rowIndex].Tag).isMergeFirstCell.ToString());
                }
            }
        }

        List<MergeLink> listMerge = null;
        public void 合并ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TabItem ti in this.tabControl1.Tabs)
            {
                if (ti.IsSelected)
                {
                    foreach (Control ctl in ti.AttachedControl.Controls)
                    {
                        //将单元格合并
                        if (ctl is LPSP_MergeDGV.MergeDataGridView)
                            Merge(ctl as LPSP_MergeDGV.MergeDataGridView, listMerge);
                    }
                }
            }
            合并ToolStripMenuItem.Visible = false;
        }

        private void DisableDataColumnsSort(DataGridView dgv)
        {
            foreach (DataGridViewColumn column in dgv.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        //打印当前表格
        private void Print_Click(object sender, EventArgs e)
        {
            foreach (TabItem ti in this.tabControl1.Tabs)
            {
                if (ti.IsSelected)
                {
                    foreach (Control ctl in ti.AttachedControl.Controls)
                    {
                        DataGridView dg = (DataGridView)ctl;
                        Print prt = new Print();
                        prt.btnFormer.Enabled = false;
                        prt.btnBack.Enabled = false;
                        prt.btnBackAll.Enabled = false;
                        string[] arrStr = ti.Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
                        if (arrStr.Length > 2)
                        {
                            prt.name = arrStr[0] + " " + arrStr[1] + "\n";
                            //if (arrStr[2].Trim() != "")
                            //    prt.name += "(" + arrStr[2] + ")";///dgv.Name;
                            /////prt.beizhu = arrStr[2];
                            prt.unit = arrStr[2];
                        }
                        else
                        {//当传过来 自定义表格时 arrStr没有[2]
                            prt.name = "自定义表格：" + arrStr[0] ;
                        }
                        
                        prt.dgv = dg;
                        prt.ShowDialog();
                    }
                }
            }
        }

        private void 打印所有表格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print prt = new Print(this.tabControl1);
            prt.Show();

            /*
            //遍历所有表格
            foreach (TabItem ti in this.tabControl1.Tabs)
            {
                foreach (Control ctl in ti.AttachedControl.Controls)
                {
                    DevComponents.DotNetBar.Controls.DataGridViewX dgv = (DevComponents.DotNetBar.Controls.DataGridViewX)ctl;
                    DataGridView dg = (DataGridView)dgv;
                    string[] arrStr = ti.Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
                    prt.name = arrStr[0] + " " + arrStr[1] + "\n";
                    if (arrStr[2].Trim() != "")
                        prt.name += "(" + arrStr[2] + ")";///dgv.Name;
                    //prt.beizhu = arrStr[2];
                    prt.unit = arrStr[3];
                    prt.dgv = dg;
                    prt.Show();
                }
            }
             * */
        }
        //导出所有表格
        private void SaveAllTab_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "2003 Excel表格文件(*.xls)|*.xls|2007 Excel表格文件(*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 2;

            TabItem ti = tabControl1.SelectedTab;
            string[] arrString = null;
            string filename = "";

            // arrString的格式示例为：
            // 表2 [0]
            // 2010年 枯水年 Test_SubA 系统电力平衡结果表 [1]
            // 方案1 Test_Sys最大负荷日 [2]
            // 单位：万千瓦时 [3]
            arrString = ti.Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
            // set the filename of saved excel file.
            filename = arrString[0];

            // 由于是全部导出，所以在文件名后面加上（合辑）后缀
            filename += "（合辑）";

            // set the filename to be saved default to the setted one by our algorithmn.
            saveFileDialog.FileName = filename;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Thread thdSub = new Thread(new ThreadStart(this.progressB));
                thdSub.Start();
                Thread.Sleep(100);
                try
                {                
                    var tabItems = this.tabControl1.Tabs;
                    List<DataGridView> list = new List<DataGridView>();
                    List<string> sheetNames = new List<string>();
                    List<string> titleList = new List<string>();
                    List<string> units = new List<string>();
                    List<string> descs = new List<string>();
                    int i = 1;
                    foreach (TabItem item in tabItems)
                    {
                        string[] arrStr = item.Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
                        string[] everyName=arrStr[0].Split(' ');
                        string sheetname = everyName[0];
                        sheetNames.Add(sheetname);

                        string description = "";
                        string unitString = "";
                        if (arrStr.Length > 2)
                        {
                            if (arrStr[1] != "")
                                description = arrStr[1];
                            unitString = arrStr[2];
                        }
                        units.Add(unitString);
                        descs.Add(description);

                        DataGridView dgv = null;
                        if ((dgv = (item.AttachedControl.Controls[0] as DataGridView)) != null)
                        {
                            list.Add(dgv);                           
                            titleList.Add(arrStr[0]);
                        }
                        else
                        {
                            MessageBox.Show("第" + i + "个将要被导出的不是一个工作表，将会被忽略");
                        }
                        i++;
                    }
                    if (saveFileDialog.FilterIndex == 1)
                    {
                        ExportTools.ExportToExcel.export03SheetGroups(list.ToArray(),
                            saveFileDialog.FileName,
                            sheetNames.ToArray(),
                            titleList.ToArray(),descs.ToArray(),
                            units.ToArray());
                    }
                    else if (saveFileDialog.FilterIndex == 2)
                    {                 
                        ExportTools.ExportToExcel.export07SheetGroups(list.ToArray(),
                            saveFileDialog.FileName, 
                            sheetNames.ToArray(),
                            titleList.ToArray(),descs.ToArray(),
                            units.ToArray());
                    }
                    else
                        throw new Exception("错误的导出文件类型");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    //关闭进度条
                    this.myprogress.isOver = true;
                }
            }
        }

        private void TableView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.parentForm != null)
            {
                this.parentForm.Visible = true;
            }
        }

        private void DeleteRow_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menuStrip = (sender as ToolStripMenuItem).Owner as ContextMenuStrip;
            DataGridView dgv= menuStrip.SourceControl as DataGridView;
            DataGridViewCell cell = dgv.CurrentCell;
            int rowIndex = cell.RowIndex;
            int colIndex = cell.ColumnIndex;
            cell = null;
            dgv.Rows.RemoveAt(rowIndex);
        }

        private void DeleteColumn_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menuStrip = (sender as ToolStripMenuItem).Owner as ContextMenuStrip;
            DataGridView dgv = menuStrip.SourceControl as DataGridView;
            DataGridViewCell cell = dgv.CurrentCell;
            int rowIndex = cell.RowIndex;
            int colIndex = cell.ColumnIndex;
            cell = null;
            dgv.Columns.RemoveAt(colIndex);
        }

        //导出当前表格
        private void SaveCurrentTab_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "2003 Excel表格文件(*.xls)|*.xls|2007 Excel表格文件(*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory=true;//保存对话框记忆上次打开的目录
            TabItem ti = tabControl1.SelectedTab;
            string[] arrString = null;
            string filename = "";
            // arrString的格式示例为：
            // 表2 [0]
            // 2010年 枯水年 Test_SubA 系统电力平衡结果表 [1]
            // 方案1 Test_Sys最大负荷日 [2]
            // 单位：万千瓦时 [3]
            arrString = ti.Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
            // set the filename of saved excel file.
            filename = arrString[0];

            // set the filename to be saved default to the setted one by our algorithmn.
            saveFileDialog.FileName = filename;
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Thread thdSub = new Thread(new ThreadStart(this.progressB));
                //thdSub.Start();
                //Thread.Sleep(100);
                try
                {
                    string description = "";
                    // set the unit string.
                    string unitString = "";
                    if (arrString.Length > 2)
                    {
                        if (arrString[1] != "")
                            description = "(" + arrString[1] + ")";
                        unitString = arrString[2];
                    }

                    DataGridView dgv = null;
                    var ctrls = ti.AttachedControl.Controls;
                    if (ctrls.Count > 0 && ((dgv = (ctrls[0] as DataGridView)) != null))
                    {
                        
                        string[] everyName=arrString[0].Split(' ');
                        string sheetname = everyName[0];
                        string title =  filename;
                        if (saveFileDialog.FilterIndex == 1)
                            ExportTools.ExportToExcel.export03(dgv, saveFileDialog.FileName, sheetname, 
                                title,description, unitString );
                        else if (saveFileDialog.FilterIndex == 2)
                            ExportTools.ExportToExcel.export07(dgv, saveFileDialog.FileName, sheetname,
                                title, description, unitString);
                        else
                            MessageBox.Show("错误的导出文件类型");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    ////关闭进度条
                    //this.myprogress.isOver = true;
                }
            }
        }

        private DataGridView RotateDataGridView(DataGridView dgv)
        {
            DataGridView newDgv = null;
            if (dgv is LPSP_MergeDGV.MergeDataGridView)
                newDgv = new LPSP_MergeDGV.MergeDataGridView(); 
            else
                newDgv=new DataGridView();

            DataTable dt = new DataTable();
            //dt.Columns.Add(dgv.Columns[0].HeaderText, typeof(string));

            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = dgv.Columns[0].HeaderText;
                newDgv.Columns.Add(col);
            }
            
            foreach (DataGridViewRow row in dgv.Rows)
            {
                //dt.Columns.Add(row.Cells[0].Value.ToString(), typeof(double));
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = row.Cells[0].Value.ToString();
                newDgv.Columns.Add(col);
            }
            for (int i = 1; i < dgv.Columns.Count; i++)
            {
                DataGridViewColumn col = dgv.Columns[i];
                //DataRow newRow = dt.NewRow();
                int rowIndex = newDgv.Rows.Add(1);
                //newRow[0] = col.Name;
                newDgv.Rows[rowIndex].Cells[0].Value = col.Name;
                int colIndex = 1;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    //为什么要转换成Double类型？ 当右边为空时  这个DBnull->null的转换是会发生异常的
                    //newDgv.Rows[rowIndex].Cells[colIndex].Value = Convert.ToDouble(row.Cells[col.Name].Value);
                    newDgv.Rows[rowIndex].Cells[colIndex].Value = row.Cells[col.Name].Value;
                    
                    colIndex++;
                }
                //dt.Rows.Add(newRow);
            }

            newDgv.AllowUserToAddRows = false;
            //newDgv.DataSource = dt;
            //MessageBox.Show(dd.TableName);
            newDgv.Dock = DockStyle.Fill;
            newDgv.ReadOnly = true;
            newDgv.RowHeadersVisible = false;

            newDgv.AllowUserToResizeRows = false;
            newDgv.ContextMenuStrip = contextMenuStrip1;
            newDgv.Name = dgv.Name;

            return newDgv;
        }

        private void Rotate_Click(object sender, EventArgs e)
        {            
            {                          
                foreach (TabItem ti in this.tabControl1.Tabs)
                {
                    DataGridView newDgv = null;
                    foreach (Control ctl in ti.AttachedControl.Controls)
                    {
                        DataGridView dgv = (DataGridView)ctl;
                        newDgv = RotateDataGridView(dgv);
                    }
                    ti.AttachedControl.Controls.Clear();
                    ti.AttachedControl.Controls.Add(newDgv);
                    
                    this.tabControl1.Refresh();

                    //禁用列排序
                    DisableDataColumnsSort(newDgv);

                    //调整单元格宽度
                    int n = newDgv.Columns.Count;
                    int m = newDgv.Rows.Count;
                    DataTable dt = newDgv.DataSource as DataTable;
                    newDgv.ColumnHeadersHeight = Convert.ToInt32(newDgv.Font.Size * 2.1) + 4;

                    for (int i = 0; i < m; ++i)
                    {
                        newDgv.Rows[i].Height = Convert.ToInt32(newDgv.Font.Size * 2.1);
                    }

                    if (newDgv.Columns.Contains("名  称"))
                    {
                        newDgv.Columns.Remove("名  称");
                        DataGridViewColumn col = new DataGridViewTextBoxColumn();
                        col.Name = "名  称";
                        col.DataPropertyName = "名  称";
                        newDgv.Columns.Insert(0, col);
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    for (int i = 0; i < n; ++i)
                    {
                        int k = newDgv.Columns[i].HeaderText.Length;
                        for (int g = 0; g < m - 1; g++)
                        {
                            if (Convert.ToString(newDgv.Rows[g].Cells[i].Value).Trim().Length > k)
                                k = Convert.ToString(newDgv.Rows[g].Cells[i].Value).Trim().Length;
                        }

                        if (i == 0)
                        {
                            newDgv.Columns[i].Width = Convert.ToInt32(k * Convert.ToInt32(newDgv.Font.Size) * 1.5);
                            //MessageBox.Show(Convert.ToString(dgv.Font.Size * 8));
                        }
                        else
                        {
                            if (k < 5) k = 5;
                            newDgv.Columns[i].Width = Convert.ToInt32(k * Convert.ToInt32(newDgv.Font.Size) * 0.8);
                            newDgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                        }
                    }
                }
            }
        }
    }
    
}
