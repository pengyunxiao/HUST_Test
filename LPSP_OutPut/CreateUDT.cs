using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.IO;
using ProS_Assm;
using DevComponents.DotNetBar;

namespace HUST_OutPut
{
    public partial class CreateUDT : Form
    {
        public Dictionary<string, string> tableIdAndName = null; //基础表ID 和 表名的对应

        //模拟计算结果表
        DataSet OutDS = new DataSet();

        //筛选条件计数
        public int filterCount = 0;

        //文件计数
        public int fileCount = 0;

        public progress myprogress;
        public CreateUDT()
        {
            Control.CheckForIllegalCrossThreadCalls = false; 
            InitializeComponent();


            tableIdAndName = CreateUDT.GetAllTableIdAndName();

            //初始化 源表 DataGridView
            DataGridViewTextBoxColumn column = null;
            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "表格名称";
            dgvSourceTable.Columns.Add(column);
            column = new DataGridViewTextBoxColumn();
            column.HeaderText = "表格ID";
            dgvSourceTable.Columns.Add(column);

            //初始化表格--将初始化放在点击浏览按钮后，因为要根据点击的按钮来决定对“dataGridViewCreate”进行的是初始化还是导入现有的表格
            //清空表格ToolStripMenuItem_Click(null,null);
            
        }

        //线程函数  在构造函数.创建线程时被引用
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();
        }
        //从模拟计算结果中 读取数据
        private bool ReadOutFiles()
        {
            try
            {
                DataSet ds1 = new DataSet();
                ds1.ReadXml(this.tbSourceFile.Text);

                #region 读取源文件
                string str = this.tbSourceFile.Text;
                if(str.EndsWith("_RST.xml") || str.EndsWith("_GEN.xml"))
                {
                    str = str.Substring(0,str.Length-8);
                    str += ".xml";
                }
                DataSet ds2 = new DataSet();
                ds2.ReadXml(str);
                #endregion
                OutDS.Clear();
                OutDS.Merge(ds1, true);
                OutDS.Merge(ds2, true);
                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message + "\n获取文件数据失败！");
                return false;
            }
        }

        //获取所有的“基础表号和表名”的对应
        static public Dictionary<string, string> GetAllTableIdAndName()
        {
            Dictionary<string, string> idAndName = new Dictionary<string, string>();
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Application.StartupPath + "\\TableViewConfig_" + ProS_Assm.UnitMnt.mMode + ".xml");
                //得到顶层节点列表
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode element in topM)
                {
                    if (element.Name.ToLower() == "dictionary")
                    {
                        foreach (XmlNode el in element.ChildNodes)//读元素值
                        {
                            if (el.Name.ToLower().Equals("item") && el.Attributes["id"] != null && el.Attributes["id"].Value.ToLower()=="dict")
                            {
                                foreach (XmlNode xn in el.ChildNodes)
                                {
                                    idAndName.Add(xn.Attributes["id"].Value, xn.Attributes["name"].Value);
                                }
                            }
                        }
                    }
                }
                return idAndName;
            }
            catch (Exception e) { MessageBox.Show(e.Message); return null; }
        }

        //选择表源文件
        private void btnGetFile_Click(object sender, EventArgs e)
        {
            this.dgvCreated.AllowUserToAddRows = false;
            if (dlgOpenSourceFile.ShowDialog() == DialogResult.OK)
            {
                
                this.tbSourceFile.Text = dlgOpenSourceFile.FileName;

                dgvSourceTable.Rows.Clear();//清除已有的表行
                DataSet ds = new DataSet();
                ds.ReadXml(this.tbSourceFile.Text);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(this.tbSourceFile.Text);
               // foreach (XmlElement node in xmlDoc)
                {
                    if (xmlDoc.DocumentElement.Name != "UserDefinedTable" && xmlDoc.DocumentElement.Name != "DataSet1" && !this.tbSourceFile.Text.EndsWith("_MAP.xml"))
                    {
                        清空表格ToolStripMenuItem_Click(null, null);
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (this.tableIdAndName.ContainsKey(dt.TableName))
                                dgvSourceTable.Rows.Add(this.tableIdAndName[dt.TableName], dt.TableName); //dt的表名 其实是 表的ID
                            else
                                dgvSourceTable.Rows.Add("", dt.TableName);
                            
                        }
                        this.Deal.Visible = false;
                        this.contextMenuStripDgvCreated.Items["setFilter"].Visible = false;
                        //this.btn_Create.Visible = false;
                        this.input_UDT.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("不是表源文件！");
                        this.tbSourceFile.Text = "";
                    }
                }
                
            }
        }

        //选择表格中对应的表
        private void dgvSourceTable_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0)
            {
                string tableId = dgvSourceTable.Rows[e.RowIndex].Cells[1].Value.ToString(); //表格ID

                dgvColumn.Rows.Clear();
                dgvColumn.Columns.Clear();
                dgvRow.Rows.Clear();
                dgvRow.Columns.Clear();

                #region ********获取行列************
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(Application.StartupPath + "\\TableViewConfig_" + ProS_Assm.UnitMnt.mMode + ".xml");
                    //得到顶层节点列表
                    XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                    foreach (XmlNode element in topM)
                    {
                        if (element.Name.ToLower() == "dictionary")
                        {
                            foreach (XmlNode el in element.ChildNodes)//读元素值
                            {
                                if (el.Name.ToLower().Equals("item") && el.Attributes["id"] != null && el.Attributes["id"].Value.ToLower() == tableId.ToLower())
                                {
                                    foreach (XmlNode xn in el.ChildNodes)
                                    {
                                        if (xn.Name.ToLower() == "columns")
                                        {
                                            foreach (XmlNode column in xn)
                                            {
                                                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                                                dgvColumn.Columns.Add(col);
                                            }
                                            dgvColumn.Rows.Add(2);
                                            dgvColumn.Rows[0].HeaderCell.Value = "列名";
                                            dgvColumn.Rows[1].HeaderCell.Value = "列Id";
                                            for (int i = 0; i < xn.ChildNodes.Count;i++ )
                                            {
                                                dgvColumn[i, 0].Value=xn.ChildNodes[i].Attributes["name"].Value;
                                                dgvColumn[i, 1].Value = xn.ChildNodes[i].Attributes["id"].Value;
                                            }
                                            for (int i = 0; i < 6; i++)
                                            {
                                                this.dgvColumn.Columns[i].Visible = false;
                                            }
                                        }
                                        else if (xn.Name.ToLower() == "rows")
                                        {
                                            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn();
                                            col0.HeaderText = "行名";
                                            dgvRow.Columns.Add(col0);
                                            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                                            dgvRow.Columns.Add(col1);
                                            col1.HeaderText = "行Id";

                                            foreach (XmlNode row in xn)
                                            {
                                                dgvRow.Rows.Add(row.Attributes["content"].Value,row.Attributes["code"].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exc) { MessageBox.Show(exc.Message); }
                #endregion

                //读取模拟计算结果文件   在设置 列或者行 的 筛选条件时  会用到里面的数据
                ReadOutFiles();//最好放前面 后面的初始化都需要它先初始化

                #region  *********初始设置条件筛选**********
                this.labelFilter0.Text = this.dgvColumn.Rows[0].Cells[0].Value.ToString();// +"：";
                this.labelFilter1.Text = this.dgvColumn.Rows[0].Cells[1].Value.ToString();// +"：";

                if ((this.dgvColumn.Rows[0].Cells[3].Value.ToString() == "") && (this.dgvColumn.Rows[1].Cells[3].Value.ToString() == "mID"))
                    this.labelFilter3.Text = "月份";
                else
                    this.labelFilter3.Text = this.dgvColumn.Rows[0].Cells[3].Value.ToString();//+ "：";
                if (this.labelFilter3.Text == "电站ID")
                    this.labelFilter3.Text = "电站";
                if (this.labelFilter3.Text == "系统ID")
                    this.labelFilter3.Text = "系统或分区";

                this.labelFilter2.Text = this.dgvColumn.Rows[0].Cells[2].Value.ToString();// +"：";
                if (this.labelFilter2.Text == "水文年")
                    this.labelFilter2.Text = "水文条件";
                this.labelFilter4.Text = this.dgvColumn.Rows[0].Cells[4].Value.ToString();// +"：";

                cbFilter0.Items.Clear();
                cbFilter1.Items.Clear();
                cbFilter2.Items.Clear();
                cbFilter3.Items.Clear();
                cbFilter4.Items.Clear();

                foreach (DataRow row in this.OutDS.Tables[tableId].Rows)
                {
                    //增加方案描述，方案的显示也变成了“1-sth”这样的形式    ----2014.07.18 GAO Yang
                    #region projectDescription
                    DataTable dt = OutDS.Tables["方案表"];
                    
                    string Prj_str = null;
                    foreach (DataRow r in dt.Rows)
                    {
                        if (row[0].ToString().Equals(r[1].ToString()) && r[2].ToString() != "无")
                        {
                            Prj_str = row[0].ToString() + "-" + r[2].ToString();
                            if (!this.cbFilter0.Items.Contains(Prj_str))
                                cbFilter0.Items.Add(Prj_str);
                            
                            //cbFilter0.Items.Add(str);
                            break;
                        }
                        else if (r[2].ToString() == "无")
                        {
                            Prj_str = row[0].ToString();
                            if (!this.cbFilter0.Items.Contains(Prj_str))
                                cbFilter0.Items.Add(Prj_str);
                            break;
                        }
                    }
                    

                    #endregion
                    if (!this.cbFilter1.Items.Contains(row[1]))
                        this.cbFilter1.Items.Add(row[1]);

                    #region 水平条件的添加
                    if (this.cbFilter2.Items.Count == 0)
                    {
                        string str = "";
                        switch (int.Parse(row[2].ToString()))
                        {
                            case 1:
                                str = "-枯水年";
                                break;
                            case 2:
                                str = "-平水年";
                                break;
                            case 4:
                                str = "-丰水年";
                                break;
                            case 8:
                                str = "-特枯年";
                                break;
                            case 16:
                                str = "-特丰年";
                                break;
                        }
                        this.cbFilter2.Items.Add(row[2] + str);
                    }
                    else
                    {
                        bool temp = true;
                        foreach (object s in this.cbFilter2.Items)
                        {
                            if (s.ToString().StartsWith(row[2].ToString()))
                            {
                                temp = false;
                                break;
                            }
                            
                         }
                        if (temp)
                        {
                            string str = "";
                            switch (int.Parse(row[2].ToString()))
                            {
                                case 1:
                                    str = "-枯水年";
                                    break;
                                case 2:
                                    str = "-平水年";
                                    break;
                                case 4:
                                    str = "-丰水年";
                                    break;
                                case 8:
                                    str = "-特枯年";
                                    break;
                                case 16:
                                    str = "-特丰年";
                                    break;
                            }
                            this.cbFilter2.Items.Add(row[2] + str);
                        }

                    }
                    #endregion

                    #region 系统或分区、电站的添加
                    if (labelFilter3.Text == "系统或分区")
                    {
                        if (this.cbFilter3.Items.Count == 0)
                        {
                            DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                            foreach (DataRow dr in rows)
                            {
                                if (dr[0].Equals(row[3]))
                                {
                                    this.cbFilter3.Items.Add(row[3] +"-"+ dr[1].ToString());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bool flag = true;
                            foreach(object s in  this.cbFilter3.Items)
                            {
                                if(s.ToString().StartsWith(row[3].ToString()))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                foreach(DataRow dr in rows)
                                {
                                    if(dr[0].Equals(row[3]))
                                    {
                                        this.cbFilter3.Items.Add(row[3]+"-"+dr[1].ToString());
                                        break;
                                    }
                                }
                                
                            }
                        }
                    }
                    else if(labelFilter3.Text == "电站")
                    {
                        DataView rows = OutDS.Tables["系统表"].DefaultView;
                        rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
                        if(!this.cbFilter3.Items.Contains(int.Parse(row[3].ToString())+ "-"+rows[int.Parse(row[3].ToString())]["节点名称"].ToString()))
                        {
                            this.cbFilter3.Items.Add(int.Parse(row[3].ToString())+ "-"+rows[int.Parse(row[3].ToString())]["节点名称"].ToString());
                        }

                    }
                    #endregion

                    #region 日类型添加
                    switch (tableId)
                    {
                        case "PPL":
                        case "PLD":
                        case "GEN":
                        case "HST":
                            DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                            if (this.cbFilter4.Items.Count == 0)
                            {
                                
                                foreach (DataRow dr in rows)
                                {
                                    if (dr[0].Equals(row[4]))
                                    {
                                        this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bool flag = true;
                                foreach (object s in this.cbFilter4.Items)
                                {
                                    if (s.ToString().StartsWith(row[4].ToString()))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    //DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                    foreach (DataRow dr in rows)
                                    {
                                        if (dr[0].Equals(row[4]))
                                        {
                                            this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                            break;
                                        }
                                    }

                                }
                            }
                            if(this.cbFilter4.Items.Count >= rows.Count())
                            {

                                if (int.Parse(row[4].ToString()) == rows.Count() && !this.cbFilter4.Items.Contains(row[4] + "-" + "周一"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周一");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 1 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周二"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周二");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 2 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周三"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周三");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 3 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周四"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周四");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 4 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周五"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周五");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 5 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周六"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周六");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 6 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周日"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周日");

                            }
                            break;
                        case "ENS":
                        case "ENG":
                        case "TEC":
                        case "TRK":
                            string str = "";

                            if (int.Parse(row[4].ToString()) == 0)
                                str = row[4].ToString() + "-" + "最大负荷日合计";
                            else if (int.Parse(row[4].ToString()) == 1)
                                str = row[4].ToString() + "-" + "年总计";
                            if (!this.cbFilter4.Items.Contains(str))
                                this.cbFilter4.Items.Add(str);
                            break;
                    }
                    #endregion

                }
                cbFilter0.SelectedIndex = 0;
                cbFilter1.SelectedIndex = 0;
                cbFilter2.SelectedIndex = 0;
                cbFilter3.SelectedIndex = 0;
                cbFilter4.SelectedIndex = 0;
                #endregion
            }
        }

        #region      ****************自定义表格右键菜单*****************
        private void 在选中单元格前插入一行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.Rows.Insert(this.dgvCreated.SelectedCells[0].RowIndex,1);
        }

        private void 在选中单元格后插入一行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.Rows.Insert(this.dgvCreated.SelectedCells[0].RowIndex+1, 1);
        }

        private void 在选中单元格前插入一列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            this.dgvCreated.Columns.Insert(this.dgvCreated.SelectedCells[0].ColumnIndex,column);
        }

        private void 在选中单元格后插入一列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            this.dgvCreated.Columns.Insert(this.dgvCreated.SelectedCells[0].ColumnIndex+1, column);
        }

        private void 删除单元格所在行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in this.dgvCreated.SelectedCells)
            {
                dgvCreated.BeforeRowRemove(cell.RowIndex); //先处理好单元格 链表 的断裂问题
                dgvCreated.Rows.RemoveAt(cell.RowIndex);
            }


        }

        private void 删除单元格所在列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in this.dgvCreated.SelectedCells)
            {
                dgvCreated.BeforeColumnRemove(cell.ColumnIndex); //先处理好单元格 链表 的断裂问题
                dgvCreated.Columns.RemoveAt(cell.ColumnIndex);
            }
        }

        private void 设置为求和列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellExMessage exmsg=new CellExMessage();
            exmsg.id = "__SUM__";
            this.dgvCreated.SelectedCells[0].Tag = exmsg;

            this.dgvCreated.SelectedCells[0].Value = "Sum";
        }

        private void 合并选中单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.MergeDataGridViewCell();
        }

        private void 取消合并单元格序列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvCreated.UnmergeDataGridViewCell(dgvCreated.SelectedCells[0]);
        }

        private void 将所选列添加到自定义表格所选单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dgvCreated.SelectedCells[0].RowIndex;
            int columnIndex = this.dgvCreated.SelectedCells[0].ColumnIndex;

            //添加列头
            for (int i = 0; i < this.dgvColumn.Columns.Count; i++)
            {
                if (dgvColumn[i, 1].Selected || dgvColumn[i,0].Selected)
                {
                    //注意事项：
                    //设置附加信息，这附加信息也是设置到单元格的Tag属性里，
                    //而合并单元格的一些额外信息也是设置到这里，所以要注意这样的单元格是不能成为合并单元格的
                    //否则会导致信息丢失
                    CellExMessage exmsg=new CellExMessage();
                    exmsg.filterIndex = ("filter" + this.filterCount).ToString(); 
                    exmsg.fileName = this.tbSourceFile.Text;
                    exmsg.tableName = this.dgvSourceTable[1, this.dgvSourceTable.SelectedCells[0].RowIndex].Value.ToString() ;
                    exmsg.filter_0 = this.cbFilter0.Text.Split('-')[0];
                    exmsg.filter_1 = this.cbFilter1.Text;

                    string[] str = this.cbFilter2.Text.Split('-');
                    exmsg.filter_2 = str[0];
                    str = this.cbFilter3.Text.Split('-');
                    exmsg.filter_3 = str[0];
                    str = this.cbFilter4.Text.Split('-');
                    exmsg.filter_4 = str[0];

                    exmsg.id = dgvColumn[i, 1].Value.ToString();
                    dgvCreated[columnIndex, rowIndex].Tag = exmsg;

                    dgvCreated[columnIndex++, rowIndex].Value = dgvColumn[i, 0].Value;

                    if (this.dgvCreated.Columns.Count - 1 == columnIndex - 1)
                    {
                        //列不够
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        this.dgvCreated.Columns.Add(column);
                    }
                }
            }

            //将当前列头单元格  后移
            this.dgvCreated.ClearSelection();
            this.dgvCreated[columnIndex, rowIndex].Selected = true;
            this.filterCount++;
        }

        private void 将所选行添加到自定义表格所选单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dgvCreated.SelectedCells[0].RowIndex;
            int columnIndex = this.dgvCreated.SelectedCells[0].ColumnIndex;

            //添加行头
            for (int i = 0; i < this.dgvRow.Rows.Count; i++)
            {
                if (dgvRow[1, i].Selected || dgvRow[0,i].Selected)
                {
                    //注意事项：
                    //设置附加信息，这附加信息也是设置到单元格的Tag属性里，
                    //而合并单元格的一些额外信息也是设置到这里，所以要注意这样的单元格是不能成为合并单元格的
                    //否则会导致信息丢失
                    CellExMessage exmsg = new CellExMessage();
                    exmsg.id = dgvRow[1, i].Value.ToString();
                    dgvCreated[columnIndex, rowIndex].Tag = exmsg;

                    dgvCreated[columnIndex, rowIndex++].Value = dgvRow[0, i].Value;

                    if (this.dgvCreated.Rows.Count == rowIndex)
                    {//行不够
                        this.dgvCreated.Rows.Add();
                    }
                }
            }

            //将当前列头单元格  后移
            this.dgvCreated.ClearSelection();
            this.dgvCreated[columnIndex, rowIndex].Selected = true;
        }

        private void 将所选列中以行表选中行最大值设置到自定义单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dgvCreated.SelectedCells[0].RowIndex;
            int columnIndex = this.dgvCreated.SelectedCells[0].ColumnIndex;

            CellExMessage exmsg = new CellExMessage();
            exmsg.filterIndex = ("filter" + this.filterCount).ToString();
            exmsg.fileName = this.tbSourceFile.Text;
            exmsg.tableName = this.dgvSourceTable[1, this.dgvSourceTable.SelectedCells[0].RowIndex].Value.ToString();
            exmsg.filter_0 = this.cbFilter0.Text.Split('-')[0];
            exmsg.filter_1 = this.cbFilter1.Text;
            exmsg.filter_2 = this.cbFilter2.Text.Split('-')[0];
            exmsg.filter_3 = this.cbFilter3.Text.Split('-')[0];
            exmsg.filter_4 = this.cbFilter4.Text.Split('-')[0];
            //string str = "MAX_" + dgvRow[1, dgvRow.SelectedCells[0].RowIndex].Value.ToString() + "(";
            string str = "MAX_(";
            //foreach (DataGridViewCell cell in dgvColumn.SelectedCells)
            //{
            //    str += cell.Value.ToString()+",";
            //}

            foreach ( DataGridViewCell cell in dgvColumn.SelectedCells)
            {
                str += dgvColumn[cell.ColumnIndex, 1].Value.ToString() + ",";
            }

            exmsg.id = str.Substring(0, str.Length - 1)+")";
            dgvCreated[columnIndex, rowIndex].Tag = exmsg;

            string valueStr = "MAX_(";
            for(int temp = 0; temp < this.dgvColumn.ColumnCount;temp++)
            {
                if(dgvColumn[temp,1].Selected || dgvColumn[temp,0].Selected)
                {
                    valueStr += dgvColumn[temp, 0].Value + ",";
                }
            }
            valueStr += ")";
            this.dgvCreated.SelectedCells[0].Value = valueStr; ;
            this.filterCount++;
        }

        private void 将所选列中以行表选中行最小值设置到自定义单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dgvCreated.SelectedCells[0].RowIndex;
            int columnIndex = this.dgvCreated.SelectedCells[0].ColumnIndex;

            CellExMessage exmsg = new CellExMessage();
            exmsg.filterIndex = ("filter" + this.filterCount).ToString();
            exmsg.fileName = this.tbSourceFile.Text;
            exmsg.tableName = this.dgvSourceTable[1, this.dgvSourceTable.SelectedCells[0].RowIndex].Value.ToString();
            exmsg.filter_0 = this.cbFilter0.Text.Split('-')[0];
            exmsg.filter_1 = this.cbFilter1.Text;
            exmsg.filter_2 = this.cbFilter2.Text.Split('-')[0];
            exmsg.filter_3 = this.cbFilter3.Text.Split('-')[0];
            exmsg.filter_4 = this.cbFilter4.Text.Split('-')[0];
            //string str = "MIN_" + dgvRow[1, dgvRow.SelectedCells[0].RowIndex].Value.ToString() + "(";
            string str = "MIN_(";
            foreach (DataGridViewCell cell in dgvColumn.SelectedCells)
            {
                str += cell.Value.ToString() + ",";
            }
            exmsg.id = str.Substring(0, str.Length - 1) + ")";
            dgvCreated[columnIndex, rowIndex].Tag = exmsg;


            string valueStr = "MIN_(";
            for (int temp = 0; temp < this.dgvColumn.ColumnCount; temp++)
            {
                if (dgvColumn[temp, 1].Selected || dgvColumn[temp, 0].Selected)
                {
                    valueStr += dgvColumn[temp, 0].Value + ",";
                }
            }
            valueStr += ")";
            this.dgvCreated.SelectedCells[0].Value = valueStr;
            this.filterCount++;
        }

        private void 清空表格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.Rows.Clear();
            this.dgvCreated.Columns.Clear();
            if (btnGetFile.Visible == true)
            {
                DataGridViewTextBoxColumn column = null;
                //初始设置 创建的表格 一行 一列
                for (int i = 0; i < 10; i++)
                {
                    column = new DataGridViewTextBoxColumn();
                    this.dgvCreated.Columns.Add(column);
                }
                for (int i = 0; i < 20; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    this.dgvCreated.Rows.Add(row);
                }
                
            }
            else
            {
                this.cbFilter0.Items.Clear();
                this.cbFilter1.Items.Clear();
                this.cbFilter2.Items.Clear();
                this.cbFilter3.Items.Clear();
                this.cbFilter4.Items.Clear();
                this.Text = "";
                this.tbSourceFile.Text = "";
                this.tbTableName.Text = "";
                this.btnGetFile.Visible = true;
            }
        }

        #endregion

        //找到  表块的  行头 列头行
        //start : 表块 起始 行号
        //end : 表块 结束行号
        private void FindBlockRowColumnHeaderIndex(int start,int end,out int row,out int column)
        {
            row = 0;
            column = 0;
            for (int i = start; i <= end; i++)
            {
                for (int j = 0; j < dgvCreated.Columns.Count - 1; j++)
                {
                    if ((dgvCreated[j, i + 1].Tag != null && dgvCreated[j, i + 1].Tag is CellExMessage) &&
                        (dgvCreated[j + 1, i].Tag != null && dgvCreated[j + 1, i].Tag is CellExMessage))
                    {
                        //循环找到 右 下 单元格都是 CellExMessage的单元格  
                        row = i;
                        column = j;
                        return;
                    }
                    //else if (j==dgvCreated.Columns.Count -1&&(dgvCreated[j, i + 1].Tag != null && dgvCreated[j, i + 1].Tag is CellExMessage))
                    { }
                }
            }
        }

        //保存到xml文件中
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.tbTableName.Text == "")
            {
                MessageBox.Show("自定义表格的名称不能为空，请重新填写。");
                this.DialogResult = DialogResult.None;
                return;
            }

            //检查文件
            string fileName=Application.StartupPath + "\\UserDefinedTables\\"+this.tbTableName.Text+".xml";
            if (File.Exists(fileName))
            {
                if (MessageBox.Show("表格文件已经存在，是否要覆盖？", "警告", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
            
            #region 重新创建表格文件
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write("<UserDefinedTable></UserDefinedTable>");
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                this.DialogResult = DialogResult.None;
                return;
            }
            #endregion

            #region    ****************解析表格 保存到 xml文件********************
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(fileName);
                
                #region 遍历所有的 未合并 列 找到所有文件个Table 以及筛选条件 创建<File>和<Filter>
                XmlElement nodeFile = xmldoc.CreateElement("File");
                XmlElement nodeFilter = xmldoc.CreateElement("Filter");
                #endregion
                #region 遍历所有的行首列  找到所有的块 Block
                for (int i = 0; i < dgvCreated.Rows.Count; i++)
                    {
                        if (dgvCreated[0,i].Tag is LPSP_MergeDGV.CellTagMsg && ((LPSP_MergeDGV.CellTagMsg)(dgvCreated[0, i].Tag)).isMergeFirstCell == true)
                        {
                            //注意DataGridView中访问单元格是dgv[ColumnIndex,RowIndex]
                            //合并序列头, 这一个合并序列就是一个 块，  也就是一个子表
                            //找到这块的 行列头 序号
                            int columnHeaderRowIndex = 0;
                            int rowHeaderColumnIndex = 0;
                            //找到行列头号
                            FindBlockRowColumnHeaderIndex(i, i + dgvCreated.GetMergeCellCount(dgvCreated[0, i]) - 1, out columnHeaderRowIndex, out rowHeaderColumnIndex);


                            XmlElement nodeBlock = xmldoc.CreateElement("Block");
                            nodeBlock.SetAttribute("name", dgvCreated[0, i].Value.ToString());

                            //遍历每一个列头行
                            XmlElement nodeColumnHeaders = xmldoc.CreateElement("ColumnHeaders");
                            for (int rowIndex = i; rowIndex <= columnHeaderRowIndex; rowIndex++)
                            {
                                XmlElement nodeRow = xmldoc.CreateElement("Row");
                                nodeRow.SetAttribute("order", rowIndex.ToString());
                                for (int j = rowHeaderColumnIndex + 1; j < dgvCreated.Columns.Count && dgvCreated[j, rowIndex].Value != null; )
                                {
                                    //每一行添加头  , 到后面的  空白列 就退出循环
                                    XmlElement nodeHead = xmldoc.CreateElement("Head");//普通头
                                    XmlElement nodeHead1 = xmldoc.CreateElement("Head");//文件 头
                                    XmlElement nodeHead2 = xmldoc.CreateElement("Head");//筛选 头
                                    nodeHead.SetAttribute("startOrder", j.ToString());
                                    nodeHead.SetAttribute("throughCells", dgvCreated.GetMergeCellCount(dgvCreated[j, rowIndex]).ToString());
                                    nodeHead.SetAttribute("value", dgvCreated[j, rowIndex].Value.ToString());

                                    CellExMessage exg = new CellExMessage();
                                    
                                    if (rowIndex == columnHeaderRowIndex)
                                    {
                                        
                                        if (((CellExMessage)(dgvCreated[j, rowIndex].Tag)).id != "__SUM__")
                                        {
                                            exg = (CellExMessage)this.dgvCreated[j, rowIndex].Tag;
                                            nodeHead.SetAttribute("id", exg.id);
                                            nodeHead.SetAttribute("filter", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filterIndex);
                                            //到了非合并列头行,将节点加入File节点和
                                            nodeHead1.SetAttribute("name", "file" + fileCount);

                                            nodeHead1.SetAttribute("file", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).fileName);
                                            nodeHead1.SetAttribute("table", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).tableName);


                                            nodeHead2.SetAttribute("name", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filterIndex);
                                            nodeHead2.SetAttribute("file", "file" + fileCount);
                                            fileCount++;
                                            nodeHead2.SetAttribute("filter_0", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filter_0);
                                            nodeHead2.SetAttribute("filter_1", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filter_1);
                                            nodeHead2.SetAttribute("filter_2", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filter_2);
                                            nodeHead2.SetAttribute("filter_3", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filter_3);
                                            nodeHead2.SetAttribute("filter_4", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).filter_4);
                                            //将相应的文件头和筛选头添加到相应的node中
                                            #region 若有重复，不重复添加 此处需要注意后续修改的时候的问题 ---BY GaoYang 2014.04.01
                                            bool flag = false;
                                            foreach (XmlElement test in nodeFile)
                                            {
                                                if (test.Attributes["file"].Value.ToString() == nodeHead1.Attributes["file"].Value.ToString()
                                                    && test.Attributes["table"].Value.ToString() == nodeHead1.Attributes["table"].Value.ToString())
                                                {
                                                    flag = true;
                                                    fileCount--;
                                                }

                                            }
                                            if (!flag)
                                            {
                                                if (nodeFile.IsEmpty)
                                                    fileCount--;
                                                nodeHead1.SetAttribute("name", "file" + fileCount);
                                                nodeFile.AppendChild(nodeHead1);
                                                
                                            }
                                            flag = false;
                                            foreach (XmlElement test in nodeFilter)
                                            {
                                                if (test.Attributes["name"].Value.ToString() == nodeHead2.Attributes["name"].Value.ToString())
                                                {
                                                    /*
                                                    if (test.Attributes["filter_0"].Value.ToString() != nodeHead2.Attributes["filter_0"].Value.ToString())
                                                        test.SetAttribute("filter_0", nodeHead2.Attributes["filter_0"].Value.ToString());
                                                    if (test.Attributes["filter_1"].Value.ToString() != nodeHead2.Attributes["filter_1"].Value.ToString())
                                                        test.SetAttribute("filter_1", nodeHead2.Attributes["filter_1"].Value.ToString());
                                                    if (test.Attributes["filter_2"].Value.ToString() != nodeHead2.Attributes["filter_2"].Value.ToString())
                                                        test.SetAttribute("filter_2", nodeHead2.Attributes["filter_2"].Value.ToString());
                                                    if (test.Attributes["filter_3"].Value.ToString() != nodeHead2.Attributes["filter_3"].Value.ToString())
                                                        test.SetAttribute("filter_3", nodeHead2.Attributes["filter_3"].Value.ToString());
                                                    if (test.Attributes["filter_4"].Value.ToString() != nodeHead2.Attributes["filter_4"].Value.ToString())
                                                        test.SetAttribute("filter_4", nodeHead2.Attributes["filter_4"].Value.ToString());
                                                     */
                                                    flag = true;
                                                    
                                                }
                                            }
                                            if (!flag)
                                            {
                                                nodeHead2.SetAttribute("file","file"+fileCount);
                                                nodeFilter.AppendChild(nodeHead2);
                                                //fileCount--;
                                            }
                                           
                                            
                                            #endregion
                                        }
                                        else 
                                        {
                                            nodeHead.SetAttribute("id", ((CellExMessage)(dgvCreated[j, rowIndex].Tag)).id);
                                        }
                                    }

                                    nodeRow.AppendChild(nodeHead);
                                    j += dgvCreated.GetMergeCellCount(dgvCreated[j, rowIndex]);//向后移动
                                }
                                nodeColumnHeaders.AppendChild(nodeRow);
                                
                            }
                            nodeBlock.AppendChild(nodeColumnHeaders);


                            //遍历每一个行头列
                            XmlElement nodeRowHeaders = xmldoc.CreateElement("RowHeaders");
                            for (int columnIndex = 1; columnIndex <= rowHeaderColumnIndex; columnIndex++)
                            {
                                XmlElement nodeColumn = xmldoc.CreateElement("Column");
                                nodeColumn.SetAttribute("order", columnIndex.ToString());
                                for (int j = columnHeaderRowIndex + 1; j < i + dgvCreated.GetMergeCellCount(dgvCreated[0, i]); )
                                {
                                    //每一行添加头  , 到后面的  空白列 就退出循环
                                    XmlElement nodeHead = xmldoc.CreateElement("Head");
                                    nodeHead.SetAttribute("startOrder", j.ToString());
                                    nodeHead.SetAttribute("throughCells", dgvCreated.GetMergeCellCount(dgvCreated[columnIndex, j]).ToString());
                                    if(dgvCreated[columnIndex,j].Value == null)
                                    {
                                        nodeHead.SetAttribute("value", "");
                                    }
                                    else
                                        nodeHead.SetAttribute("value", dgvCreated[columnIndex, j].Value.ToString());

                                    if (columnIndex == rowHeaderColumnIndex)
                                    {
                                        //到了非合并行头列
                                        if (dgvCreated[columnIndex, j].Value != null)
                                            nodeHead.SetAttribute("id", ((CellExMessage)(dgvCreated[columnIndex, j].Tag)).id);
                                        else
                                            nodeHead.SetAttribute("id","");
                                    }

                                    nodeColumn.AppendChild(nodeHead);

                                    j += dgvCreated.GetMergeCellCount(dgvCreated[columnIndex, j]);//向后移动
                                }
                                nodeRowHeaders.AppendChild(nodeColumn);
                            }
                            nodeBlock.AppendChild(nodeRowHeaders);

                            //插入每一块
                            
                            xmldoc.DocumentElement.AppendChild(nodeBlock);
                            
                        }
                    }
                #endregion
                xmldoc.DocumentElement.AppendChild(nodeFilter);
                xmldoc.DocumentElement.AppendChild(nodeFile);
                
                    //保存
                xmldoc.Save(fileName);
                MessageBox.Show("保存成功！");
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
            #endregion
        }

        public string FilePath = null;
        public string FileName = null;
        private void input_UDT_Click(object sender, EventArgs e)
        {
            
            //private string FilePath = null;
           // private string FileName = null;
            OpenFileDialog openFileDialog = null;
            openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            
            
            FilePath = openFileDialog.FileName;
            FileName = openFileDialog.SafeFileName;
            if (FilePath != "" && FileName != "")
            {
                this.btnGetFile.Visible = false;
                this.btnOK.Text = "保存";
                
                this.Text = FilePath;
                this.tbTableName.Text = FileName.Substring(0, FileName.LastIndexOf("."));
                this.dgvCreated.Rows.Clear();
                this.dgvCreated.Columns.Clear();
                //this.dgvSourceTable.Size = new System.Drawing.Size(0,0);;
                //this.dgvSourceTable.Visible = false;
                //this.dgvColumn.Size = new System.Drawing.Size(0,0);;
                //this.dgvColumn.Visible = false;
                //this.dgvRow.Size = new System.Drawing.Size(0,0);
                //this.dgvRow.Visible = false;
                //this.groupBox3.Size = new Size(1067, 58);
                //this.dgvCreated.Size = new Size(1067, 344);
                this.dgvCreated.Location = new System.Drawing.Point(0, 157);
                

            }
            if (FilePath != "")
            {
                this.contextMenuStripDgvCreated.Items["setFilter"].Visible = false;
                ShowTable();

            }
        }
        /// <summary>
        /// 放弃使用右键菜单，该为使用双击单元格出现筛选条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReSetFilter(object sender, EventArgs e)
        {
            if (dgvCreated.SelectedCells.Count == 1 && dgvCreated.SelectedCells[0].Value != null)
            {
                try
                {
                    CellExMessage exg = (CellExMessage)dgvCreated.SelectedCells[0].Tag;
                    #region 寻找单元格对应的筛选条件和相应的文件
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(FilePath);
                    XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                    foreach (XmlNode element in topM)
                    {
                        if (element.Name == "Filter")
                        {
                            foreach (XmlNode node in element)
                            {
                                if (node.Attributes["name"].Value.ToString() == exg.filterIndex)
                                {
                                    exg.filter_0 = node.Attributes["filter_0"].Value.ToString();
                                    exg.filter_1 = node.Attributes["filter_1"].Value.ToString();
                                    exg.filter_2 = node.Attributes["filter_2"].Value.ToString();
                                    exg.filter_3 = node.Attributes["filter_3"].Value.ToString();
                                    exg.filter_4 = node.Attributes["filter_4"].Value.ToString();
                                    exg.fileIndex = node.Attributes["file"].Value.ToString();
                                    break;
                                }
                            }
                        }
                        else if (element.Name == "File")
                        {
                            foreach (XmlNode node in element)
                            {
                                if (node.Attributes["name"].Value.ToString().Equals(exg.fileIndex))
                                {
                                    exg.fileName = node.Attributes["file"].Value.ToString();
                                    exg.tableName = node.Attributes["table"].Value.ToString();
                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                    this.tbSourceFile.Text = exg.fileName + "-" + exg.tableName;

                    #region comboBox-init
                    this.cbFilter0.Items.Clear();
                    this.cbFilter1.Items.Clear();
                    this.cbFilter2.Items.Clear();
                    this.cbFilter3.Items.Clear();
                    this.cbFilter4.Items.Clear();
                    OutDS.Clear();
                    #endregion

                    #region 加载相应的文件以及筛选条件
                    DataSet ds = new DataSet();
                    ds.ReadXml(exg.fileName);

                    DataTable dt = new DataTable();
                    dt = ds.Tables[exg.tableName];

                    DataSet ds1 = new DataSet();
                    string strTemp = exg.fileName;
                    if (strTemp.EndsWith("_RST.xml") || strTemp.EndsWith("_GEN.xml"))
                    {
                        strTemp = strTemp.Substring(0, strTemp.Length - 8);
                        strTemp += ".xml";
                    }
                    ds1.ReadXml(strTemp);
                    
                    OutDS.Merge(ds1,true);
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!this.cbFilter0.Items.Contains(row[0]))
                        {
                            DataTable dts = OutDS.Tables["方案表"];
                            //增加方案描述，方案的显示也变成了“1-sth”这样的形式    ----2014.07.18 GAO Yang
                            foreach (DataRow r in dts.Rows)
                            {
                                if (row[0].ToString().Equals(r[1].ToString()) && r[2].ToString() != "无")
                                {
                                    string str = row[0].ToString() + "-" + r[2].ToString();
                                    int i=cbFilter0.Items.Add(str);
                                    break;
                                }
                                else if (r[2].ToString() == "无")
                                {
                                    int i=cbFilter0.Items.Add(row[0].ToString());
                                    break;
                                }

                            }
                            //int i = this.cbFilter0.Items.Add(row[0]);
                            //if (row[0].ToString().Equals(exg.filter_0))
                            //{
                            //    this.cbFilter0.SelectedIndex = i;
                            //}
                        }
                        if (!this.cbFilter1.Items.Contains(row[1]))
                        {
                            int i = this.cbFilter1.Items.Add(row[1]);
                            //if (row[1].ToString().Equals(exg.filter_1))
                            //{
                            //    this.cbFilter1.SelectedIndex = i;
                            //}
                        }
                        #region 水平条件的添加
                        if (this.cbFilter2.Items.Count == 0)
                        {
                            string str = "";
                            switch (int.Parse(row[2].ToString()))
                            {
                                case 1:
                                    str = "-枯水年";
                                    break;
                                case 2:
                                    str = "-平水年";
                                    break;
                                case 4:
                                    str = "-丰水年";
                                    break;
                                case 8:
                                    str = "-特枯年";
                                    break;
                                case 16:
                                    str = "-特丰年";
                                    break;
                            }
                            this.cbFilter2.Items.Add(row[2] + str);
                        }
                        else
                        {
                            bool temp = true;
                            foreach (object s in this.cbFilter2.Items)
                            {
                                if (s.ToString().StartsWith(row[2].ToString()))
                                {
                                    temp = false;
                                    break;
                                }

                            }
                            if (temp)
                            {
                                string str = "";
                                switch (int.Parse(row[2].ToString()))
                                {
                                    case 1:
                                        str = "-枯水年";
                                        break;
                                    case 2:
                                        str = "-平水年";
                                        break;
                                    case 4:
                                        str = "-丰水年";
                                        break;
                                    case 8:
                                        str = "-特枯年";
                                        break;
                                    case 16:
                                        str = "-特丰年";
                                        break;
                                }
                                this.cbFilter2.Items.Add(row[2] + str);
                            }

                        }
                        #endregion
                        #region 系统或分区、电站的添加
                        if (labelFilter3.Text == "系统或分区")
                        {
                            if (this.cbFilter3.Items.Count == 0)
                            {
                                DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                foreach (DataRow dr in rows)
                                {
                                    if (dr[0].Equals(row[3]))
                                    {
                                        this.cbFilter3.Items.Add(row[3] + "-" + dr[1].ToString());
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bool flag = true;
                                foreach (object s in this.cbFilter3.Items)
                                {
                                    if (s.ToString().StartsWith(row[3].ToString()))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                    foreach (DataRow dr in rows)
                                    {
                                        if (dr[0].Equals(row[3]))
                                        {
                                            this.cbFilter3.Items.Add(row[3] + "-" + dr[1].ToString());
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        else if (labelFilter3.Text == "电站")
                        {
                            DataView rows = OutDS.Tables["系统表"].DefaultView;
                            rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
                            if (!this.cbFilter3.Items.Contains(int.Parse(row[3].ToString()) + "-" + rows[int.Parse(row[3].ToString())]["节点名称"].ToString()))
                            {
                                this.cbFilter3.Items.Add(int.Parse(row[3].ToString()) + "-" + rows[int.Parse(row[3].ToString())]["节点名称"].ToString());
                            }

                        }
                        #endregion

                        #region 日类型添加
                        switch (exg.tableName)
                        {
                            case "PPL":
                            case "PLD":
                            case "GEN":
                            case "HST":
                                DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                if (this.cbFilter4.Items.Count == 0)
                                {

                                    foreach (DataRow dr in rows)
                                    {
                                        if (dr[0].Equals(row[4]))
                                        {
                                            this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    bool flag = true;
                                    foreach (object s in this.cbFilter4.Items)
                                    {
                                        if (s.ToString().StartsWith(row[4].ToString()))
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        //DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                        foreach (DataRow dr in rows)
                                        {
                                            if (dr[0].Equals(row[4]))
                                            {
                                                this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (this.cbFilter4.Items.Count >= rows.Count())
                                {

                                    if (int.Parse(row[4].ToString()) == rows.Count() && !this.cbFilter4.Items.Contains(row[4] + "-" + "周一"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周一");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 1 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周二"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周二");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 2 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周三"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周三");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 3 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周四"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周四");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 4 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周五"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周五");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 5 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周六"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周六");
                                    else if (int.Parse(row[4].ToString()) == rows.Count() + 6 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周日"))
                                        this.cbFilter4.Items.Add(row[4] + "-" + "周日");

                                }
                                break;
                            case "ENS":
                            case "ENG":
                            case "TEC":
                            case "TRK":
                                string str = "";

                                if (int.Parse(row[4].ToString()) == 0)
                                    str = row[4].ToString() + "-" + "最大负荷日合计";
                                else if (int.Parse(row[4].ToString()) == 1)
                                    str = row[4].ToString() + "-" + "年总计";
                                if (!this.cbFilter4.Items.Contains(str))
                                    this.cbFilter4.Items.Add(str);
                                break;
                        }
                        #endregion
                    }
                    if (this.cbFilter0.Text.Split('-')[0] != exg.filter_0 || this.cbFilter1.Text != exg.filter_1
                        || this.cbFilter2.Text != exg.filter_2 || this.cbFilter3.Text != exg.filter_3
                        || this.cbFilter4.Text != exg.filter_4)
                    {
                        for (int i = 0; ; i++)
                        {

                            if (this.cbFilter0.Items[i].ToString().Split('-')[0] == exg.filter_0)
                            {
                                this.cbFilter0.SelectedIndex = i;
                                break;
                            }
                        }
                        for (int i = 0; ; i++)
                        {

                            if (this.cbFilter1.Items[i].ToString() == exg.filter_1)
                            {
                                this.cbFilter1.SelectedIndex = i;
                                break;
                            }
                        }
                        for (int i = 0; ; i++)
                        {

                            if (this.cbFilter2.Items[i].ToString().StartsWith(exg.filter_2))
                            {
                                this.cbFilter2.SelectedIndex = i;
                                break;
                            }
                        }
                        for (int i = 0; ; i++)
                        {

                            if (this.cbFilter3.Items[i].ToString().StartsWith(exg.filter_3))
                            {
                                this.cbFilter3.SelectedIndex = i;
                                break;
                            }
                        }
                        for (int i = 0; ; i++)
                        {

                            if (this.cbFilter4.Items[i].ToString().StartsWith(exg.filter_4))
                            {
                                this.cbFilter4.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void ShowTable()
        {
            #region 旧版
            /*
            int Block_Start = 0, Block_End = 0;
            int row_count = 0;//记录新插入的行的数目
            int row_index = 0;//记录插入行的索引
            int head_count = 0;//记录这类头的数目
            string is_ColumnHeaders = "ColumnHeaders"; //用来标识是否是新的块的第一个ColumnHeaders的第一个Row 这样可以决定是否进行行插入
            //dgvCreated.DataSource = dt;

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
                                            if (Block_Start >= 1 && Block_End >= 1)
                                            {
                                                for (int i = Block_End; i < Block_Start - 1; i++)
                                                {
                                                    DataGridViewRow r = new DataGridViewRow();
                                                    row_index = dgvCreated.Rows.Add(r);
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
                                                    dgvCreated.Columns.Add(column);
                                                }

                                                //当Row order从非0开始时，插入相应数量的行数
                                                if (int.Parse(sub_node.Attributes["order"].Value) > 0)
                                                {
                                                    row_index = dgvCreated.Rows.Add(int.Parse(sub_node.Attributes["order"].Value) + 1);
                                                    row_count += int.Parse(sub_node.Attributes["order"].Value);

                                                }

                                                //
                                                else if (int.Parse(sub_node.Attributes["order"].Value) == 0)
                                                {
                                                    row_index = this.dgvCreated.Rows.Add(1);
                                                    row_count++;
                                                }
                                            }
                                            if (ssub_node.Attributes["filter"] != null)
                                            {
                                                h1.filter = ssub_node.Attributes["filter"].Value.ToString();
                                                h1.id = ssub_node.Attributes["id"].Value.ToString();
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
                                                 
                                            #endregion
                                             



                                            #region 加入一个Head
                                            if (h1.through_cells > 1)
                                            {
                                                dgvCreated.ClearSelection();
                                                dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;

                                                for (int i = h1.start_order; i < h1.start_order + h1.through_cells; i++)
                                                {
                                                    
                                                    dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[i].Selected = true;

                                                }
                                                dgvCreated.MergeDataGridViewCell();

                                            }
                                            else if (h1.through_cells == 1)
                                            {
                                                if (dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells.Count > h1.start_order)
                                                {
                                                    dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;
                                                    CellExMessage exg = new CellExMessage();
                                                    exg.id = h1.id;
                                                    exg.filterIndex = h1.filter;
                                                    exg.fileName = h1.file;
                                                    exg.tableName = h1.table;
                                                    exg.filter_0 = h1.filter_0;
                                                    exg.filter_1 = h1.filter_1;
                                                    exg.filter_2 = h1.filter_2;
                                                    exg.filter_3 = h1.filter_3;
                                                    exg.filter_4 = h1.filter_4;
                                                    dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Tag = exg; 
                                                }
                                                else
                                                {
                                                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                    this.dgvCreated.Columns.Add(column);
                                                    dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value)].Cells[h1.start_order].Value = h1.value;
                                                }

                                            }
                                            #endregion
                                        }
                                        is_ColumnHeaders = "RowHeaders";
                                        DataGridViewRow row = new DataGridViewRow();
                                        row_index = dgvCreated.Rows.Add(row);
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
                                                row_index = dgvCreated.Rows.Add(r);
                                                row_count++;
                                                h1.start_order = int.Parse(ssub_node.Attributes["startOrder"].Value);
                                                h1.through_cells = int.Parse(ssub_node.Attributes["throughCells"].Value);
                                                h1.value = ssub_node.Attributes["value"].Value;
                                                h1.id = ssub_node.Attributes["id"].Value;
                                                dgvCreated.Rows[h1.start_order].Cells[h1.through_cells].Value = h1.value;
                                                CellExMessage exg = new CellExMessage();
                                                exg.id = h1.id;
                                                dgvCreated.Rows[h1.start_order].Cells[h1.through_cells].Tag = exg;
                                                #region 填入表格数据
                                                //DataView  dv = new DataView();
                                                //dv.Table = dt;
                                                //dv.RowFilter = "Flg = " + h1.value;
                                                //DataTable tmp = new DataTable();
                                                //tmp = dv.ToTable();
                                                /*
                                                int j = 1, count = 0;

                                                for (int i = h1.through_cells + 1; i < dgvCreated.Rows[h1.start_order].Cells.Count; i++)
                                                {
                                                    foreach (DataRow rw in dt.Rows)
                                                    {
                                                        if (rw[count].ToString().Equals(h1.value))
                                                        {
                                                            
                                                            dgvCreated.Rows[h1.start_order].Cells[i].Value = rw[j];
                                                            
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
                            dgvCreated.Rows[Block_Start].Cells[0].Value = nodeTable.Attributes["name"].Value.ToString();
                            dgvCreated.ClearSelection();
                            for (int i = Block_Start; i <= Block_End; i++)
                            {
                                //dgvCreated.Rows[int.Parse(sub_node.Attributes["order"].Value) - 1].Cells[i].Selected = true;
                                dgvCreated.Rows[i].Cells[0].Selected = true;
                            }
                            dgvCreated.MergeDataGridViewCell();
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
            */
            #endregion
            //初始设置 创建的表格 一行 一列
            //for (int i = 0; i < 1; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.ReadOnly = true;
                this.dgvCreated.Columns.Add(column);
            }
            //for (int i = 0; i < 3; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                this.dgvCreated.Rows.Add(row);
            }
            //bool is_SUM = false;//标记表格中是否有求和列，如果有，则置为TRUE，默认没有
            int Block_Start = 0, Block_End = 0;
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
                            foreach (XmlNode r_c_head in nodeTable)
                            {
                                if (r_c_head.Name.Equals("RowHeaders"))
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
                                            if (head.Attributes["value"].Value != "")
                                            {
                                                h1.value = head.Attributes["value"].Value;
                                                h1.id = head.Attributes["id"].Value;
                                            }
                                            else
                                                h1.value = "";
                                            
                                            if (h1.start_order == this.dgvCreated.Rows.Count - 1)
                                            {
                                                this.dgvCreated.Rows.Insert(this.dgvCreated.Rows.Count - 1, 1);
                                            }

                                            this.dgvCreated[columnIndex, h1.start_order].Value = h1.value;
                                            CellExMessage exg = new CellExMessage();
                                            
                                            if(h1.value != null)
                                                exg.id = h1.id;
                                            this.dgvCreated[columnIndex,h1.start_order].Tag = exg;

                                            //#region 填入表格数据
                                            //int j = 1, count = 0;

                                            //for (int i = columnIndex + 1; i < dgvCreated.ColumnCount; i++)
                                            //{
                                            //    if (count >= dt.Columns.Count)
                                            //        break;
                                            //    else
                                            //    {
                                            //        foreach (DataRow rw in dt.Rows)
                                            //        {
                                            //            if (rw[count].ToString().Equals(h1.value))
                                            //            {
                                            //                dgvCreated.Rows[h1.start_order].Cells[i].Value = rw[j];
                                            //                j += 2;
                                            //                count += 2;
                                            //                break;
                                            //            }
                                            //        }
                                            //    }
                                            //}

                                            //#endregion

                                        }
                                    }
                                }
                                else if (r_c_head.Name.Equals("ColumnHeaders"))
                                {

                                    foreach (XmlNode row in r_c_head)
                                    {
                                        if (r_c_head.FirstChild == row)
                                            Block_Start = int.Parse(row.Attributes["order"].Value);
                                        int rowIndex = int.Parse(row.Attributes["order"].Value);
                                        if (rowIndex == this.dgvCreated.Rows.Count - 1)
                                        {
                                            //while(rowIndex>this.dgvCreated.Rows.Count-1)
                                            this.dgvCreated.Rows.Add(1);

                                        }
                                        //else
                                        {
                                            foreach (XmlNode head in row)
                                            {
                                                if (row.ChildNodes.Count >= 1)
                                                {
                                                    h1.start_order = int.Parse(head.Attributes["startOrder"].Value);
                                                    h1.through_cells = int.Parse(head.Attributes["throughCells"].Value);
                                                    //if (head.Attributes["value"].Value != null)
                                                        h1.value = head.Attributes["value"].Value;
                                                    //else
                                                        //h1.value = "";
                                                    
                                                    if (h1.through_cells > 1)
                                                    {
                                                        if (h1.start_order + h1.through_cells > this.dgvCreated.ColumnCount)
                                                        {
                                                            int temp = this.dgvCreated.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp; i++)
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                //column.ReadOnly = true;
                                                                this.dgvCreated.Columns.Add(column);
                                                            }
                                                        }
                                                        //else
                                                        {
                                                            this.dgvCreated.ClearSelection();
                                                            for (int i = h1.start_order; i < h1.start_order + h1.through_cells; i++)
                                                                this.dgvCreated[i, rowIndex].Selected = true;
                                                            this.dgvCreated[h1.start_order, rowIndex].Value = h1.value;
                                                            this.dgvCreated.MergeDataGridViewCell();
                                                        }
                                                    }
                                                    else if (h1.through_cells == 1 && head.Attributes["id"].Value != "__SUM__")
                                                    {
                                                        if (h1.start_order + h1.through_cells > this.dgvCreated.ColumnCount)
                                                        {
                                                            int temp = this.dgvCreated.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp; i++)
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                //column.ReadOnly = true;

                                                                this.dgvCreated.Columns.Add(column);
                                                            }

                                                        }
                                                        //else
                                                        {

                                                            h1.id = head.Attributes["id"].Value;
                                                            h1.filter = head.Attributes["filter"].Value;
                                                            this.SetFilter(h1);
                                                            CellExMessage exg = new CellExMessage();
                                                            exg.id = h1.id;
                                                            exg.filterIndex = h1.filter;
                                                            exg.fileName = h1.file;
                                                            exg.tableName = h1.table;
                                                            exg.filter_0 = h1.filter_0;
                                                            exg.filter_1 = h1.filter_1;
                                                            exg.filter_2 = h1.filter_2;
                                                            exg.filter_3 = h1.filter_3;
                                                            exg.filter_4 = h1.filter_4;
                                                            this.dgvCreated[h1.start_order, rowIndex].Tag = exg;
                                                            this.dgvCreated[h1.start_order, rowIndex].Value = h1.value;
                                                        }
                                                        //确定筛选条件
                                                        //this.SetFilter(h1);
                                                        //读入数据到DataTable
                                                        //this.CreateDataTable(dt, h1);

                                                    }
                                                    else if (h1.through_cells == 1 && head.Attributes["id"].Value == "__SUM__")
                                                    {
                                                        h1.id = head.Attributes["id"].Value;
                                                        //is_SUM = true;
                                                        if (h1.start_order + h1.through_cells > this.dgvCreated.ColumnCount)
                                                        {
                                                            int temp = this.dgvCreated.ColumnCount;
                                                            for (int i = 0; i < h1.start_order + h1.through_cells - temp; i++)
                                                            {
                                                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                                                column.ReadOnly = true;
                                                                this.dgvCreated.Columns.Add(column);
                                                            }

                                                        }
                                                        CellExMessage exg = new CellExMessage();
                                                        exg.id = h1.id;
                                                        this.dgvCreated[h1.start_order, rowIndex].Tag = exg;
                                                        this.dgvCreated[h1.start_order, rowIndex].Value = h1.value;
                                                        
                                                    }
                                                }
                                                else
                                                { continue; }
                                            }
                                        }
                                    }
                                }
                            }

                            this.dgvCreated.ClearSelection();
                            for (int i = Block_Start; i <= Block_End; i++)
                                this.dgvCreated[0, i].Selected = true;
                            this.dgvCreated[0, Block_Start].Value = nodeTable.Attributes["name"].Value;
                            this.dgvCreated.MergeDataGridViewCell();
                            dt.Rows.Clear();
                            dt.Columns.Clear();
                            //is_SUM = false;
                        }
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
        }

        private void SetFilter(Head h1)
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

        //修改筛选条件并映射到相应的地方
        private void Deal_Click(object sender, EventArgs e)
        {
            if (this.cbFilter0.Items.Count == 0)
                return;
            CellExMessage exg = new CellExMessage();
            exg = (CellExMessage)this.dgvCreated.SelectedCells[0].Tag;
            exg.filter_0 = this.cbFilter0.SelectedItem.ToString();
            exg.filter_1 = this.cbFilter1.SelectedItem.ToString();
            exg.filter_2 = this.cbFilter2.SelectedItem.ToString();
            exg.filter_3 = this.cbFilter3.SelectedItem.ToString();
            exg.filter_4 = this.cbFilter4.SelectedItem.ToString();
            string[] s_temp = exg.filter_2.Split('-');
            exg.filter_2 = s_temp[0];
            s_temp = exg.filter_3.Split('-');
            exg.filter_3 = s_temp[0];
            s_temp = exg.filter_4.Split('-');
            exg.filter_4 = s_temp[0];


            this.dgvCreated.SelectedCells[0].Tag = exg;
            
            XmlDocument temp = new XmlDocument();
            temp.Load(FilePath);
            XmlNodeList topM = temp.DocumentElement.ChildNodes;
            foreach (XmlElement element in topM)
            {
                if (element.Name == "Filter")
                {
                    foreach (XmlNode node in element)
                    {
                        if (node.Attributes["name"].Value.ToString() == exg.filterIndex)
                        {
                            //((XmlElement)node).SetAttribute["filter_0", exg.filter_0];

                            node.Attributes["filter_0"].Value = exg.filter_0;
                            node.Attributes["filter_1"].Value = exg.filter_1;
                            //string[] str = this.cbFilter2.Text.Split('-');
                            //exg.filter_2 = str[0];
                            node.Attributes["filter_2"].Value = exg.filter_2;
                            //str = this.cbFilter3.Text.Split('-');
                            //exg.filter_3 = str[0];
                            node.Attributes["filter_3"].Value = exg.filter_3;
                            //str = this.cbFilter4.Text.Split('-');
                            //exg.filter_4 = str[0];
                            node.Attributes["filter_4"].Value = exg.filter_4;
                            //this.dgvCreated.SelectedCells[0].Tag = exg;
                            temp.Save(FilePath);
                            break;
                        }
                    }
                }

            }
            
            this.cbFilter0.Items.Clear();
            this.cbFilter0.Text = "";
            this.cbFilter1.Items.Clear();
            this.cbFilter1.Text = "";
            this.cbFilter2.Items.Clear();
            this.cbFilter2.Text = "";
            this.cbFilter3.Items.Clear();
            this.cbFilter3.Text = "";
            this.cbFilter4.Items.Clear();
            this.cbFilter4.Text = "";

            int rowIndex = 0, columnIndex = 0;
            for (rowIndex = 0; rowIndex < this.dgvCreated.Rows.Count; rowIndex++)
                for (columnIndex = 0; columnIndex < this.dgvCreated.Columns.Count; columnIndex++)
                {
                    if (this.dgvCreated[columnIndex, rowIndex].Value != null && this.dgvCreated[columnIndex, rowIndex].Tag is CellExMessage)
                    {
                        if (((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filterIndex == ((CellExMessage)this.dgvCreated.SelectedCells[0].Tag).filterIndex)
                        {
                            //exg.id = ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).id;
                            ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filter_0 = exg.filter_0;
                            ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filter_1 = exg.filter_1;
                            ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filter_2 = exg.filter_2;
                            ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filter_3 = exg.filter_3;
                            ((CellExMessage)this.dgvCreated[columnIndex, rowIndex].Tag).filter_4 = exg.filter_4;
                        }
                    }
                }

        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            this.btnOK_Click(sender,e);
            if (this.tbTableName.Text.ToString() == "")
                return;
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);
            ReadUDT showUDT = new ReadUDT();
            showUDT.Owner = this;
            showUDT.StartPosition = FormStartPosition.CenterScreen;
            if (this.input_UDT.Visible == true)
            {
                showUDT.FilePath = this.FilePath;
                showUDT.FileName = this.FileName.Substring(0, this.FileName.Length - 4);
            }
            else
            {
                showUDT.FilePath = Application.StartupPath + "\\UserDefinedTables\\" + this.tbTableName.Text + ".xml";
                showUDT.FileName = this.tbTableName.Text;
            }
            showUDT.ShowTable();
            this.myprogress.isOver = true;
            //showUDT.ParentForm = this;
            
            showUDT.ShowDialog();
            
        }

        #region 新添加按钮点击功能
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGetFile_Click_1(object sender, EventArgs e)
        {
            this.btnGetFile_Click(sender, e);
        }

        private void input_UDT_Click_1(object sender, EventArgs e)
        {
            this.input_UDT_Click(sender, e);
        }

        private void Deal_Click_1(object sender, EventArgs e)
        {
            this.Deal_Click(sender, e);
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            this.btnOK_Click(sender, e);
            //this.buttonX1_Click(sender,e);
        }

        private void btn_Create_Click_1(object sender, EventArgs e)
        {
            this.btn_Create_Click(sender , e);
        }
        #endregion

        //初始化
        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.清空表格ToolStripMenuItem_Click(null,null);
            this.dgvColumn.Rows.Clear();
            this.dgvColumn.Columns.Clear();
            this.dgvCreated.Rows.Clear();
            this.dgvCreated.Columns.Clear();
            this.dgvRow.Rows.Clear();
            this.dgvRow.Columns.Clear();
            this.dgvSourceTable.Rows.Clear();
            this.cbFilter0.Items.Clear();
            this.cbFilter1.Items.Clear();
            this.cbFilter2.Items.Clear();
            this.cbFilter3.Items.Clear();
            this.cbFilter4.Items.Clear();
            this.cbFilter0.Text = "";
            this.cbFilter1.Text = "";
            this.cbFilter2.Text = "";
            this.cbFilter3.Text = "";
            this.cbFilter4.Text = "";
            this.Deal.Visible = true;
            this.input_UDT.Visible = true;
            this.btn_Create.Visible = true;
            OutDS.Clear();
            this.tbSourceFile.Text = "";
            this.contextMenuStripDgvCreated.Items["setFilter"].Visible = true;
            this.tbTableName.Text = "";
        }

        //双击修改筛选条件
        private void CellDouble_Click(object sender, DataGridViewCellMouseEventArgs e)
        {
            //LPSP_MergeDGV.CellTagMsg ex = (LPSP_MergeDGV.CellTagMsg)dgvCreated[e.ColumnIndex, e.RowIndex].Tag;
            //bool flag = true;
            //通过判断连续一行或者一列的两个单元格值是否相等来判断是否为合并单元格
            string tag = "";
            if(dgvCreated[e.ColumnIndex, e.RowIndex].Tag!=null)
                 tag = dgvCreated[e.ColumnIndex, e.RowIndex].Tag.ToString();
            if (tag == "HUST_OutPut.CellExMessage")
            {
                CellExMessage exg = (CellExMessage)dgvCreated[e.ColumnIndex, e.RowIndex].Tag;
                Res_Filter(exg);
            }
        }
        //具体完成修改筛选条件的工作
        private void Res_Filter(CellExMessage exg)
        {
            if (exg.filter_0 != null)
            {
                #region 寻找单元格对应的筛选条件和相应的文件
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(FilePath);
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode element in topM)
                {
                    if (element.Name == "Filter")
                    {
                        foreach (XmlNode node in element)
                        {
                            if (node.Attributes["name"].Value.ToString() == exg.filterIndex)
                            {
                                exg.filter_0 = node.Attributes["filter_0"].Value.ToString();
                                exg.filter_1 = node.Attributes["filter_1"].Value.ToString();
                                exg.filter_2 = node.Attributes["filter_2"].Value.ToString();
                                exg.filter_3 = node.Attributes["filter_3"].Value.ToString();
                                exg.filter_4 = node.Attributes["filter_4"].Value.ToString();
                                exg.fileIndex = node.Attributes["file"].Value.ToString();
                                break;
                            }
                        }
                    }
                    else if (element.Name == "File")
                    {
                        foreach (XmlNode node in element)
                        {
                            if (node.Attributes["name"].Value.ToString().Equals(exg.fileIndex))
                            {
                                exg.fileName = node.Attributes["file"].Value.ToString();
                                exg.tableName = node.Attributes["table"].Value.ToString();
                                break;
                            }
                        }
                    }
                }

                #endregion
                this.tbSourceFile.Text = exg.fileName + "-" + exg.tableName;

                #region comboBox-init
                this.cbFilter0.Items.Clear();
                this.cbFilter1.Items.Clear();
                this.cbFilter2.Items.Clear();
                this.cbFilter3.Items.Clear();
                this.cbFilter4.Items.Clear();
                OutDS.Clear();
                #endregion

                #region 加载相应的文件以及筛选条件
                DataSet ds = new DataSet();
                ds.ReadXml(exg.fileName);

                DataTable dt = new DataTable();
                dt = ds.Tables[exg.tableName];

                DataSet ds1 = new DataSet();
                string strTemp = exg.fileName;
                if (strTemp.EndsWith("_RST.xml") || strTemp.EndsWith("_GEN.xml"))
                {
                    strTemp = strTemp.Substring(0, strTemp.Length - 8);
                    strTemp += ".xml";
                }
                else
                    return;
                ds1.ReadXml(strTemp);

                OutDS.Merge(ds1, true);
                foreach (DataRow row in dt.Rows)
                {
                    if (!this.cbFilter0.Items.Contains(row[0]))
                    {
                        DataTable dts = OutDS.Tables["方案表"];
                        //增加方案描述，方案的显示也变成了“1-sth”这样的形式    ----2014.07.18 GAO Yang
                        foreach (DataRow r in dts.Rows)
                        {
                            if (row[0].ToString().Equals(r[1].ToString()) && r[2].ToString() != "无")
                            {
                                string str = row[0].ToString() + "-" + r[2].ToString();
                                int i = cbFilter0.Items.Add(str);
                                break;
                            }
                            else if (r[2].ToString() == "无")
                            {
                                int i = cbFilter0.Items.Add(row[0].ToString());
                                break;
                            }

                        }
                    }
                    if (!this.cbFilter1.Items.Contains(row[1]))
                    {
                        int i = this.cbFilter1.Items.Add(row[1]);
                        //if (row[1].ToString().Equals(exg.filter_1))
                        //{
                        //    this.cbFilter1.SelectedIndex = i;
                        //}
                    }
                    #region 水平条件的添加
                    if (this.cbFilter2.Items.Count == 0)
                    {
                        string str = "";
                        switch (int.Parse(row[2].ToString()))
                        {
                            case 1:
                                str = "-枯水年";
                                break;
                            case 2:
                                str = "-平水年";
                                break;
                            case 4:
                                str = "-丰水年";
                                break;
                            case 8:
                                str = "-特枯年";
                                break;
                            case 16:
                                str = "-特丰年";
                                break;
                        }
                        this.cbFilter2.Items.Add(row[2] + str);
                    }
                    else
                    {
                        bool temp = true;
                        foreach (object s in this.cbFilter2.Items)
                        {
                            if (s.ToString().StartsWith(row[2].ToString()))
                            {
                                temp = false;
                                break;
                            }

                        }
                        if (temp)
                        {
                            string str = "";
                            switch (int.Parse(row[2].ToString()))
                            {
                                case 1:
                                    str = "-枯水年";
                                    break;
                                case 2:
                                    str = "-平水年";
                                    break;
                                case 4:
                                    str = "-丰水年";
                                    break;
                                case 8:
                                    str = "-特枯年";
                                    break;
                                case 16:
                                    str = "-特丰年";
                                    break;
                            }
                            this.cbFilter2.Items.Add(row[2] + str);
                        }

                    }
                    #endregion
                    #region 系统或分区、电站的添加
                    if (labelFilter3.Text == "系统或分区")
                    {
                        if (this.cbFilter3.Items.Count == 0)
                        {
                            DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                            foreach (DataRow dr in rows)
                            {
                                if (dr[0].Equals(row[3]))
                                {
                                    this.cbFilter3.Items.Add(row[3] + "-" + dr[1].ToString());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bool flag = true;
                            foreach (object s in this.cbFilter3.Items)
                            {
                                if (s.ToString().StartsWith(row[3].ToString()))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                foreach (DataRow dr in rows)
                                {
                                    if (dr[0].Equals(row[3]))
                                    {
                                        this.cbFilter3.Items.Add(row[3] + "-" + dr[1].ToString());
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else if (labelFilter3.Text == "电站")
                    {
                        DataView rows = OutDS.Tables["系统表"].DefaultView;
                        rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
                        if (!this.cbFilter3.Items.Contains(int.Parse(row[3].ToString()) + "-" + rows[int.Parse(row[3].ToString())]["节点名称"].ToString()))
                        {
                            this.cbFilter3.Items.Add(int.Parse(row[3].ToString()) + "-" + rows[int.Parse(row[3].ToString())]["节点名称"].ToString());
                        }

                    }
                    #endregion

                    #region 日类型添加
                    switch (exg.tableName)
                    {
                        case "PPL":
                        case "PLD":
                        case "GEN":
                        case "HST":
                            DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                            if (this.cbFilter4.Items.Count == 0)
                            {

                                foreach (DataRow dr in rows)
                                {
                                    if (dr[0].Equals(row[4]))
                                    {
                                        this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bool flag = true;
                                foreach (object s in this.cbFilter4.Items)
                                {
                                    if (s.ToString().StartsWith(row[4].ToString()))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    //DataRow[] rows = OutDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
                                    foreach (DataRow dr in rows)
                                    {
                                        if (dr[0].Equals(row[4]))
                                        {
                                            this.cbFilter4.Items.Add(row[4] + "-" + dr[1].ToString() + "最大负荷日");
                                            break;
                                        }
                                    }

                                }
                            }
                            if (this.cbFilter4.Items.Count >= rows.Count())
                            {

                                if (int.Parse(row[4].ToString()) == rows.Count() && !this.cbFilter4.Items.Contains(row[4] + "-" + "周一"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周一");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 1 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周二"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周二");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 2 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周三"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周三");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 3 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周四"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周四");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 4 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周五"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周五");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 5 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周六"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周六");
                                else if (int.Parse(row[4].ToString()) == rows.Count() + 6 && !this.cbFilter4.Items.Contains(row[4] + "-" + "周日"))
                                    this.cbFilter4.Items.Add(row[4] + "-" + "周日");

                            }
                            break;
                        case "ENS":
                        case "ENG":
                        case "TEC":
                        case "TRK":
                            string str = "";

                            if (int.Parse(row[4].ToString()) == 0)
                                str = row[4].ToString() + "-" + "最大负荷日合计";
                            else if (int.Parse(row[4].ToString()) == 1)
                                str = row[4].ToString() + "-" + "年总计";
                            if (!this.cbFilter4.Items.Contains(str))
                                this.cbFilter4.Items.Add(str);
                            break;
                    }
                    #endregion
                }
                if (this.cbFilter0.Text.Split('-')[0] != exg.filter_0 || this.cbFilter1.Text != exg.filter_1
                    || this.cbFilter2.Text != exg.filter_2 || this.cbFilter3.Text != exg.filter_3
                    || this.cbFilter4.Text != exg.filter_4)
                {
                    for (int i = 0; ; i++)
                    {

                        if (this.cbFilter0.Items[i].ToString().Split('-')[0] == exg.filter_0)
                        {
                            this.cbFilter0.SelectedIndex = i;
                            break;
                        }
                    }
                    for (int i = 0; ; i++)
                    {

                        if (this.cbFilter1.Items[i].ToString() == exg.filter_1)
                        {
                            this.cbFilter1.SelectedIndex = i;
                            break;
                        }
                    }
                    for (int i = 0; ; i++)
                    {

                        if (this.cbFilter2.Items[i].ToString().StartsWith(exg.filter_2))
                        {
                            this.cbFilter2.SelectedIndex = i;
                            break;
                        }
                    }
                    for (int i = 0; ; i++)
                    {

                        if (this.cbFilter3.Items[i].ToString().StartsWith(exg.filter_3))
                        {
                            this.cbFilter3.SelectedIndex = i;
                            break;
                        }
                    }
                    for (int i = 0; ; i++)
                    {

                        if (this.cbFilter4.Items[i].ToString().StartsWith(exg.filter_4))
                        {
                            this.cbFilter4.SelectedIndex = i;
                            break;
                        }
                    }
                }
                #endregion
            }
        }
    }

    public class CellExMessage
    {
        public string filterIndex;      //筛选条件的索引
        public string fileIndex;
        public string fileName;         //文件名
        public string tableName;        //表名
        public string filter;           //单元格的筛选条件
        public string filter_0;
        public string filter_1;
        public string filter_2;
        public string filter_3;
        public string filter_4;

        public string id;               //列ID或者行ID  如果id="__SUM__"，表示 合并列
    }


    public class Head
    {
        public int start_order = 0;  //起始单元格
        public int through_cells; //跨越的单元格数
        public string value;   //列 或者 行 名称
        public string file;    //属于的文件 
        public string table;   //属于的表格
        public string id;      //相应value的id，当该列是求和列时id值为“_SUM_”
        public string filter; //标记列的筛选条件（文件（文件名，表名）、筛选条件（0~4））
        public string filter_0;//筛选条件0~4
        public string filter_1;
        public string filter_2;
        public string filter_3;
        public string filter_4;
    }
}
