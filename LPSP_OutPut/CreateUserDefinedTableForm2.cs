using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ProS_Assm;
using System.Threading;
using LPSP_MergeDGV;
//using MergeDataGridView;

namespace LPSP_OutPut
{
    public partial class CreateUserDefinedTableForm2 : Form
    {
        enum DGVROWTYPE{HEAD_ROW,DATA_ROW}; //定义 行  的 标志符

        private string restructType = "";//组合方式
        public Dictionary<string, string> rowNameAndCode = null; //行码与行名对应
        private List<TableTemplate> tableTemplate = new List<TableTemplate>(); //模板表
        private TableTemplate curTemplate = null;       //指示当前选中的 模板表

        //模拟计算结果表
        DataSet OutDS = new DataSet();


        public progress myprogress;
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();

        }


        public CreateUserDefinedTableForm2(string restructType)
        {
            Control.CheckForIllegalCrossThreadCalls = false; 
            InitializeComponent();

            //创建一个进程显示-加载数据-进度条
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);


            this.restructType = restructType;
            if (this.restructType == "row")
            {
                this.Text = "第二步:选择行及筛选条件";
                添加行头列ToolStripMenuItem.Visible = true;
                添加列头行ToolStripMenuItem.Visible = false;
                this.dgvColumn.ClearSelection();
                this.dgvColumn.Enabled = false;
            }
            else if (this.restructType == "column")
            {
                this.Text = "第二步:选择列及筛选条件";
                添加列头行ToolStripMenuItem.Visible = true;
                添加行头列ToolStripMenuItem.Visible = false;
                this.dgvRow.ClearSelection();
                this.dgvRow.Enabled = false;
            }

            //读取模拟计算结果文件   在设置 列或者行 的 筛选条件时  会用到里面的数据
            if (false == ReadOutFiles())//最好放前面 后面的初始化都需要它先初始化
            {
                this.myprogress.isOver = true;
                return;
            }

            //获取所有行号与行码的对应
            rowNameAndCode = CreateUserDefinedTableForm2.GetAllRowName(); //这要放在 cbSourceDgv.SelectedIndex改变事件发生之前

            //获取所有模板
            GetFixedTemplate();
            if (this.cbSourceDgv.Items.Count>0)
                this.cbSourceDgv.SelectedIndex = 0;

            //设置当前的模板表
            SetCurrentTableTemplate();

            //初始化所有的筛选条件 组合框
            ResetComboBox(); //这要放在 源表组合框初始化之后  

            this.myprogress.isOver = true;
        }

        //初始化筛选条件组合框
        private void ResetComboBox()
        {
            //方案
            this.cbScheme.Items.Clear();
            DataTable distinctValues = OutDS.Tables[this.curTemplate.beLongedTableId].DefaultView.ToTable(true, "Prj");
            foreach (DataRow row in distinctValues.Rows)
                this.cbScheme.Items.Add(row[0].ToString());
            if(this.cbScheme.Items.Count>0)
                this.cbScheme.SelectedIndex = 0;

            //分区那有四种类型  根据表格 ID
            switch (this.curTemplate.sysId)
            {
                case "1":
                    labelPartition.Text = "系统及分区:";
                    SysPart();
                    break;
                case "2":
                    labelPartition.Text = "电站名称:";
                    genPart(this.cbPartition);
                    break;
                case "3":
                    labelPartition.Text = "联络线名称:";
                    linePart(this.cbPartition, 3);
                    break;
                case "4":
                    labelPartition.Text = "输电线名称:";
                    linePart(this.cbPartition, 4);
                    break;
                default:
                    break;
            }

            //水平年 ，它会根据 方案的变化而变化
            if (this.cbScheme.Text != "")
            {
                this.cbYearLevel.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[this.curTemplate.beLongedTableId];
                dv.RowFilter = "Prj=" + this.cbScheme.Text;
                distinctValues = dv.ToTable(true, "Yrs");
                foreach (DataRow row in distinctValues.Rows)
                    this.cbYearLevel.Items.Add(row[0].ToString());
                if (this.cbYearLevel.Items.Count > 0)
                    this.cbYearLevel.SelectedIndex = 0;
            }
            else
                MessageBox.Show("筛选方案为空，水平年初始化失败！");

            //日类型
            switch (this.curTemplate.dayId)
            {
                case "1":
                    dayPart1(this.cbDayType);
                    break;
                case "2":
                    dayPart2(this.cbDayType);
                    break;
                default:
                    break;
            }

            //水文条件  根据 水平年的变化而变化
            if (this.cbYearLevel.Text != "")
            {
                this.cbHydrateCondition.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[curTemplate.beLongedTableId];
                dv.RowFilter = "Prj=" + this.cbScheme.Text;
                dv.RowFilter += " and Yrs=" + this.cbYearLevel.Text; ;

                distinctValues = dv.ToTable(true, "Hyd");
                foreach (DataRow row in distinctValues.Rows)
                {
                    string str = row[0].ToString();
                    switch (str)
                    {
                        case "1":
                            str = "枯水年";
                            break;
                        default:
                            str = "其他";
                            break;
                    }
                    this.cbHydrateCondition.Items.Add(str);
                }
                if (this.cbHydrateCondition.Items.Count > 0)
                    this.cbHydrateCondition.SelectedIndex = 0;
            }
        }
        #region  初始化组合框函数
        //如果是系统和分区
        private void SysPart()
        {
            //分区
            this.cbPartition.Items.Clear();
            DataRow[] rows = UniVars.InDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <200");
            foreach (DataRow row in rows)
                this.cbPartition.Items.Add(row["节点名称"].ToString());
            if (this.cbPartition.Items.Count > 0)
                this.cbPartition.SelectedIndex = 0;
        }

        //读电站名称
        private void genPart(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
            for (int index = 0; index < rows.Count; index++)
                comboBox.Items.Add(rows[index]["节点名称"].ToString());
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        //读线路名称
        private void linePart(ComboBox comboBox, int index)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
            if (index == 3)
                rows.RowFilter = "节点类型 in (400,401) and IT01 = 0";
            else if (index == 4)
                rows.RowFilter = "节点类型 in (400,401) and IT01 <> 0";
            else
                rows.RowFilter = "节点类型 >=100 and 节点类型 <200";

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                comboBox.Items.Add(rows[rowIndex]["节点名称"].ToString());
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        //读日类型，有7种的
        private void dayPart1(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter = "节点类型 >=100 and 节点类型 <200";
            for (int index = 0; index < rows.Count; index++)
                comboBox.Items.Add(rows[index]["节点名称"].ToString() + "最大负荷日");
            comboBox.Items.Add("周一");
            comboBox.Items.Add("周二");
            comboBox.Items.Add("周三");
            comboBox.Items.Add("周四");
            comboBox.Items.Add("周五");
            comboBox.Items.Add("周六");
            comboBox.Items.Add("周日");
            comboBox.SelectedIndex = 0;
        }

        //读日类型，有2种的
        private void dayPart2(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("最大负荷日合计");
            comboBox.Items.Add("年总计");
            comboBox.SelectedIndex = 0;
        }
        #endregion

        //获取所有的固定模板
        private void GetFixedTemplate()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Application.StartupPath + "\\TableViewConfig_" + ProS_Assm.UnitMnt.mMode + ".xml");
                //得到顶层节点列表
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode element in topM)
                {
                    if (element.Name.ToLower() == "templete")
                    {
                        foreach (XmlNode el in element.ChildNodes)//读元素值
                        {
                            if (el.Name.ToLower().Equals("item"))
                            {
                                this.cbSourceDgv.Items.Add(el.Attributes["name"].Value);
                                TableTemplate t = new TableTemplate();
                                t.name = el.Attributes["name"].Value;
                                t.id = el.Attributes["id"].Value;
                                //每个模板至少有行或者列，所以用下标0
                                t.beLongedTableId = el.ChildNodes[0].ChildNodes[0].Attributes["belongTableId"].Value;

                                //获取模板 sIdType dIdType
                                foreach (XmlNode node in el.ChildNodes)
                                {
                                    if (node.Name.ToLower().Equals("sidtype"))
                                        t.sysId = node.InnerText;
                                    else if (node.Name.ToLower().Equals("didtype"))
                                        t.dayId = node.InnerText;
                                }
                                this.tableTemplate.Add(t);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        //获取所有的行码 与 行名的对应
        static public Dictionary<string,string> GetAllRowName()
        {
            Dictionary<string, string> nameAndCode = new Dictionary<string, string>();
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
                            if (el.Name.ToLower().Equals("item"))
                            {
                                foreach (XmlNode xn in el.ChildNodes)
                                {//进入每一个基本表
                                    if (xn.Name.ToLower() == "rows")
                                    {//基本表行才有行码与行名的对应
                                        foreach (XmlNode node in xn.ChildNodes)
                                        {//行
                                            if (!nameAndCode.ContainsKey(node.Attributes["code"].Value))//如果不存在键值才添加
                                                nameAndCode.Add(node.Attributes["code"].Value,node.Attributes["content"].Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return nameAndCode;
            }
            catch (Exception e) { MessageBox.Show(e.Message); return null; }
        }

        //从模拟计算结果中 读取数据
        private bool ReadOutFiles()
        {
            try
            {
                DataSet ds1 = new DataSet();
                ds1.ReadXml(UniVars.mOutFile + "_RST.xml");
                OutDS.Merge(ds1, true);

                DataSet ds2 = new DataSet();
                ds2.ReadXml(UniVars.mOutFile + "_GEN.xml");
                OutDS.Merge(ds2, true);

                DataSet ds3 = new DataSet();
                ds3.ReadXml(UniVars.mOutFile + "_MAP.xml");
                OutDS.Merge(ds3, true);

                return true;
            }
            catch (Exception exc)
            {
                this.btnOK.Enabled = false;
                MessageBox.Show(exc.Message + "\n这可能是由于还没有选择打开文件导致，请检查！"); 
                return false;  
            }
        }

        //根据组合框选中的模板表名  设置当前模板表类
        private void SetCurrentTableTemplate()
        {
            foreach (TableTemplate t in tableTemplate)
            {
                if (t.name == this.cbSourceDgv.Text)
                {
                    this.curTemplate = t;
                    break;
                }
            }
        }

        //选择不同的固定模板
        private void cbSourceDgv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbSourceDgv.SelectedIndex < 0)
                this.btnOK.Enabled = false;
            this.dgvRow.Rows.Clear();
            this.dgvRow.Columns.Clear();
            this.dgvColumn.Rows.Clear();
            this.dgvColumn.Columns.Clear();


            //根据组合框选中的模板表名  设置当前模板表类
            SetCurrentTableTemplate();

            //刷新  筛选条件组合框
            ResetComboBox();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(Application.StartupPath + "\\TableViewConfig_" + ProS_Assm.UnitMnt.mMode + ".xml");
                //得到顶层节点列表
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode element in topM)
                {
                    if (element.Name.ToLower() == "templete")
                    {
                        foreach (XmlNode el in element.ChildNodes)//读元素值
                        {
                            if (el.Name.ToLower().Equals("item"))
                            {
                                if (el.Attributes["name"].Value == this.cbSourceDgv.Text)
                                {
                                    foreach (XmlNode xn in el.ChildNodes)
                                    {
                                        if (xn.Name.ToLower() == "rows")
                                        {
                                            this.dgvRow.Columns.Add("","");
                                            int i = 0;
                                            foreach (XmlNode node in xn.ChildNodes)
                                            {//行
                                                this.dgvRow.Rows.Add();
                                                this.dgvRow.Rows[i].Tag = node.Attributes["refcodeId"].Value;
                                                this.dgvRow[0,i++].Value=rowNameAndCode[node.Attributes["refcodeId"].Value];
                                            }
                                        }
                                        else if (xn.Name.ToLower() == "columns")
                                        {
                                            foreach (XmlNode node in xn.ChildNodes)
                                            {//列
                                                this.dgvColumn.Columns.Add("", "");     
                                            }
                                            this.dgvColumn.Rows.Add();
                                            int i = 0;
                                            foreach (XmlNode node in xn.ChildNodes)
                                            {//列
                                                this.dgvColumn[i++, 0].Value = node.Attributes["refid"].Value;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
        }

        //方案变化
        private void cbScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            //水平年 ，它会根据 方案的变化而变化
            if (this.cbScheme.Text != "")
            {
                this.cbYearLevel.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[this.curTemplate.beLongedTableId];
                dv.RowFilter = "Prj=" + this.cbScheme.Text;
                DataTable distinctValues = dv.ToTable(true, "Yrs");
                foreach (DataRow row in distinctValues.Rows)
                    this.cbYearLevel.Items.Add(row[0].ToString());
                if (this.cbYearLevel.Items.Count > 0)
                    this.cbYearLevel.SelectedIndex = 0;
            }
            else
                MessageBox.Show("筛选方案为空，水平年初始化失败！");
        }

        //水平年变化
        private void cbYearLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //水文条件  根据 水平年的变化而变化
            if (this.cbYearLevel.Text != "")
            {
                this.cbHydrateCondition.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[curTemplate.beLongedTableId];
                dv.RowFilter = "Prj=" + this.cbScheme.Text;
                dv.RowFilter += " and Yrs=" + this.cbYearLevel.Text; ;

                DataTable distinctValues = dv.ToTable(true, "Hyd");
                foreach (DataRow row in distinctValues.Rows)
                {
                    string str = row[0].ToString();
                    switch (str)
                    {
                        case "1":
                            str += "-枯水年";
                            break;
                        default:
                            str += "-其他";
                            break;
                    }
                    this.cbHydrateCondition.Items.Add(str);
                }
                if (this.cbHydrateCondition.Items.Count > 0)
                    this.cbHydrateCondition.SelectedIndex = 0;
            }
        }

        private bool isRowAndColumnHeaderAdded=false;    //指示 表中行 以及列头是否添加   这二者是同步的
        //添加行 或者 列
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (restructType == "row")
            {
                //已有的列数不够，需要扩增
                if (dgvColumn.Columns.Count > dgvCreated.Columns.Count)
                {
                    for (int i = dgvCreated.Columns.Count; i < dgvColumn.Columns.Count; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.HeaderText = "第" + i.ToString() + "列";
                        //添加到自定义表格里,这是原始列，不可由用户修改
                        column.ReadOnly = true;   //数据列 不可编辑
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;  //不允许 可排序
                        this.dgvCreated.Columns.Add(column);
                    }
                }

                //添加列头行
                DataGridViewRow row = new DataGridViewRow();
                for (int i = 0; i < dgvColumn.Columns.Count; i++)
                {
                    DataGridViewTextBoxCell textboxcell = new DataGridViewTextBoxCell();
                    textboxcell.Value = dgvColumn.Rows[0].Cells[i].Value;
                    row.Cells.Add(textboxcell);
                }
                this.dgvCreated.Rows.Add(row);

                //添加选中的行
                int lastIndex = this.dgvCreated.Rows.Count;//用尾插法
                foreach (DataGridViewCell cell in this.dgvRow.SelectedCells)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    r.HeaderCell.Value = cell.Value;
                    this.dgvCreated.Rows.Insert(lastIndex, r);
                }
                
            }
            else if (restructType == "column")
            {
                #region 列合并 添加
                int lastIndex = this.dgvCreated.Columns.Count;//用尾插法
                foreach (DataGridViewCell cell in this.dgvColumn.SelectedCells)//由于列选择里 总共只设置了一行，所以可以用单元格 代表一列
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.HeaderText = cell.Value.ToString();
                    column.Tag = new TableItem(cell.Value.ToString(), curTemplate.id, this.cbScheme.Text, this.cbPartition.SelectedIndex.ToString(), this.cbYearLevel.Text, this.cbDayType.SelectedIndex.ToString(), (this.cbHydrateCondition.SelectedIndex + 1).ToString());
                    ((TableItem)column.Tag).dayTypeName = this.cbDayType.Text;
                    ((TableItem)column.Tag).partitionName = this.cbPartition.Text;
                    ((TableItem)column.Tag).hydrateConditionName = this.cbHydrateCondition.Text;
                    column.ToolTipText = "筛选条件：" + ((TableItem)column.Tag).GetFilter();
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;  //不允许 可排序
                    //添加到自定义表格里,这是原始列，不可由用户修改
                    this.dgvCreated.Columns.Insert(lastIndex,column);
                }
                if (this.isRowAndColumnHeaderAdded==false)
                {//添加所有行，如果已有就不添加   并 添加一行自定义列头
                    foreach (DataGridViewRow row in this.dgvRow.Rows)
                    {
                        DataGridViewRow r = new DataGridViewRow();
                        r.Tag = row.Tag; //这里保存着 行的  refcodeId
                        r.ReadOnly = true; //数据行是不可编辑的， 只有 列头行  是可以编辑的
                        r.HeaderCell.Value = row.Cells[0].Value;
                        this.dgvCreated.Rows.Add(r);
                    }

                    //列头添加 ,,并允许 用户编辑  列头
                    DataGridViewRow head = new DataGridViewRow();
                    head.Tag = DGVROWTYPE.HEAD_ROW;    //  标志这行 是 列头行
                    for (int i = 0; i < dgvCreated.Columns.Count; i++)
                    {
                        DataGridViewTextBoxCell textboxcell = new DataGridViewTextBoxCell();
                        textboxcell.Value = dgvCreated.Columns[i].HeaderText;
                        head.Cells.Add(textboxcell);
                    }
                    this.dgvCreated.Rows.Insert(0, head);
                    
                    //设置已经添加过了
                    this.isRowAndColumnHeaderAdded = true;
                }
                #endregion
            }
        }
        
        //表格列 增加 或者 减少
        private void dgvCreated_ColumnCountChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (this.restructType == "column")
            {//在列组合模式下，只能选择同类表
                if (this.dgvCreated.Columns.Count > 0)
                    this.cbSourceDgv.Enabled = false;
                else
                    this.cbSourceDgv.Enabled = true;
            }
        }

        //表有任何鼠标点击  主要是处理表头的左键点击事件  选中 整行  或者  整列
        private void dgvCreated_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex == -1 && e.ColumnIndex != -1)
                {
                    this.dgvCreated.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                    this.dgvCreated.Columns[e.ColumnIndex].Selected = true;
                }
                else if (e.RowIndex != -1 && e.ColumnIndex == -1)
                {
                    this.dgvCreated.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    this.dgvCreated.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void 添加列头行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //列头添加 ,,并允许 用户编辑  列头
            if (this.dgvCreated.Columns.Count > 0)
            {
                DataGridViewRow head = new DataGridViewRow();
                head.Tag = DGVROWTYPE.HEAD_ROW;
                this.dgvCreated.Rows.Insert(0, head);
            }
            else
            {
                MessageBox.Show("还没有添加列，无法添加列头行！");
                return;
            }
        }
        private void 添加行头列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //行头添加 ,,并允许 用户编辑 
            DataGridViewTextBoxColumn head = new DataGridViewTextBoxColumn();
            head.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvCreated.Columns.Insert(0, head);
        }
        private void 合并单元格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.MergeDataGridViewCell();
        }
        private void 删除选中行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgvCreated.SelectedRows)
                this.dgvCreated.Rows.Remove(row);
        }
        private void 删除选中列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in this.dgvCreated.SelectedColumns)
                this.dgvCreated.Columns.Remove(column);
        }
        private void 清空预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dgvCreated.Rows.Clear();
            this.dgvCreated.Columns.Clear();
            this.isRowAndColumnHeaderAdded = false;
        }



        //上一步
        private void btnPreStep_Click(object sender, EventArgs e)
        {
            this.Hide();
            CreateUserDefinedTableForm1 form = new CreateUserDefinedTableForm1(this.restructType);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        //生成列组合表
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.restructType == "column")
            {
                #region 列组合
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(Application.StartupPath + "\\UserDefinedTable.xml");
                    //得到顶层节点列表
                    XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                    int maxId = 0;
                    foreach (XmlNode element in topM)
                    {//找到最大表格ID号
                        if (int.Parse(element.Attributes["id"].Value) > maxId)
                            maxId = int.Parse(element.Attributes["id"].Value);
                    }
                    //插入自定义表
                    XmlElement nodeTable = xmldoc.CreateElement("Table");
                    nodeTable.SetAttribute("id", (maxId + 1).ToString());
                    nodeTable.SetAttribute("name", this.tbCreatedDgvName.Text);
                    nodeTable.SetAttribute("belongTableId", this.curTemplate.beLongedTableId);
                    nodeTable.SetAttribute("restructType", this.restructType);

                    //往表节点添加  列
                    XmlElement nodeColumns = xmldoc.CreateElement("Columns");

                    foreach (DataGridViewTextBoxColumn column in this.dgvCreated.Columns)
                    {
                        XmlElement nodeColumn = xmldoc.CreateElement("Column");
                        nodeColumn.SetAttribute("order", column.Index.ToString());
                        nodeColumn.SetAttribute("refid", column.HeaderText);
                        nodeColumn.SetAttribute("scheme", ((TableItem)column.Tag).scheme);
                        nodeColumn.SetAttribute("partition", ((TableItem)column.Tag).partition);
                        nodeColumn.SetAttribute("yearLevel", ((TableItem)column.Tag).yearLevel);
                        nodeColumn.SetAttribute("dayType", ((TableItem)column.Tag).dayType);
                        nodeColumn.SetAttribute("hydrateCondition", ((TableItem)column.Tag).hydrateCondition);

                        nodeColumns.AppendChild(nodeColumn);
                    }
                    nodeTable.AppendChild(nodeColumns);

                    //往表节点添加  行 （包括 数据行 、列头行）
                    XmlElement nodeRows = xmldoc.CreateElement("Rows");
                    XmlElement nodeHeaders = xmldoc.CreateElement("Headers");

                    foreach (DataGridViewRow row in this.dgvCreated.Rows)
                    {
                        XmlElement nodeRow = xmldoc.CreateElement("Row");
                        if (row.Tag != null && row.Tag is DGVROWTYPE && (DGVROWTYPE)row.Tag == DGVROWTYPE.HEAD_ROW)
                        {//列头行
                            nodeRow.SetAttribute("order", row.Index.ToString());
                            for (int i = 0; i < row.Cells.Count; i++)
                            {//遍历列头行 的  所有单元  添加组合列头 
                                if (row.Cells[i].Tag != null && row.Cells[i].Tag is CellTagMsg && ((CellTagMsg)row.Cells[i].Tag).isMergeCell == true)
                                {
                                    //每个合并序列 只添加一次 列头  这是用first标志
                                    if (((CellTagMsg)row.Cells[i].Tag).isMergeFirstCell == true)
                                    {
                                        XmlElement nodeHead = xmldoc.CreateElement("Head");
                                        nodeHead.SetAttribute("startOrder", i.ToString());
                                        nodeHead.SetAttribute("throughCell", this.dgvCreated.GetMergeCellCount(row.Cells[i]).ToString());
                                        nodeHead.SetAttribute("value", row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());

                                        nodeRow.AppendChild(nodeHead);
                                    }
                                }
                                else
                                {
                                    XmlElement nodeHead = xmldoc.CreateElement("Head");
                                    nodeHead.SetAttribute("startOrder", i.ToString());
                                    nodeHead.SetAttribute("throughCell", this.dgvCreated.GetMergeCellCount(row.Cells[i]).ToString());
                                    nodeHead.SetAttribute("value", row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());

                                    nodeRow.AppendChild(nodeHead);
                                }
                            }
                            nodeHeaders.AppendChild(nodeRow);
                        }
                        else
                        {//一般的数据行
                            nodeRow.SetAttribute("order", row.Index.ToString());
                            nodeRow.SetAttribute("refcodeId", row.Tag.ToString());

                            nodeRows.AppendChild(nodeRow);
                        }
                    }

                    nodeTable.AppendChild(nodeRows);
                    nodeTable.AppendChild(nodeHeaders);

                    //添加到顶层表中
                    xmldoc.DocumentElement.AppendChild(nodeTable);

                    //保存
                    xmldoc.Save(Application.StartupPath + "\\UserDefinedTable.xml");
                    MessageBox.Show("保存成功！");
                }
                catch (Exception exc) { MessageBox.Show(exc.Message); }
                #endregion
            }
            else if(this.restructType=="row")
            {
               /*
                #region 行组合

                try 
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(Application.StartupPath + "\\UserDefinedTable.xml");
                    //得到顶层节点列表
                    XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                    int maxId = 0;
                    foreach (XmlNode element in topM)
                    {//找到最大表格ID号
                        if (int.Parse(element.Attributes["id"].Value) > maxId)
                            maxId = int.Parse(element.Attributes["id"].Value);
                    }
                    //插入自定义表
                    XmlElement nodeTable = xmldoc.CreateElement("Table");
                    nodeTable.SetAttribute("id", (maxId + 1).ToString());
                    nodeTable.SetAttribute("name", this.tbCreatedDgvName.Text);
                    nodeTable.SetAttribute("belongTableId", this.curTemplate.beLongedTableId);
                    nodeTable.SetAttribute("restructType", this.restructType);
                    //往表节点添加  行
                    XmlElement nodeRows = xmldoc.CreateElement("Rows");

                    foreach (DataGridViewRow row in this.dgvCreated.Rows)
                    {
                        if (row.Cells.ToString() == "")
                        { continue; }
                        else
                        {
                            XmlElement nodeColumn = xmldoc.CreateElement("Rows");
                            nodeColumn.SetAttribute("order", row.Index.ToString());
                            nodeColumn.SetAttribute("refid", row.HeaderCell.ToString());
                            nodeColumn.SetAttribute("scheme", ((TableItem)row.Tag).scheme);
                            nodeColumn.SetAttribute("partition", ((TableItem)row.Tag).partition);
                            nodeColumn.SetAttribute("yearLevel", ((TableItem)row.Tag).yearLevel);
                            nodeColumn.SetAttribute("dayType", ((TableItem)row.Tag).dayType);
                            nodeColumn.SetAttribute("hydrateCondition", ((TableItem)row.Tag).hydrateCondition);

                            nodeRows.AppendChild(nodeColumn);
                        }
                    }
                 
                    nodeTable.AppendChild(nodeRows);

                    //往表节点添加  列 （包括 数据列 、行头列）
                    XmlElement nodeColumns = xmldoc.CreateElement("Columns");
                    XmlElement nodeHeaders = xmldoc.CreateElement("Headers");

                    foreach (DataGridViewTextBoxColumn column in this.dgvCreated.Columns)
                    {
                        XmlElement nodeRow = xmldoc.CreateElement("Row");
                        if (row.Tag != null && row.Tag is DGVROWTYPE && (DGVROWTYPE)row.Tag == DGVROWTYPE.HEAD_ROW)
                        {//列头行
                            nodeRow.SetAttribute("order", row.Index.ToString());
                            for (int i = 0; i < row.Cells.Count; i++)
                            {//遍历列头行 的  所有单元  添加组合列头 
                                if (row.Cells[i].Tag != null && row.Cells[i].Tag is CellTagMsg && ((CellTagMsg)row.Cells[i].Tag).isMergeCell == true)
                                {
                                    //每个合并序列 只添加一次 列头  这是用first标志
                                    if (((CellTagMsg)row.Cells[i].Tag).isMergeFirstCell == true)
                                    {
                                        XmlElement nodeHead = xmldoc.CreateElement("Head");
                                        nodeHead.SetAttribute("startOrder", i.ToString());
                                        nodeHead.SetAttribute("throughCell", this.dgvCreated.GetMergeCellCount(row.Cells[i]).ToString());
                                        nodeHead.SetAttribute("value", row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());

                                        nodeRow.AppendChild(nodeHead);
                                    }
                                }
                                else
                                {
                                    XmlElement nodeHead = xmldoc.CreateElement("Head");
                                    nodeHead.SetAttribute("startOrder", i.ToString());
                                    nodeHead.SetAttribute("throughCell", this.dgvCreated.GetMergeCellCount(row.Cells[i]).ToString());
                                    nodeHead.SetAttribute("value", row.Cells[i].Value == null ? "" : row.Cells[i].Value.ToString());

                                    nodeRow.AppendChild(nodeHead);
                                }
                            }
                            nodeHeaders.AppendChild(nodeRow);
                        }
                        else
                        {//一般的数据行
                            nodeRow.SetAttribute("order", row.Index.ToString());
                            nodeRow.SetAttribute("refcodeId", row.Tag.ToString());

                            nodeRows.AppendChild(nodeRow);
                        }
                    }

                    nodeTable.AppendChild(nodeRows);
                    nodeTable.AppendChild(nodeHeaders);

                    //添加到顶层表中
                    xmldoc.DocumentElement.AppendChild(nodeTable);

                    //保存
                    xmldoc.Save(Application.StartupPath + "\\UserDefinedTable.xml");
                    MessageBox.Show("保存成功！");
                    

                }
                catch (Exception exc) { MessageBox.Show(exc.Message); }
                #endregion
                * */
            }
        }
    }//end of class
    
    //表格行 或者 列 类的定义
    public class TableItem
    {
        public string name = "";       //行名 或者 列名
        public string belongId = "";   //所属模板表的ID号
        
        //筛选条件
        public string scheme = "";            //方案
        public string partition = "";         //分区   //以数字表示  如0 对应 系统分区
        public string yearLevel = "";         //水平年
        public string dayType = "";           //日类型
        public string hydrateCondition = "";  //水文条件

        public string dayTypeName = "";               //以文本表示
        public string partitionName = "";
        public string hydrateConditionName = "";

        public TableItem()
        {
        }

        public TableItem(string tName = "", string tBelongId = "", string tScheme = "", string tPartition = "", string tYearLevel = "", string tDayType = "", string tHydrateCondition = "")
        {
            this.name = tName;
            this.belongId = tBelongId;
            this.scheme = tScheme;
            this.partition = tPartition;
            this.yearLevel = tYearLevel;
            this.dayType = tDayType;
            this.hydrateCondition = tHydrateCondition;
        }

        public void SetItemAttribute(string tName="",string tBelongId="",string tScheme="",string tPartition="",string tYearLevel="",string tDayType="",string tHydrateCondition="")
        {
            this.name = tName;
            this.belongId = tBelongId;
            this.scheme = tScheme;
            this.partition = tPartition;
            this.yearLevel = tYearLevel;
            this.dayType = tDayType;
            this.hydrateCondition = tHydrateCondition;
        }

        //获取筛选条件
        public string GetFilter()
        {
            return "方案:" + scheme + ",分区:" + partitionName + ",水平年:" + yearLevel + ",日类型:" + dayTypeName + ",水文条件:" + hydrateConditionName;
        }
    }

    //模板表
    public class TableTemplate
    {
        public string name = "";               //表名
        public string id = "";                 //ID号
        public string beLongedTableId = "";    //所属基本表ID

        public string sysId = "";           
        public string dayId = "";

        public TableTemplate()
        {
        }
    }
}//end of namespace
