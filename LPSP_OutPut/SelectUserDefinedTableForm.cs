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
using ProS_Assm;
using LPSP_MergeDGV;

namespace HUST_OutPut
{
    public partial class SelectUserDefinedTableForm : Form
    {
        DataSet OutDS = new DataSet(); //数据源

        //所有自定义表格键值对  表格名，组合类型
        Dictionary<string, string> table=null;

        //保存用户所选择自定义表的所有信息
        UserDefinedTable curTable = null;


        public progress myprogress;
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();

        }

        public SelectUserDefinedTableForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false; 
            InitializeComponent();

            //创建一个进程显示-加载数据-进度条
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);

            //初始化
            table = new Dictionary<string, string>();
            this.curTable = new UserDefinedTable();

            //获取所有自定义表格，并初始化控件
            GetUserDefinedTable();

            //填充选择表格的组合框
            foreach (KeyValuePair<string, string> kv in table)
            {
                this.cbUserDefinedTable.Items.Add(kv.Key);
            }
            if (this.cbUserDefinedTable.Items.Count > 0)
                this.cbUserDefinedTable.SelectedIndex = 0;
            else
            {
                this.myprogress.isOver = true;
                return;
            }
            this.tbRestructType.Text = table[this.cbUserDefinedTable.Text];

            //从运算结果里读取数据
            ReadOutFiles();

            this.myprogress.isOver = true;
        }

        //读取所有数据源 至 OutDS里
        private void ReadOutFiles()
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
            }
            catch (Exception exc)
            {
                this.myprogress.isOver = true;
                MessageBox.Show("无法读取模拟计算数据！" + exc.Message);
                this.btnOK.Enabled = false;
            }

        }

        //获取所有的用户自定义表格
        private void GetUserDefinedTable()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; //忽略注释

                XmlReader reader = XmlReader.Create(Application.StartupPath + "\\UserDefinedTable.xml", settings);
                xmldoc.Load(reader);
                //得到顶层节点列表
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode element in topM)
                {
                    if (element.Name.ToLower() == "table")
                    {
                        table.Add(element.Attributes["name"].Value + ",id=" +element.Attributes["id"].Value, element.Attributes["restructType"].Value);
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
            
        }

        //选择相应的表格
        private void cbUserDefinedTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果
            if (this.cbUserDefinedTable.SelectedIndex >= 0)
                this.btnOK.Enabled = true;
            else
                this.btnOK.Enabled = false;

            //this.tbRestructType.Text = table[this.cbUserDefinedTable.Text];
        }

        //选择组合类型
        private void tbRestructType_TextChanged(object sender, EventArgs e)
        {
            //this.Text = "colomn";
            //this.Text = "row";
            this.tbRestructType.Text = table[this.cbUserDefinedTable.Text];
        }

        //生成自定义表格
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            //显示进度条
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);

            TableView tableView = new TableView();
            DataTable mytable = SelectTable();  //解析xml表格 并填充数据

            this.myprogress.isOver = true;

            tableView.newTab(mytable,curTable.listMergLink);  //第二个参数表示要合并的表格序列合集
            tableView.Text = "自定义表格";
            tableView.Owner = this;
            tableView.StartPosition = FormStartPosition.CenterScreen;
            tableView.parentForm = this;
            tableView.ShowDialog();
        }

        //解析选择的自定义，并网表格里填充数据
        private DataTable SelectTable()
        {
            DataTable dt = readData(); //解析自定义表格
            writeData(ref dt);         //往表格里填充数据
            return dt;
        }

        //读取xml文件，生成表格 行列头
        private DataTable readData()
        {
            DataTable dt = new DataTable();
            XmlDocument xmldoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true; //忽略注释

            //获取所有的列
            try
            {
                XmlReader reader = XmlReader.Create(Application.StartupPath + "\\UserDefinedTable.xml", settings);
                xmldoc.Load(reader);
                //得到顶层节点列表
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode nodeTable in topM)
                {
                    if (this.cbUserDefinedTable.Text == nodeTable.Attributes["name"].Value + ",id=" + nodeTable.Attributes["id"].Value)
                    {
                        if (nodeTable.Attributes["restructType"].Value.ToLower() == "row")
                        {//行组合表
                        }
                        else if (nodeTable.Attributes["restructType"].Value.ToLower() == "column")
                        {
                            //列组合表
                            /*
                             * 如果 Xml文件 中  不是按照 先 列 后行的顺序，估计这里就会出异常，应为DataTable没有先添加列 就 添加行 会出异常
                             */

                            curTable.id = nodeTable.Attributes["id"].Value;
                            curTable.name = nodeTable.Attributes["name"].Value;
                            curTable.belongTableId = nodeTable.Attributes["belongTableId"].Value;
                            curTable.restructType = nodeTable.Attributes["restructType"].Value;

                            foreach (XmlNode node in nodeTable)
                            {
                                if (node.LocalName == "Columns")
                                {//列
                                    //先添加行头列
                                    DataColumn rowHeadColumn = new DataColumn();
                                    rowHeadColumn.ColumnName = "项目";
                                    dt.Columns.Add(rowHeadColumn);

                                    this.curTable.listColumn.Clear();
                                    //添加数据列 ,先全部添加到内存中，顺便排序
                                    foreach (XmlNode nodeColumn in node)
                                    {
                                        //获取列的筛选 条件 
                                        Column c = new Column();
                                        c.order=int.Parse(nodeColumn.Attributes["order"].Value);
                                        c.refid = nodeColumn.Attributes["refid"].Value;
                                        c.scheme=nodeColumn.Attributes["scheme"].Value;
                                        c.partition = nodeColumn.Attributes["partition"].Value;
                                        c.yearLevel = nodeColumn.Attributes["yearLevel"].Value;
                                        c.dayType = nodeColumn.Attributes["dayType"].Value;
                                        c.hydrateCondition = nodeColumn.Attributes["hydrateCondition"].Value;

                                        this.curTable.listColumn.Add(c);
                                    }
                                    //再添加到 数据表中
                                    foreach (Column c in curTable.listColumn)
                                    {
                                        DataColumn column = new DataColumn();
                                        column.ColumnName = c.refid;  //设置列头

                                        dt.Columns.Add(column);  //这里 本应该 添加到 对应的列，用InsertAt函数。参照 行的添加。 但Xml文件里 是 按序 排好的。所以
                                    }
                                }
                                else if (node.LocalName == "Rows")
                                {//行
                                    this.curTable.listRow.Clear();
                                    //先排序插入到内存
                                    foreach (XmlNode nodeRow in node)
                                    {
                                        Row r = new Row();
                                        r.order = int.Parse(nodeRow.Attributes["order"].Value);
                                        r.refcodeId = nodeRow.Attributes["refcodeId"].Value;

                                        this.curTable.listRow.Capacity = 1000; //默认设置最大1000行，超出了下面的语句就会出 异常
                                        this.curTable.listRow.Add(r);
                                    }
                                    //再排序插入到表格
                                    foreach (Row r in curTable.listRow)
                                    {
                                        DataRow row = dt.NewRow();
                                        //row[0] = CreateUserDefinedTableForm2.GetAllRowName()[r.refcodeId];
                                        row[0] = r.refcodeId;

                                        dt.Rows.InsertAt(row, r.order);
                                    }
                                }
                                else if (node.LocalName == "Headers")
                                {//列头行
                                    this.curTable.listMergLink.Clear();
                                    foreach (XmlNode nodeRow in node)
                                    {
                                        DataRow row = dt.NewRow();
                                        foreach (XmlNode nodeHead in nodeRow)
                                        {
                                            MergeLink mLink = new MergeLink();
                                            mLink.rowIndex = int.Parse(nodeRow.Attributes["order"].Value);
                                            mLink.columnIndex = int.Parse(nodeHead.Attributes["startOrder"].Value);
                                            mLink.value=nodeHead.Attributes["value"].Value;
                                            mLink.count = int.Parse(nodeHead.Attributes["throughCell"].Value);
                                            if(nodeTable.Attributes["restructType"].Value == "column")
                                                mLink.mergeDir = LPSP_MergeDGV.MergeDirector.Row;
                                            else if (nodeTable.Attributes["restructType"].Value == "row")
                                                mLink.mergeDir = LPSP_MergeDGV.MergeDirector.Column;

                                            curTable.listMergLink.Add(mLink); //添加到内存中

                                            row[mLink.columnIndex + 1] = mLink.value;
                                        }
                                        dt.Rows.InsertAt(row,int.Parse(nodeRow.Attributes["order"].Value));//添加列头行
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc) { this.myprogress.isOver = true; MessageBox.Show(exc.Message); }
            dt.TableName = this.curTable.name;
            return dt;
        }

        //往表格里填充数据
        private void writeData(ref DataTable dt)
        {
            //数据源
            DataView dvSource = new DataView();
            dvSource.Table = OutDS.Tables[this.curTable.belongTableId];

            //遍历 dt表的每一个单元格，往里面填充数据，，以列优先的方式遍历
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                //每一列采用相同的 筛选条件，所以以列优先遍历
                dvSource.RowFilter = this.curTable.listColumn[i - 1].GetFilterString(); //dt表的第0列 没有 与listColumn里对应
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int k = 0; k < dvSource.Count; k++)
                    {//遍历数据源中的行，找到对应的行
                        if (dvSource[k]["Flg"].ToString() == dt.Rows[j][0].ToString())
                        {
                            dt.Rows[j][i] = dvSource[k][dt.Columns[i].ColumnName];
                        }
                    }
                }
            }
        }



    }//end of class

    //用户自定义表格类
    class UserDefinedTable
    {
        public string name = "";      //表格名字   
        public string id = "";        //表格ID号
        public string belongTableId = "";  //表格源所属的ID号
        public string restructType = "";   //重组类型

        public List<Column> listColumn = new List<Column>();  //所有列
        public List<Row> listRow = new List<Row>();           //所有行
        public List<MergeLink> listMergLink = new List<MergeLink>();  //所有的合并序列
    }

    //定义列
    class Column
    {
        public int order = 0;                //列序号
        public string refid = "";            //列ID
        public string scheme = "";           //方案
        public string partition = "";        //分区
        public string yearLevel = "";        //水平年
        public string dayType = "";          //日类型
        public string hydrateCondition = "";   //水文条件

        //获取筛选条件
        public string GetFilterString()
        {
            return "Prj = "+scheme+" and Yrs = "+yearLevel+" and Hyd = "+hydrateCondition+" and sID = "+partition+ " and dID = "+dayType;
        }
    }

    //定义行
    class Row
    {
        public int order = 0;            //行序号
        public string refcodeId = "";    //行头标志ID

    }

    //定义合并序列
    public class MergeLink
    {
        //其实单元格 位置
        public int rowIndex = 0;
        public int columnIndex = 0;

        public string value = "";     //序列值

        public int count = 0;          //序列的单元格数
        public LPSP_MergeDGV.MergeDirector mergeDir = LPSP_MergeDGV.MergeDirector.Row;   //序列的合并方向
    }
}//end of namespace
