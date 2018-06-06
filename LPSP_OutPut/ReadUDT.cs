using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using LPSP_MergeDGV;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System.Threading;


namespace HUST_OutPut
{
    public partial class ReadUDT : Form
    {

        //自定义表路径
        public string FilePath = null;
        public string FileName = null;
        //public MergeDataGridView dgv_UDT = new MergeDataGridView();
        public progress myprogress;
        public int columnHeader_count = 0;

        //模拟计算结果表
        DataSet OutDS = new DataSet();

        public ReadUDT()
        {
            InitializeComponent();
            inidgv_UDT();
        }

        public void inidgv_UDT()
        {
            this.dgv_UDT.Rows.Clear();
            this.dgv_UDT.Columns.Clear();
            DataGridViewTextBoxColumn column = null;
            //初始设置 创建的表格 一行 一列
            //for (int i = 0; i < 1; i++)
            {
                column = new DataGridViewTextBoxColumn();
                column.ReadOnly = true;
                column.HeaderText = columnHeader_count.ToString();
                columnHeader_count++;
                this.dgv_UDT.Columns.Add(column);
            }
            //for (int i = 0; i < 3; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                this.dgv_UDT.Rows.Add(row);
            }
        }
        //线程函数  在构造函数.创建线程时被引用
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();
        }
        //选择用户自定义文件
        private void Choose_UDT_Click(object sender, EventArgs e)
        {
            /*Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);*/
            OpenFileDialog openFileDialog = null;
            openFileDialog =new OpenFileDialog();
            openFileDialog.ShowDialog();
            FilePath = openFileDialog.FileName;
            FileName = openFileDialog.SafeFileName;
            this.Text = FileName;
            if (FilePath != "")
            {
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                Thread thdSub = new Thread(new ThreadStart(this.progressB));
                thdSub.Start();
                Thread.Sleep(100);
                ShowTable();
                this.myprogress.isOver = true;
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
            }
            else
                DoNothing();
        }

        //根据ChooseUDT选中的项显示相应的表格
        public void ShowTable()
        {
            #region 前版本
            /*
            int Block_Start = 0 ,Block_End = 0;
            int row_count = 0;//记录新插入的行的数目
            int row_index = 0;//记录插入行的索引
            int head_count = 0;//记录这类头的数目
            string is_ColumnHeaders = "ColumnHeaders"; //用来标识是否是新的块的第一个ColumnHeaders的第一个Row 这样可以决定是否进行行插入
            //dgv_UDT.DataSource = dt;
            
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(FilePath);
                if (xmldoc.FirstChild.Name.Equals("UserDefinedTable"))
                {
                    //得到顶层节点列表
                    XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                    foreach (XmlNode nodeTable in topM)
                    {
                        DataTable dt = new DataTable();
                        #region 块的处理
                        if (nodeTable.Name.Equals("Block"))
                        {
                            Head h1 = new Head();
                            
                            foreach (XmlNode node in nodeTable)
                            {
                                
                                #region ColumnHeaders的处理
                                if (node.Name.Equals("ColumnHeaders"))
                                {
                                    foreach (XmlNode sub_node in node)
                                    {
                                            if (node.Name.Equals(is_ColumnHeaders))
                                            {
                                                Block_Start = int.Parse(sub_node.Attributes["order"].Value);
                                                if(Block_Start>=1&&Block_End>=1)
                                                {
                                                    for (int i = Block_End; i < Block_Start-1; i++)
                                                    {
                                                        DataGridViewRow r = new DataGridViewRow();
                                                        row_index = dgv_UDT.Rows.Add(r);
                                                        row_count++;
                                                    }
                                                }
                                            }

                                            foreach (XmlNode ssub_node in sub_node)
                                            {
                                                head_count++;
                                               
                                                h1.start_order = int.Parse(ssub_node.Attributes["startOrder"].Value);
                                                h1.through_cells = int.Parse(ssub_node.Attributes["throughCells"].Value);
                                                h1.value = ssub_node.Attributes["value"].Value;

                                                //第一个Head确定表格的列数
                                                if ((head_count == 1) && (h1.through_cells > 1))
                                                {
                                                    //新建列
                                                    for (int i = 0; i < h1.start_order + h1.through_cells; i++)
                                                    {
                                                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                        dgv_UDT.Columns.Add(column);
                                                    }
                                                    
                                                    //当Row order从非0开始时，插入相应数量的行数
                                                    if (int.Parse(sub_node.Attributes["order"].Value) > 0)
                                                    {
                                                        row_index = dgv_UDT.Rows.Add(int.Parse(sub_node.Attributes["order"].Value)+1);
                                                        row_count += int.Parse(sub_node.Attributes["order"].Value);
                                                        
                                                    }

                                                    //
                                                    else if (int.Parse(sub_node.Attributes["order"].Value) == 0)
                                                    {
                                                        row_index = this.dgv_UDT.Rows.Add(1);
                                                        row_count++;
                                                    }
                                                }


                                                #region  确定筛选条件
                                                if (ssub_node.Attributes["filter"] != null)
                                                {
                                                    
                                                    //当有了筛选条件，就可以在datatable中插入“列”和“行”了
                                                    h1.filter = ssub_node.Attributes["filter"].Value.ToString();
                                                    h1.id = ssub_node.Attributes["id"].Value.ToString();
                                                    foreach (XmlNode node_filter in topM)
                                                    {
                                                        if (node_filter.Name.Equals("Filter"))
                                                        {
                                                            foreach (XmlNode node_file in node_filter)
                                                            {
                                                                if (node_file.Attributes["name"].Value.ToString().Equals(h1.filter))
                                                                {

                                                                    h1.filter_0 = node_file.Attributes["filter_0"].Value.ToString();
                                                                    h1.filter_1 = node_file.Attributes["filter_1"].Value.ToString();
                                                                    h1.filter_2 = node_file.Attributes["filter_2"].Value.ToString();
                                                                    h1.filter_3 = node_file.Attributes["filter_3"].Value.ToString();
                                                                    h1.filter_4 = node_file.Attributes["filter_4"].Value.ToString();
                                                                    foreach (XmlNode node_filter1 in topM)
                                                                    {
                                                                        if (node_filter1.Name.Equals("File"))
                                                                        { 
                                                                            foreach(XmlNode node_file1 in node_filter1)
                                                                            {
                                                                                if (node_file1.Attributes["name"].Value.ToString().Equals(node_file.Attributes["file"].Value.ToString()))
                                                                                {
                                                                                    h1.file = node_file1.Attributes["file"].Value.ToString();
                                                                                    h1.table = node_file1.Attributes["table"].Value.ToString();

                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    //读入数据到DataTable
                                                    this.ReadOutFiles(h1.file);
                                                    //dt = OutDS.Tables[h1.table].DefaultView.ToTable(true);
                                                    
                                                    DataView dv = new DataView();
                                                    DataTable tmp = new DataTable();
                                                    int count = 0;
                                                    dv.Table = OutDS.Tables[h1.table];
                                                    dv.RowFilter = "Prj = "+ h1.filter_0+"and Yrs = " + h1.filter_1 + "and Hyd = "+ h1.filter_2 + "and sID = " + h1.filter_3 + "and dID = "+ h1.filter_4;
                                                    tmp = dv.ToTable();
                                                    dt.Columns.Add("Flg"+h1.value,typeof(String));
                                                    dt.Columns.Add(h1.value,typeof(Double));
                                                    if (dt.Columns.Count <= 2)
                                                    {
                                                        foreach (DataRow row1 in tmp.Rows)
                                                        {
                                                            DataRow rw = dt.NewRow();
                                                            rw["Flg" + h1.value] = row1["Flg"];
                                                            rw[h1.value] = row1[h1.value];
                                                            dt.Rows.Add(rw);
                                                        }
                                                    }

                                                    else
                                                    {
                                                        foreach (DataRow row1 in tmp.Rows)
                                                        {
                                                            if (count < dt.Rows.Count)
                                                            {
                                                                dt.Rows[count]["Flg" + h1.value] = row1["Flg"];
                                                                dt.Rows[count][h1.value] = row1[h1.value];
                                                                count++;
                                                            }
                                                            else
                                                            {
                                                                DataRow rw = dt.NewRow();
                                                                rw["Flg" + h1.value] = row1["Flg"];
                                                                rw[h1.value] = row1[h1.value];
                                                                dt.Rows.Add(rw);
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion

                                                

                                                #region 加入一个Head
                                                if (h1.through_cells > 1)
                                                {
                                                    dgv_UDT.ClearSelection();
                                                    //dgv_UDT.Rows[row_index].Cells[0].Selected = false;
                                                    //dgv_UDT.Rows[0].Cells[0].Selected = false;//是当前选中的单元格为不选中状态
                                                    dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;
                                                    
                                                    for (int i = h1.start_order; i < h1.start_order + h1.through_cells; i++)
                                                    {
                                                        dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value) ].Cells[i].Selected = true;
                                                        
                                                    }
                                                    dgv_UDT.MergeDataGridViewCell();
                                                    
                                                }
                                                else if(h1.through_cells == 1)
                                                {
                                                    if (dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells.Count > h1.start_order)
                                                    {
                                                        dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;
                                                    }
                                                    else
                                                    {
                                                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                        this.dgv_UDT.Columns.Add(column);
                                                        dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;
                                                    }
                                                  
                                                }
                                                #endregion                                        
                                            }
                                        is_ColumnHeaders = "RowHeaders";
                                        DataGridViewRow row = new DataGridViewRow();
                                        row_index = dgv_UDT.Rows.Add(row);
                                        row_count++;
                                    }
                                }
                                #endregion
                                #region RowHeaders的处理
                                else if (node.Name.Equals("RowHeaders"))
                                {
                                    foreach (XmlNode sub_node in node)
                                    {
                                        
                                        
                                        //if (int.Parse(sub_node.Attributes["order"].Value) > 0)
                                        {
                                            //Head h1 = new Head();
                                            foreach (XmlNode ssub_node in sub_node)
                                            {
                                                DataGridViewRow r = new DataGridViewRow();
                                                row_index = dgv_UDT.Rows.Add(r);
                                                row_count++;
                                                h1.start_order = int.Parse(ssub_node.Attributes["startOrder"].Value);
                                                h1.through_cells = int.Parse(ssub_node.Attributes["throughCells"].Value);
                                                h1.value = ssub_node.Attributes["value"].Value;
                                                dgv_UDT.Rows[h1.start_order].Cells[h1.through_cells].Value = h1.value;

                                                #region 填入表格数据
                                                //DataView  dv = new DataView();
                                                //dv.Table = dt;
                                                //dv.RowFilter = "Flg = " + h1.value;
                                                //DataTable tmp = new DataTable();
                                                //tmp = dv.ToTable();
                                                int j = 1,count = 0;

                                                for (int i = h1.through_cells + 1; i < dgv_UDT.Rows[h1.start_order].Cells.Count; i++)
                                                {
                                                    foreach (DataRow rw in dt.Rows)
                                                    {
                                                        if (rw[count].ToString().Equals(h1.value))
                                                        {
                                                            dgv_UDT.Rows[h1.start_order].Cells[i].Value = rw[j];
                                                            j += 2;
                                                            count += 2;
                                                            break;
                                                        }
                                                    }
                                                }
                                                
                                                #endregion
                                            }
                                            
                                        }
                                        
                                        
                                    }
                                    is_ColumnHeaders = "ColumnHeaders";

                                }
                                #endregion
                            }
                            #region 合并第一列单元格形成块
                            Block_End = h1.start_order;
                            dgv_UDT.Rows[Block_Start].Cells[0].Value = nodeTable.Attributes["name"].Value.ToString();
                            dgv_UDT.ClearSelection();
                            for (int i = Block_Start; i <= Block_End; i++)
                            {
                                //dgv_UDT.Rows[int.Parse(sub_node.Attributes["order"].Value) - 1].Cells[i].Selected = true;
                                dgv_UDT.Rows[i].Cells[0].Selected = true;
                            }
                            dgv_UDT.MergeDataGridViewCell();
                            #endregion

                            
                        }
                        
                        #endregion

                    }
                }
                else
                {
                    MessageBox.Show("It is not a User Defined Table");
                }
            }
            
            catch (Exception exc)
            { MessageBox.Show(exc.Message); }
             * */

            #endregion
            #region 新版本
            
            
            //建立一个datatable来保存各列是否为合并列
            //bool is_SUM = false;//标记表格中是否有求和列，如果有，则置为TRUE，默认没有
            DataTable is_SUM = new DataTable();
            DataColumn ColumnIndex = new DataColumn();
            ColumnIndex.ColumnName = "ColumnIndex";
            DataColumn is_sum = new DataColumn();
            is_sum.ColumnName = "is_sum";
            is_SUM.Columns.Add(ColumnIndex);
            is_SUM.Columns.Add(is_sum);



            int Block_Start = 0,Block_End = 0;
            DataTable dt = new DataTable();
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            if (doc.FirstChild.Name.Equals("UserDefinedTable"))
            {
                XmlNodeList topM = doc.DocumentElement.ChildNodes;
                try
                {
                    foreach (XmlNode nodeTable in topM)
                    {
                        if (nodeTable.Name.Equals("Block"))
                        {
                            Head h1 = new Head();
                            foreach(XmlNode r_c_head in nodeTable)
                            {
                                if(r_c_head.Name.Equals("RowHeaders"))
                                {
                                    foreach (XmlNode column in r_c_head)
                                    {
                                        int columnIndex = int.Parse(column.Attributes["order"].Value);
                                        foreach (XmlNode head in column)
                                        {
                                            if (column.LastChild == head)
                                                Block_End = int.Parse(head.Attributes["startOrder"].Value);
                                            h1.start_order = int.Parse(head.Attributes["startOrder"].Value);
                                            h1.through_cells = int.Parse(head.Attributes["throughCells"].Value);
                                            h1.value = head.Attributes["value"].Value;
                                            h1.id = head.Attributes["id"].Value;
                                            if  (h1.start_order == this.dgv_UDT.Rows.Count-1)
                                            {
                                                 this.dgv_UDT.Rows.Insert(this.dgv_UDT.Rows.Count-1,1);
                                            }
                                            
                                            this.dgv_UDT[columnIndex, h1.start_order].Value = h1.value;
                                            //this.dgv_UDT[]
                                            if (h1.value != "")
                                            {
                                                #region 填入表格数据
                                                int j = 1, count = 0;
                                                int start_index = columnIndex + 1, end_index = columnIndex + 1;
                                                for (int i = columnIndex + 1; i < dgv_UDT.ColumnCount; i++)
                                                {
                                                    //if (count >= dt.Columns.Count && is_SUM == true)
                                                    //{
                                                    //    double sum = 0;
                                                    //    for (int t = columnIndex + 1; t < dgv_UDT.ColumnCount - 1; t++)
                                                    //        sum += double.Parse(this.dgv_UDT.Rows[h1.start_order].Cells[t].Value.ToString());
                                                    //    this.dgv_UDT.Rows[h1.start_order].Cells[i].Value = sum;
                                                    //    break;
                                                    //}
                                                    //else if(count<dt.Rows.Count && is_SUM == true)
                                                    //{

                                                    //}
                                                    //else if (count >= dt.Columns.Count && is_SUM != true)
                                                    //    break;

                                                    bool temp = false;
                                                    foreach (DataRow temprw in is_SUM.Rows)
                                                    {
                                                        if (int.Parse(temprw["ColumnIndex"].ToString()) == i)
                                                        {
                                                            temp = bool.Parse(temprw["is_sum"].ToString());
                                                            break;
                                                        }
                                                    }
                                                    if (temp)
                                                    {
                                                        //end_index = i - 1;
                                                        double sum = 0;
                                                        for (int t = start_index; t < end_index; t++)
                                                            sum += double.Parse(this.dgv_UDT.Rows[h1.start_order].Cells[t].Value.ToString());
                                                        this.dgv_UDT.Rows[h1.start_order].Cells[i].Value = sum;

                                                        i++;
                                                        start_index = i;
                                                        end_index = i;
                                                        //break;

                                                    }
                                                    if (count >= dt.Columns.Count)
                                                        break;
                                                    else
                                                    {
                                                        foreach (DataRow rw in dt.Rows)
                                                        {
                                                            if (h1.id != null && rw[count].ToString().Equals(h1.id))
                                                            {
                                                                dgv_UDT.Rows[h1.start_order].Cells[i].Value = rw[j];
                                                                j += 2;
                                                                count += 2;
                                                                break;
                                                            }
                                                        }
                                                        end_index++;
                                                    }
                                                }

                                                #endregion
                                            }

                                        }
                                    }
                                }
                                else if(r_c_head.Name.Equals("ColumnHeaders"))
                                {
                                    
                                    foreach(XmlNode row in r_c_head)
                                    {
                                        if(r_c_head.FirstChild == row)
                                            Block_Start = int.Parse(row.Attributes["order"].Value);
                                        int rowIndex = int.Parse(row.Attributes["order"].Value);
                                        if (rowIndex == this.dgv_UDT.Rows.Count-1)
                                        {
                                            //while(rowIndex>this.dgv_UDT.Rows.Count-1)
                                                this.dgv_UDT.Rows.Add(1);
                                            
                                        }
                                        //else
                                        {
                                            foreach (XmlNode head in row)
                                            {
                                                if (row.ChildNodes.Count >= 1)
                                                {
                                                    h1.start_order = int.Parse(head.Attributes["startOrder"].Value);
                                                    h1.through_cells = int.Parse(head.Attributes["throughCells"].Value);
                                                    h1.value = head.Attributes["value"].Value;
                                                    if (h1.through_cells > 1)
                                                    {
                                                        if (h1.start_order + h1.through_cells > this.dgv_UDT.ColumnCount)
                                                        {
                                                            int temp = this.dgv_UDT.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp;i++ )
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                column.HeaderText = columnHeader_count.ToString();
                                                                
                                                                columnHeader_count++;
                                                                column.ReadOnly = true;
                                                                this.dgv_UDT.Columns.Add(column);
                                                            }
                                                        }
                                                        //else
                                                        {
                                                            this.dgv_UDT.ClearSelection();
                                                            for (int i = h1.start_order; i < h1.start_order + h1.through_cells; i++)
                                                                this.dgv_UDT[i, rowIndex].Selected = true;
                                                            this.dgv_UDT[h1.start_order, rowIndex].Value = h1.value;
                                                            this.dgv_UDT.MergeSelectedCell();
                                                        }
                                                    }
                                                    else if (h1.through_cells == 1 && head.Attributes["id"].Value != "__SUM__")
                                                    {
                                                        if (h1.start_order + h1.through_cells > this.dgv_UDT.ColumnCount)
                                                        {
                                                            int temp = this.dgv_UDT.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp ; i++)
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                column.ReadOnly = true;
                                                                column.HeaderText = columnHeader_count.ToString();
                                                                columnHeader_count++;
                                                                this.dgv_UDT.Columns.Add(column);
                                                            }
                                                            
                                                        }
                                                        //else
                                                        {

                                                            h1.id = head.Attributes["id"].Value;
                                                            h1.filter = head.Attributes["filter"].Value;
                                                            this.dgv_UDT[h1.start_order, rowIndex].Value = h1.value;
                                                        }
                                                        //确定筛选条件
                                                        this.SetFilter(h1);
                                                        //读入数据到DataTable
                                                        this.CreateDataTable(dt, h1);

                                                        DataRow newrow = is_SUM.NewRow();
                                                        newrow["ColumnIndex"] = h1.start_order;
                                                        newrow["is_sum"] = false;
                                                        is_SUM.Rows.Add(newrow);
                                                        
                                                    }
                                                    else if (h1.through_cells == 1 && head.Attributes["id"].Value == "__SUM__")
                                                    {
                                                        h1.id = head.Attributes["id"].Value;
                                                        //is_SUM = true;
                                                        if (h1.start_order + h1.through_cells > this.dgv_UDT.ColumnCount)
                                                        {
                                                            int temp = this.dgv_UDT.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp; i++)
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                column.ReadOnly = true;
                                                                column.HeaderText = columnHeader_count.ToString();
                                                                columnHeader_count++;
                                                                this.dgv_UDT.Columns.Add(column);
                                                            }

                                                        }
                                                        this.dgv_UDT[h1.start_order, rowIndex].Value = h1.value;
                                                        DataRow newrow = is_SUM.NewRow();
                                                        newrow["ColumnIndex"] = h1.start_order;
                                                        newrow["is_sum"] = true;
                                                        is_SUM.Rows.Add(newrow);
                                                    }
                                                }
                                                else
                                                { continue; }
                                            }
                                        }
                                    }
                                }
                            }

                            this.dgv_UDT.ClearSelection();
                            for (int i = Block_Start; i <= Block_End; i++)
                                this.dgv_UDT[0, i].Selected = true;
                            this.dgv_UDT[0, Block_Start].Value = nodeTable.Attributes["name"].Value;
                            this.dgv_UDT.MergeSelectedCell();
                            dt.Rows.Clear();
                            dt.Columns.Clear();
                            is_SUM.Clear();
                        }
                    }
                    foreach (DataGridViewColumn item in this.dgv_UDT.Columns)
                    {
                        item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("非用户自定义表格！");
            }
            #endregion
        }
        public void SetFilter(Head h1)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(FilePath);
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode node_filter in topM)
            {
                if (node_filter.Name.Equals("Filter"))
                {
                    foreach (XmlNode node_file in node_filter)
                    {
                        if (node_file.Attributes["name"].Value.ToString().Equals(h1.filter))
                        {

                            h1.filter_0 = node_file.Attributes["filter_0"].Value.ToString();
                            h1.filter_1 = node_file.Attributes["filter_1"].Value.ToString();
                            h1.filter_2 = node_file.Attributes["filter_2"].Value.ToString();
                            h1.filter_3 = node_file.Attributes["filter_3"].Value.ToString();
                            h1.filter_4 = node_file.Attributes["filter_4"].Value.ToString();
                            foreach (XmlNode node_filter1 in topM)
                            {
                                if (node_filter1.Name.Equals("File"))
                                {
                                    foreach (XmlNode node_file1 in node_filter1)
                                    {
                                        if (node_file1.Attributes["name"].Value.ToString().Equals(node_file.Attributes["file"].Value.ToString()))
                                        {
                                            h1.file = node_file1.Attributes["file"].Value.ToString();
                                            h1.table = node_file1.Attributes["table"].Value.ToString();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void CreateDataTable(DataTable dt , Head h1)
        {

            this.ReadOutFiles(h1.file);
            //dt = OutDS.Tables[h1.table].DefaultView.ToTable(true);

            DataView dv = new DataView();
            DataTable tmp = new DataTable();
            int count = 0;
            dv.Table = OutDS.Tables[h1.table];
            dv.RowFilter = "Prj = " + h1.filter_0 + "and Yrs = " + h1.filter_1 + "and Hyd = " + h1.filter_2 + "and sID = " + h1.filter_3 + "and dID = " + h1.filter_4;
            tmp = dv.ToTable();
            dt.Columns.Add("Flg" + h1.value + h1.filter, typeof(String));
            dt.Columns.Add(h1.value + h1.filter, typeof(Double));
            string str = h1.id;
            if (!str.StartsWith("MAX_(") && !str.StartsWith("MIN_("))
            {

                if (dt.Columns.Count <= 2)
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        DataRow rw = dt.NewRow();
                        rw["Flg" + h1.value+h1.filter] = row1["Flg"];
                        rw[h1.value + h1.filter] = row1[h1.id];
                        dt.Rows.Add(rw);
                    }
                }

                else
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        if (count < dt.Rows.Count)
                        {
                            dt.Rows[count]["Flg" + h1.value + h1.filter] = row1["Flg"];
                            dt.Rows[count][h1.value + h1.filter] = row1[h1.id];
                            count++;
                        }
                        else
                        {
                            DataRow rw = dt.NewRow();
                            rw["Flg" + h1.value + h1.filter] = row1["Flg"];
                            rw[h1.value + h1.filter] = row1[h1.id];
                            dt.Rows.Add(rw);
                        }
                    }
                }
            }
            else if(str.StartsWith("MAX_("))
            {
                string substring = str.Substring(5, str.Length - 4 - 2);
                string[] temp = substring.Split(',');
                if (dt.Columns.Count <= 2)
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        double max = 0;
                        DataRow rw = dt.NewRow();
                        rw["Flg" + h1.value + h1.filter] = row1["Flg"];
                        foreach (string i in temp)
                        {
                            double tempValue = double.Parse(row1[i].ToString());
                            if (tempValue > max)
                                max = tempValue;
                        }
                        rw[h1.value + h1.filter] = max;
                        dt.Rows.Add(rw);
                    }
                }

                else
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        double max = 0;
                        if (count < dt.Rows.Count)
                        {
                            dt.Rows[count]["Flg" + h1.value + h1.filter] = row1["Flg"];
                            foreach (string i in temp)
                            {
                                double tempValue = double.Parse(row1[i].ToString());
                                if (tempValue > max)
                                    max = tempValue;

                            }
                            dt.Rows[count][h1.value + h1.filter] = max;
                            count++;
                        }
                        else
                        {
                            DataRow rw = dt.NewRow();
                            rw["Flg" + h1.value + h1.filter] = row1["Flg"];
                            foreach (string i in temp)
                            {
                                double tempValue = double.Parse(row1[i].ToString());
                                if (tempValue > max)
                                    max = tempValue;

                            }
                            rw[h1.value + h1.filter] = max;
                            dt.Rows.Add(rw);
                        }
                    }
                }

            }
            else if (str.StartsWith("MIN_("))
            {
                string substring = str.Substring(5, str.Length - 4 - 2);
                string[] temp = substring.Split(',');
                if (dt.Columns.Count <= 2)
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        double min = double.Parse(row1[temp[0]].ToString());
                        DataRow rw = dt.NewRow();
                        rw["Flg" + h1.value + h1.filter] = row1["Flg"];
                        foreach (string i in temp)
                        {
                            double tempValue = double.Parse(row1[i].ToString());
                            if (tempValue < min)
                                min = tempValue;

                        }
                        rw[h1.value + h1.filter] = min;
                        dt.Rows.Add(rw);
                    }
                }

                else
                {
                    foreach (DataRow row1 in tmp.Rows)
                    {
                        double min = double.Parse(row1[temp[0]].ToString());
                        if (count < dt.Rows.Count)
                        {
                            dt.Rows[count]["Flg" + h1.value + h1.filter] = row1["Flg"];
                            foreach (string i in temp)
                            {
                                double tempValue = double.Parse(row1[i].ToString());
                                if (tempValue < min)
                                    min = tempValue;

                            }
                            dt.Rows[count][h1.value + h1.filter] = min;
                            count++;
                        }
                        else
                        {
                            DataRow rw = dt.NewRow();
                            rw["Flg" + h1.value + h1.filter] = row1["Flg"];
                            foreach (string i in temp)
                            {
                                double tempValue = double.Parse(row1[i].ToString());
                                if (tempValue < min)
                                    min = tempValue;

                            }
                            rw[h1.value + h1.filter] = min;
                            dt.Rows.Add(rw);
                        }
                    }
                }
            }
        }

        //从模拟计算结果中 读取数据
        private bool ReadOutFiles(string filename)
        {
            try
            {
                DataSet ds1 = new DataSet();
                ds1.ReadXml(filename); 
                OutDS.Merge(ds1, true);

                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message + "\n获取文件数据失败！");
                return false;
            }
        } 

        public int DoNothing()
        {
            return 0;
        }

        private void 重置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //dt.Rows.Clear();
            this.dgv_UDT.Rows.Clear();
            this.dgv_UDT.Columns.Clear();
            this.Text = this.FileName;
            this.inidgv_UDT();
            this.Close();

        }

        //private void 重置条件ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Reset_FilterWindow rf = new Reset_FilterWindow();
        //    rf.Text = FileName;
        //    rf.labelX1.Text =FilePath;
        //    rf.Init();
            
        //    rf.ShowDialog();
        //}

        private void 打印ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            //dgVprint1.Print(this.dgv_UDT,true);
            //PrintUDT prt = new PrintUDT();
            //Print prt = new Print();
            //prt.btnFormer.Enabled = false;
            //prt.btnBack.Enabled = false;
            //prt.btnBackAll.Enabled = false;
            //prt.dgv = this.dgv_UDT;
            //prt.ShowDialog();
            //VBprinter40.MulHeaderDataGridView d1 = new VBprinter40.MulHeaderDataGridView();

            //VBprinter40.VB2008Print vB2008Print1 = new VBprinter40.VB2008Print();
            //vB2008Print1.NewDoc();
            //vB2008Print1.NewPage(true);
            //vB2008Print1.EnabledPageSetting = true;
            //vB2008Print1.PrintDGV(dgv_UDT, this.FileName, new Font("黑体", 18, FontStyle.Bold), StringAlignment.Center, "1111", true, false, 0, true, true, 0, 0, false, dgv_UDT.ColHeaderTreeView);
            //vB2008Print1.EndDoc(this.Text + "打印");
            //vB2008Print1.Dispose();


            VBprinter40.DGVprint DgVprint1 = new VBprinter40.DGVprint();
            DgVprint1.PrintType = VBprinter40.DGVprint.mytype.GeneralPrint;
            DgVprint1.Alignment = StringAlignment.Center;//'表格居中
            DgVprint1.IsDrawmargin = false;
            DgVprint1.MainTitle = this.FileName;//"以普通表格打印多表头组件";
            //DgVprint1.PageHeaderLeft = "页眉内容（左边）";
            //DgVprint1.PageHeaderMiddle = "页眉内容（中间）";
            //DgVprint1.PrintType = VBprinter40.DGVprint.mytype.GeneralPrint ;//'表示普通表格样式

            //在此还可以设置其他属性，当然，也可以DGVPRINT1的悔改窗口中进行设置，效果完全一样的
            DgVprint1.Print(dgv_UDT, false, "", dgv_UDT.ColHeaderTreeView);
            
        }

    }


    
}
