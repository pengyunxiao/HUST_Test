/*************************************
*  电力平衡模拟功能 
*  Balance of power simulate function
*  Time:2016-3-30
*  Modified by Jiming Yin
**************************************/

using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using LPSP_MergeDGV;
using ProS_Assm;
using DevComponents.DotNetBar.Controls;

namespace HUST_OutPut
{
    public partial class SelectTable : DevComponents.DotNetBar.Office2007Form
    {
        DataSet OutDS = new DataSet();
        DataSet InDS = new DataSet();
        public progress myprogress;
        private TemplateInfo templateInfo = new TemplateInfo();
        private DataTable formDescription = new DataTable();
        private int deciNum;//保存小数位数，只在生成表格的时候使用
        private bool isClosed = false; //保存用户是否要关闭窗口
        /// <summary>
        /// Default initialize function
        /// </summary>
        public SelectTable()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            ////创建一个进程显示-加载数据-进度条
            //Thread thdSub = new Thread(new ThreadStart(this.progressB));
            //thdSub.Start();
            //Thread.Sleep(100);
        }

        /// <summary>
        /// Read Simulation data file function
        /// </summary>
        private void ReadOutFiles()
        {
            try
            {
                if (MyTools.HourWarning(UniVars.mOutFile))
                {
                    isClosed = true;
                    return;
                }

                //由原来构造函数移动到此处，防止提示警告信息与进度条重合
                //创建一个进程显示-加载数据-进度条
                Thread thdSub = new Thread(new ThreadStart(this.progressB));
                thdSub.Start();
                Thread.Sleep(100);

                InDS.ReadXml(UniVars.mOutFile + ".xml");//只是为了方案描述

                DataSet ds1 = new DataSet();
                ds1.ReadXml(UniVars.mOutFile + "_RST.xml");
                OutDS.Merge(ds1, true);

                DataSet ds2 = new DataSet();
                ds2.ReadXml(UniVars.mOutFile + "_GEN.xml");
                OutDS.Merge(ds2, true);
                DataSet ds3 = new DataSet();
                ds3.ReadXml(UniVars.mOutFile + "_MAP.xml");
                OutDS.Merge(ds3, true);

                //以下语句不应该放在这里的
                PrepareFormDescription(); //设置 呈现 所选择表格的DataGridView

                this.TempletesRead();  //加载所有的表格模板
                int histSelctedIndex = GetFirstHistoryRecordID() - 1;//get the default forms type ID
                //set default forms type ID
                if (histSelctedIndex < this.comboBoxEx1.Items.Count)
                    this.comboBoxEx1.SelectedIndex = histSelctedIndex;
                else
                {
                    this.comboBoxEx1.SelectedIndex = 0;
                }
                comboBoxEx1.Enabled = true;
            }
            catch (Exception exc)
            {
                if (this.myprogress != null)
                    this.myprogress.isOver = true;
                MessageBox.Show("无法读取模拟计算数据！" + exc.Message);
            }

        }

        /// <summary>
        /// The respond function of  Form1 loading action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();
            this.Text = UniVars.mOutFile;
            textBoxX1.Text = UniVars.mOutFile;
            if (textBoxX1.Text.Trim() != "")
                ReadOutFiles();//get forms default data
            //如果用户选择终止计算则关闭窗口 添加by孙凯 2016.3.21
            if (isClosed)
            {
                this.Close();
                return;
            }
            myprogress.isOver = true; //进度条设置完成
        }

        /// <summary>
        /// describe the Columns title of forms to show
        /// </summary>
        private void PrepareFormDescription()
        {
            try
            {
                formDescription.Columns.Add("编号", typeof(String));
                formDescription.Columns.Add("表格名称", typeof(String));
                formDescription.Columns.Add("备注", typeof(String));
                dataGridViewX1.DataSource = formDescription;
                dataGridViewX1.Columns[1].FillWeight = 400;
                dataGridViewX1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewX1.Columns[2].FillWeight = 400;
                dataGridViewX1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewX1.AllowUserToAddRows = false;
            }catch(Exception){}
        }

        /// <summary>
        /// Get the Combox Selection of all
        /// </summary>
        /// <returns></returns>
        private ComboBox GetComboBoxWithAllSelected()
        {
            if (comboBoxEx2.Text == "ALL")
                return comboBoxEx2;

            if (comboBoxEx3.Text == "ALL")
                return comboBoxEx3;

            if (comboBoxEx4.Text == "ALL")
                return comboBoxEx4;

            if (comboBoxEx5.Text == "ALL")
                return comboBoxEx5;

            if (comboBoxEx6.Text == "ALL")
                return comboBoxEx6;

            return null;
        }

        /// <summary>
        /// Add form of row description
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="itemIndex"></param>
        private void AddRowToFormDescription(ComboBox cb,int itemIndex)
        {
            DataRow row=formDescription.NewRow();
            formDescription.Rows.Add(row);
            row["编号"] = GenFormNo(formDescription.Rows.Count-1);

            string formName = "";//To get forms name

            string[] subStrs = comboBoxEx4.Text.Split(new char[] { '-' });
            if (cb == comboBoxEx4)
                subStrs = comboBoxEx4.Items[itemIndex].ToString().Split(new char[] { '-' });
            formName += subStrs[subStrs.Length - 1] + "年 ";

            subStrs = comboBoxEx6.Text.Split(new char[] { '-' });
            if (cb == comboBoxEx6)
                subStrs = comboBoxEx6.Items[itemIndex].ToString().Split(new char[] { '-' });
            formName += subStrs[subStrs.Length - 1] + " ";

            subStrs = comboBoxEx3.Text.Split(new char[] { '-' });
            if (cb == comboBoxEx3)
                subStrs = comboBoxEx3.Items[itemIndex].ToString().Split(new char[] { '-' });
            formName += subStrs[subStrs.Length - 1] + " ";

            formName += textBoxX4.Text;
            row["表格名称"] = formName;

            string additionalDescription = ""; //To get addition description
            subStrs = comboBoxEx2.Text.Split(new char[] { '-' });
            if (cb == comboBoxEx2)
                subStrs = comboBoxEx2.Items[itemIndex].ToString().Split(new char[] { '-' });
            additionalDescription +="方案"+ subStrs[subStrs.Length - 1]+",";

            subStrs = comboBoxEx5.Text.Split(new char[] { '-' });
            if (cb == comboBoxEx5)
                subStrs = comboBoxEx5.Items[itemIndex].ToString().Split(new char[] { '-' });
            additionalDescription += subStrs[subStrs.Length - 1];

            row["备注"] = additionalDescription;

        }

        /// <summary>
        /// get form number
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GenFormNo(int i)
        {
            if (GetComboBoxWithAllSelected() == null)
                return textBoxX2.Text;

            string str = this.textBoxX2.Text;
            int val = 0;
            int index = str.Length - 1;
            int partNum = 0;
            for (; index >= 0; index--)
            {
                string subStr = str.Substring(index);
                int j = 0;
                while (j<subStr.Length-1 && subStr[j] == '0') j++;
                if (Int32.TryParse(subStr.Substring(j), out val) == false)
                    break;
                else
                    partNum = val;
            }
            return str.Substring(0, index + 1) + (partNum + i);
        }

        private void UpdateFormDescription()
        {
            formDescription.Rows.Clear();
            ComboBox cb = GetComboBoxWithAllSelected();
            if (cb != null)
                for (int i = 0; i < cb.Items.Count - 1; i++)
                    AddRowToFormDescription(cb, i);
            else
                AddRowToFormDescription(cb, 0);
        }

        //显示进度条
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();
            
        }

        private string GetSysNameByID(string id)
        {
            DataTable dt=UniVars.InDS.Tables["系统表"];
            foreach(DataRow row in dt.Rows)
                if(row["节点ID"].ToString()==id)
                    return row["节点名称"].ToString();
            return null;
        }

        //根据模板ID返回基础表编号
        /// <summary>
        /// get template information accordiong ID number
        /// </summary>
        /// <param name="s"></param>
        private void GetTemplateInformation(string s)
        {

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "templete")
                {
                    XmlNodeList nodelist = element.ChildNodes; ;
                    if (nodelist.Count > 0)
                    {
                        foreach (XmlNode el in nodelist)//读元素值
                        {                            
                            if (el.Name.ToLower().Equals("item") && 
                                el.Attributes["id"].Value.ToString().Trim().Equals(s)
                                && el.Attributes["isFixedTemplate"] != null)
                            {
                                XmlNodeList nodelistclon = el.ChildNodes;
                                if (nodelistclon.Count > 0)
                                {
                                    foreach (XmlNode ell in nodelistclon)
                                    {
                                        string str = "";
                                        // sIDType
                                        if (ell.Name.ToLower().Equals("sidtype"))
                                            templateInfo.SysIDType = ell.InnerText;

                                        // dIDType
                                        if (ell.Name.ToLower().Equals("didtype"))
                                            templateInfo.DayIDType = ell.InnerText;

                                        // unitType
                                        if (ell.Name.ToLower().Equals("unittype"))
                                        {
                                            templateInfo.UnitTypeString1 = ell.ChildNodes[0].InnerText;
                                            templateInfo.UnitTypeString2 = ell.ChildNodes[1].InnerText;                                            
                                        }
                                        // Columns
                                        if (ell.Name.ToLower().Equals("columns"))
                                        {
                                            str = ell.ChildNodes[0].Attributes["belongTableId"].Value.ToString().Trim();
                                            //MessageBox.Show(str);
                                            templateInfo.SourceTableName = str;
                                        }
                                        // Rows
                                        if (ell.Name.ToLower().Equals("rows"))
                                        {
                                            str = ell.ChildNodes[0].Attributes["belongTableId"].Value.ToString().Trim();
                                            // MessageBox.Show(str);
                                            templateInfo.SourceTableName = str;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        //加载标签
        /// <summary>
        /// load the forms type data (comboBoxEx1)
        /// </summary>
        /// <returns></returns>
        private Boolean TempletesRead()
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
                        XmlNodeList nodelist = element.ChildNodes;
                        if (nodelist.Count > 0)
                        {
                            this.comboBoxEx1.Items.Clear();
                            foreach (XmlNode el in nodelist)//读元素值
                            {
                                if (el.Name.ToLower() == "item" && el.Attributes["isFixedTemplate"]!=null)
                                    this.comboBoxEx1.Items.Add(el.Attributes["id"].Value.ToString().Trim() +
                                        "-" + el.Attributes["name"].Value.ToString());
                            }
                            break;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        //重置combobox
        private void ComboboxReset()
        {
            comboBoxEx2.SelectedIndex = -1;
            comboBoxEx3.SelectedIndex = -1;
            comboBoxEx4.SelectedIndex = -1;
            comboBoxEx5.SelectedIndex = -1;
            comboBoxEx6.SelectedIndex = -1;
        }
        //让下拉列表框联动函数
        /// <summary>
        /// when combox changed ,reset comboBoxEx2/3/5
        /// </summary>
        /// <param name="s"></param>
        private void combChange(string s)
        {
            ComboboxReset();

            #region initial comboBoxEx2 
            comboBoxEx2.Items.Clear();
            DataTable distinctValues = OutDS.Tables[s].DefaultView.ToTable(true, "Prj");
            DataTable dt = InDS.Tables["方案表"];
            foreach (DataRow row in distinctValues.Rows)
            {
                //增加方案描述，方案的显示也变成了“1-sth”这样的形式    ----2014.07.18 GAO Yang
                foreach (DataRow r in dt.Rows)
                {
                    if (row[0].ToString().Equals(r[1].ToString()) && r[2].ToString() != "无")
                    {
                        string str = row[0].ToString() + "-" + r[2].ToString();
                        comboBoxEx2.Items.Add(str);
                        break;
                    }
                    else if(r[2].ToString() == "无")
                    {
                        comboBoxEx2.Items.Add(row[0].ToString());
                        break;
                    }
                    
                }
                
            }
            if (comboBoxEx2.Items.Count > 0)
            {
                comboBoxEx2.Items.Add("ALL");
               // comboBoxEx2.SelectedIndex = 0;
            }
            #endregion

            //initial comboBoxEx3 about system and partition
            switch (templateInfo.SysIDType)
            {
                case "1":
                    labelX6.Text = "系统及分区：";
                    //系统及分区
                    SysPart();//initial comboBoxEx3
                    break;
                case "2":
                    labelX6.Text = "  电站名称：";
                    //get power station list to ininial comboBoxEx3
                    genPart(comboBoxEx3);
                    break;
                case "3":
                    labelX6.Text = "联络线名称：";
                    //get contact line name list to ininial comboBoxEx3
                    linePart(comboBoxEx3,3);
                    break;
                case "4":
                    labelX6.Text = "输电线名称：";
                    //get power line name list to ininial comboBoxEx3
                    linePart(comboBoxEx3,4);
                    break;
            }

            //initial comboBoxEx5
            switch (templateInfo.DayIDType)
            {
                case "1":
                    dayPart1(comboBoxEx5);
                    break;
                case "2":
                    dayPart2(comboBoxEx5);
                    break;
            }
        }
        
        //加载方案描述
        private void prjPart()
        {
        }

        private void updateRowWithFactor(DataTable dt, int rowIndex, string factor)
        {
            if (factor == "1")
                return;

            switch (templateInfo.SourceTableName)
            {
                case "PPL":
                case "PLD":
                case "ENS":
                case "ENG":
                case "TEC":
                case "TRK":
                    for (int x = 0; x < dt.Columns.Count; x++)
                    {
                        string[] columnPartNames = dt.Columns[x].ColumnName.Split(new string[] { "." },
                            StringSplitOptions.None);
                        if (columnPartNames[0] == "项  目")
                            continue;
                        if (dt.Rows[rowIndex][x].ToString().Trim()!="")
                            dt.Rows[rowIndex][x] = Convert.ToDouble(dt.Rows[rowIndex][x])*Convert.ToDouble(factor);
                    }
                    break;
                default:
                    break;
            }
        }
        //修改表的行名和列名
        private void changeName(DataTable dt, int x)
        {
            int m = 0;
            int n = 0;
            //更改列名
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cName = dt.Columns[i].ColumnName;
                string[] arrStr1 = cName.Split(new string[] { "." }, StringSplitOptions.None);
                if (arrStr1[0] == "自定义")
                    dt.Columns[i].ColumnName = arrStr1[1].ToString();
                else
                {
                    string[] columnNameAndFactor = getColName(arrStr1[0], arrStr1[1]);
                    dt.Columns[i].ColumnName = columnNameAndFactor[0];
                    if (!this.rb0.Checked)
                        if (columnNameAndFactor[1] != "1")
                            foreach (DataRow row in dt.Rows)
                                if(row[i].ToString().Trim()!="")
                                    row[i] = Convert.ToDouble(row[i]) * Convert.ToDouble(columnNameAndFactor[1]);
                }
            }

            //更改行名
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string cName1 = dt.Rows[j][x].ToString();
                string[] arrStr11 = cName1.Split(new string[] { "-" }, StringSplitOptions.None);


                if (arrStr11[0] == "自定义")
                    dt.Rows[j][x] = arrStr11[1].ToString();
                else if (checkBox1.Checked == false)
                {
                    string[] rowAndFactor = getRowNameAndFactor(arrStr11[0], arrStr11[1]);
                    dt.Rows[j][x] = rowAndFactor[0];
                    if (!this.rb0.Checked)
                        updateRowWithFactor(dt, j, rowAndFactor[1]);
                }
                else
                {
                    string[] name = arrStr11[1].Split(new string[] { "." }, StringSplitOptions.None);
                    if (name.Length == 1)
                    {
                        string[] rowAndFactor = getRowNameAndFactor(arrStr11[0], arrStr11[1]);
                        dt.Rows[j][x] = convertInt(m) + "、" + rowAndFactor[0];
                        if (!this.rb0.Checked)
                            updateRowWithFactor(dt, j, rowAndFactor[1]);
                        n = 0;
                        m++;
                    }
                    else if (name.Length == 2)
                    {
                        n++;
                        string[] rowAndFactor = getRowNameAndFactor(arrStr11[0], arrStr11[1]);
                        dt.Rows[j][x] = "  " + n.ToString() + "." + rowAndFactor[0];
                        if (!this.rb0.Checked)
                            updateRowWithFactor(dt, j, rowAndFactor[1]); 
                    }
                    else if (name.Length == 3)
                    {
                        string[] rowAndFactor = getRowNameAndFactor(arrStr11[0], arrStr11[1]);
                        dt.Rows[j][x] = "    " + rowAndFactor[0];
                        if (!this.rb0.Checked)
                            updateRowWithFactor(dt, j, rowAndFactor[1]);
                    }
                }
            }
        }
        private string[] getColName(string a, string b)
        {
            string[] returnStr = new string[2];
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "dictionary")
                {
                    XmlNodeList nodelist = element.ChildNodes;
                    if (nodelist.Count > 0)
                    {
                        foreach (XmlNode el in nodelist)//读元素值
                        {
                            String c = el.Name.ToString();
                            if (el.Name.ToLower() == "item" && el.Attributes["id"].Value.ToString().Equals(a))
                            {
                                XmlNodeList nl = el.ChildNodes;
                                foreach (XmlNode ex in nl)
                                {

                                    if (ex.Name.ToLower() == "columns")
                                    {
                                        XmlNodeList nx = ex.ChildNodes;
                                        foreach (XmlNode xx in nx)
                                        {
                                            if (b.Equals(xx.Attributes["id"].Value.ToString()))
                                            {
                                                returnStr[0] = xx.Attributes["name"].Value.ToString();
                                                if (xx.Attributes["Factor"] == null)
                                                    returnStr[1] = "1";
                                                else
                                                    returnStr[1] = xx.Attributes["Factor"].Value.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return returnStr;
        }
        private string[] getRowNameAndFactor(string a, string b)
        {
            string[] returnStr =new string[2];
            returnStr[1] = "1";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "dictionary")
                {
                    XmlNodeList nodelist = element.ChildNodes;
                    if (nodelist.Count > 0)
                    {
                        foreach (XmlNode el in nodelist)//读元素值
                        {
                            String c = el.Name.ToString();
                            if (el.Name.ToLower() == "item" && el.Attributes["id"].Value.ToString().Equals(a))
                            {
                                XmlNodeList nl = el.ChildNodes;
                                foreach (XmlNode ex in nl)
                                {

                                    if (ex.Name.ToLower() == "rows")
                                    {
                                        XmlNodeList nx = ex.ChildNodes;
                                        foreach (XmlNode xx in nx)
                                        {
                                            if (b.Equals(xx.Attributes["code"].Value.ToString()))
                                            {
                                                returnStr[0] = xx.Attributes["content"].Value.ToString();
                                                if(xx.Attributes["Factor"]!=null)
                                                    returnStr[1] = xx.Attributes["Factor"].Value.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return returnStr;
        }
        private string convertInt(int d)
        {
            string tem = "";
            string[] chineseNum = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            string num = d.ToString();
            if (num.Length == 1)
            {
                tem = chineseNum[int.Parse(num)];
            }
            else if (num.Length == 2)
            {
                if (num[0] == '1')
                {
                    tem = "十" + chineseNum[int.Parse(num.Substring(1, 1))];
                }
                else
                {
                    tem = chineseNum[int.Parse(num.Substring(0, 1))] + "十" + chineseNum[int.Parse(num.Substring(1, 1))];
                }
            }
            return tem;
        }


        private string PrepareFilterString(string itemValue)
        {

            string str = "";

            //方案
            if (comboBoxEx2.Text == "ALL")
                str += "Prj = " + itemValue;
            else
                str += "Prj = " + comboBoxEx2.Text.Split('-')[0];

            //水平年
            if (comboBoxEx4.Text == "ALL")
                str += " and Yrs = " + itemValue;
            else
                str += " and Yrs = " + comboBoxEx4.Text;

            //水文条件
            if (comboBoxEx6.Text == "ALL")
                str += " and Hyd = " + itemValue;
            else
                str += " and Hyd = " + comboBoxEx6.Text.Split(new string[] { "-" }, StringSplitOptions.None)[0];

            //sID
            if (comboBoxEx3.Text == "ALL")
                str += " and sID=" + itemValue;
            else
                str += " and sID=" + comboBoxEx3.Text.Split(new string[] { "-" }, StringSplitOptions.None)[0];
                    
            //日类型
            if (comboBoxEx5.Text == "ALL")
                str += " and dID=" + itemValue;
            else
                str += " and dID=" + comboBoxEx5.Text.Split(new string[] { "-" }, StringSplitOptions.None)[0];

            return str;
        }
      
        //GEN数据写入, 这个函数好像没有用到
        private void writeDataForHST(ref DataTable dt, string combox)
        {
            DataView dv = new DataView();
            dv.Table = OutDS.Tables[templateInfo.SourceTableName];
            dv.RowFilter = PrepareFilterString(combox);
            for (int k = 0; k < dv.Count; k++)
            {
                DataRow row=dt.NewRow();
                for (int x = 0; x < dt.Columns.Count; x++)
                {
                    string[] columnPartNames = dt.Columns[x].ColumnName.Split(new string[] { "." }, StringSplitOptions.None);
                    row[x] = dv[k][columnPartNames[1]].ToString();
                }
                dt.Rows.Add(row);
            }

        }

        //往表格里填充数据
        //当sx=3时，combox为combobox3的值，当sx=5时，为combobox5的值
        private DataTable writeData(string fileName, ref DataTable dt, ref int xxu, string combox, int sx)
        {
            //一下三行 没有 任何引用
            string pName = this.comboBoxEx3.Text;
            string[] ar1 = pName.Split(new string[] { "-" }, StringSplitOptions.None); //分区
            string[] hh = comboBoxEx5.Text.Split(new string[] { "-" }, StringSplitOptions.None);  //日类型

            DataView dv = new DataView();
            dv.Table = OutDS.Tables[templateInfo.SourceTableName];

            int sign = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] ar = dt.Rows[i][sign].ToString().Split(new string[] { "-" }, StringSplitOptions.None);
                switch (ar[0])
                {
                    case "PPL":
                    case "PLD":
                    case "ENS":
                    case "ENG":
                    case "TEC":
                    case "TRK":
                    case "GEN":
                    case "HST":
                        dv.RowFilter = PrepareFilterString(combox);
                        for (int k = 0; k < dv.Count; k++)
                        {
                            if (ar[0] + "-" + dv[k]["Flg"].ToString() == dt.Rows[i][ar[0] + ".Flg"].ToString())
                            {
                                for (int x = 0; x < dt.Columns.Count; x++)
                                {
                                    string[] columnPartNames = dt.Columns[x].ColumnName.Split(new string[] { "." }, StringSplitOptions.None);
                                    if (columnPartNames[1] == "Flg")
                                        continue;
                                    string tmp = dv[k][columnPartNames[1]].ToString();
                                    if (tmp.Split(new char[] { '.' }).Length > 1)
                                        dt.Rows[i][x] = Convert.ToDouble(tmp).ToString("f2");
                                    else
                                        dt.Rows[i][x] = tmp;
                                }
                                break;
                            }
                        }
                        break;
                    default:
                        MessageBox.Show("default!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }        
            return dt;
        }

        /// <summary>
        /// set system and partition(initial comboBoxEx3)
        /// </summary>
        private void SysPart()
        {
            labelX6.Text = "系统及分区：";
            comboBoxEx3.Items.Clear();
            DataRow[] rows = UniVars.InDS.Tables["系统表"].Select("节点类型 >=100 and 节点类型 <102");
            int i = 0;
            foreach (DataRow row in rows)
            {
                comboBoxEx3.Items.Add(i + "-" + row["节点名称"].ToString());
                i++;
            }
            comboBoxEx3.Items.Add("ALL");
            
            //if (comboBoxEx3.Items.Count > 0)
            //    comboBoxEx3.SelectedIndex = 0;
        }

        //返回系统分区数目
        private int sysPart1()
        {
            foreach (object item in comboBoxEx3.Items)
                if (item.ToString()!="ALL")
                    this.comboBoxEx5.Items.Add(item.ToString()+"最大负荷日");
            return comboBoxEx3.Items.Count;
        }

        //读日类型，有7种的
        /// <summary>
        /// read date type to initialize comboBoxEx5
        /// </summary>
        /// <param name="comboBox"></param>
        private void dayPart1(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter = "节点类型 >=100 and 节点类型 <102";
            int i = 0;
            for (int index = 0; index < rows.Count; index++)
            {
                comboBox.Items.Add(i + "-" + rows[index]["节点名称"].ToString() + "最大负荷日");
                i++;
            }
            comboBox.Items.Add(i++ + "-周一");
            comboBox.Items.Add(i++ + "-周二");
            comboBox.Items.Add(i++ + "-周三");
            comboBox.Items.Add(i++ + "-周四");
            comboBox.Items.Add(i++ + "-周五");
            comboBox.Items.Add(i++ + "-周六");
            comboBox.Items.Add(i++ + "-周日");
            comboBox.Items.Add("ALL");
           // comboBox.SelectedIndex = 0;
        }

        //读日类型，有2种的
        private void dayPart2(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("0-最大负荷日合计");
            comboBox.Items.Add("1-年总计");
            comboBox.Items.Add("ALL");
          //  comboBox.SelectedIndex = 0;
        }

        //读电站名称
        private void genPart(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter="节点类型 >=300 and 节点类型 <400";
            int i = 0;
            for (int index=0;index<rows.Count;index++)
            {
                comboBox.Items.Add(i + "-" + rows[index]["节点名称"].ToString());
                i++;
            }
            comboBox.Items.Add("ALL");
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }
        //读线路名称
        private void linePart(ComboBox comboBox,int index)
        {
            comboBox.Items.Clear();
            DataView rows = UniVars.InDS.Tables["系统表"].DefaultView;
            rows.RowFilter = "节点类型 >=300 and 节点类型 <400";
            if (index == 3)
                rows.RowFilter = "节点类型 in (400,401) and IT01 = 0";
            else if(index==4)
                rows.RowFilter = "节点类型 in (400,401) and IT01 <> 0";
            else
                rows.RowFilter = "节点类型 >=100 and 节点类型 <200";

            int i = 0;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                comboBox.Items.Add(i + "-" + rows[rowIndex]["节点名称"].ToString());
                i++;
            }
            comboBox.Items.Add("ALL");
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }
        private DataTable[] selectTable()
        {
            string fName = "";
            int lLength = 0;
            //其中d为所有方案表
            if (comboBoxEx2.Text == "ALL")
            {
                int count = comboBoxEx2.Items.Count - 1;
                DataTable[] d = readAll(count);
                for (int i = 0; i < count; i++)
                {
                    string name = comboBoxEx2.Items[i].ToString().Split('-')[0];
                    writeData(fName, ref d[i], ref lLength, name, 0);
                    d[i].TableName = readTabname1(i);
                    DeleteNullRows(d[i]);
                    changeName(d[i], lLength);
                }
                return d;
            }
            //d为所有水平年的表
            else if (comboBoxEx4.Text == "ALL")
            {
                int count = comboBoxEx4.Items.Count - 1;
                DataTable[] d = readAll(count);
                for (int i = 0; i < count; i++)
                {
                    string name = comboBoxEx4.Items[i].ToString();
                    writeData(fName, ref d[i], ref lLength, name, 0);
                    d[i].TableName = readTabname1(i);
                    DeleteNullRows(d[i]);
                    changeName(d[i], lLength);
                }
                return d;
            }

                //d为所有水文条件表
            else if (comboBoxEx6.Text == "ALL")
            {
                int count = comboBoxEx6.Items.Count - 1;
                DataTable[] d = readAll(count);
                for (int i = 0; i < count; i++)
                {
                    string name = comboBoxEx6.Items[i].ToString().Substring(0, 1);
                    writeData(fName, ref d[i], ref lLength, name, 0);
                    d[i].TableName = readTabname1(i);
                    DeleteNullRows(d[i]);
                    changeName(d[i], lLength);
                }
                return d;
            }
            //d为所有分区
            else if (comboBoxEx3.Text == "ALL")
            {
                int count = comboBoxEx3.Items.Count - 1;
                DataTable[] d = readAll(count);
                for (int i = 0; i < count; i++)
                {
                    string j = comboBoxEx3.Items[i].ToString();
                    string[] xl = j.Split(new string[] { "-" }, StringSplitOptions.None);
                    writeData(fName, ref d[i], ref lLength, xl[0], 3);
                    d[i].TableName = readTabname1(i);
                    DeleteNullRows(d[i]);
                    changeName(d[i], lLength);
                }
                return d;
            }
            //d为所有日类型
            else if (comboBoxEx5.Text == "ALL")
            {
                int count = comboBoxEx5.Items.Count - 1;
                DataTable[] d = readAll(count);
                for (int i = 0; i < count; i++)
                {
                    string j = comboBoxEx5.Items[i].ToString();
                    string[] xl = j.Split(new string[] { "-" }, StringSplitOptions.None);
                    writeData(fName, ref d[i], ref lLength, xl[0], 5);
                    d[i].TableName = readTabname1(i);
                    DeleteNullRows(d[i]);
                    changeName(d[i], lLength);
                }
                return d;
            }
            else
            {
                DataTable[] d = readAll(1);
                DataTable dt = readData();
                int cc = dt.Rows.Count;
                int x = 0;
                dt = writeData(fName, ref dt, ref x, null, 0);
                dt.TableName = readTabname1(0);   
                d[0] = dt;
                DeleteNullRows(d[0]);
                changeName(d[0], x);
                return d;
            }

        }
        private void DeleteNullRows(DataTable dt)
        {
            for (int rowIndex = 0; rowIndex < dt.Rows.Count;)
            {
                bool toDelete = true;
                for (int i = 1; i < dt.Columns.Count; i++)
                    if (dt.Rows[rowIndex][i].ToString().Trim() != "")
                        toDelete = false;
                if (toDelete)
                    dt.Rows.RemoveAt(rowIndex);
                else
                    rowIndex++;
            }
        }
        //创建c个相同格式的数据表
        private DataTable[] readAll(int c)
        {
            DataTable[] dt = new DataTable[c];
            for (int i = 0; i < c; i++)
            {
                dt[i] = readData();
            }
            return dt;
        }

        //构建模板表
        private DataTable readData()
        {
            string templete = this.comboBoxEx1.Text;
            string[] arrStr1 = templete.Split(new string[] { "-" }, StringSplitOptions.None);

            DataTable dt = new DataTable();
            XmlDocument xmldoc = new XmlDocument();
            String a = Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+"";
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "templete")
                {
                    XmlNodeList nodelist = element.ChildNodes;
                    foreach (XmlNode el in nodelist)//读元素值
                    {
                        if (el.Name.ToLower() == "item" && el.Attributes["isFixedTemplate"]!=null &&
                            el.Attributes["id"].Value.ToString().Equals(arrStr1[0]))
                        {
                            XmlNodeList nl = el.ChildNodes;
                            foreach (XmlNode ex in nl)
                            {
                                if (ex.Name.ToLower() == "columns")
                                {
                                    XmlNodeList nx = ex.ChildNodes;
                                    foreach (XmlNode xx in nx)
                                    {
                                        DataColumn myDataColumn;
                                        myDataColumn = new DataColumn();
                                        if (xx.Attributes["belongTableId"].Value.ToString() == "自定义")
                                        {

                                        }
                                        else
                                        {
                                            myDataColumn.ColumnName = xx.Attributes["belongTableId"].Value.ToString() +
                                                "." + xx.Attributes["refid"].Value.ToString();
                                            myDataColumn.DataType = Type.GetType("System.String");
                                        }
                                        dt.Columns.Add(myDataColumn);
                                    }
                                }
                                if (ex.Name.ToLower() == "rows")
                                {
                                    int sign = 0;
                                    for (int j = 0; j < dt.Columns.Count; j++)
                                    {
                                        string[] xx = dt.Columns[j].ColumnName.ToString().Split(new string[] { "." },
                                            StringSplitOptions.None);
                                        if (xx[0] != "自定义" && xx[1] == "Flg")
                                        {
                                            sign = j;
                                            break;
                                        }
                                    }
                                    XmlNodeList nx = ex.ChildNodes;
                                    foreach (XmlNode xx in nx)
                                    {
                                        DataRow dr;
                                        dr = dt.NewRow();
                                        if (xx.Attributes["belongTableId"].Value.ToString() == "自定义")
                                        {

                                        }
                                        else
                                            dr[sign] = xx.Attributes["belongTableId"].Value.ToString() + 
                                                "-" + xx.Attributes["refcodeId"].Value.ToString();
                                        dt.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return dt;
        }

        //读取标签名
        private string readTabname1(int index)
        {
            string s = "";
            s += "表" + formDescription.Rows[index]["编号"].ToString() + "^";
            s +=  formDescription.Rows[index]["表格名称"].ToString() + "^";
            if(this.checkBox2.Checked)
                s += formDescription.Rows[index]["备注"].ToString();
            s+="^";
            s += labelX11.Text;
            return s;
        }

        /// <summary>
        /// enable conbox to be select and change
        /// </summary>
        /// <param name="enable"></param>
        private void EnableConditionSelect(bool enable)
        {
            //this.labelX1.Enabled = enable;
            //I don't know any sense of this
            this.labelX5.Enabled = enable;
            this.labelX6.Enabled = enable;
            this.labelX7.Enabled = enable;
            this.labelX8.Enabled = enable;
            this.labelX9.Enabled = enable;

            //make combox changeable
            this.textBoxX4.Enabled = enable;
            this.comboBoxEx2.Enabled = enable;
            this.comboBoxEx3.Enabled = enable;
            this.comboBoxEx4.Enabled = enable;
            this.comboBoxEx5.Enabled = enable;
            this.comboBoxEx6.Enabled = enable;
            this.checkBox1.Enabled = enable;
            this.rb0.Enabled = enable;
            if (enable)
            {
                //enable to add rows number 
                this.checkBox1.Checked = enable;
            }
            this.rb0.Checked = true;
            this.rb0.Text = templateInfo.UnitTypeString1;
            this.rb1.Text = templateInfo.UnitTypeString2;
        }

        /// <summary>
        /// form type combox change respond function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEx cmb = sender as ComboBoxEx;
            if (cmb.SelectedIndex >= 0)
            {
                this.buttonX3.Enabled = true;
                String templateName = cmb.Text;
                //remove number and the sign "-" to get the default name
                string[] arrStr1 = templateName.Split(new string[] { "-" }, StringSplitOptions.None);
                this.textBoxX4.Text = arrStr1[1].Substring(3);

                //get template information according ID number
                this.GetTemplateInformation(arrStr1[0].Trim());
                //enable combox can be changed
                EnableConditionSelect(true);
                //change other combox when one conbox selection had changed 
                this.combChange(templateInfo.SourceTableName);
                
                this.ReadHistory(arrStr1[0]);
            }
        }
        // two function have the same usage,why?
        private void ReadHistory(string templateID)
        {
            readHistoryByTemplateID(templateID,1);
        }

        /// <summary>
        /// set combox in default item
        /// </summary>
        private void UseDefaultCondition()
        {
            if (this.comboBoxEx2.Items.Count > 0)
                this.comboBoxEx2.SelectedIndex = 0;
            
            if (this.comboBoxEx4.Items.Count > 0)
                this.comboBoxEx4.SelectedIndex = 0;
            
            if (this.comboBoxEx6.Items.Count > 0)
                this.comboBoxEx6.SelectedIndex = 0;
            
            if (this.comboBoxEx3.Items.Count > 0)
                this.comboBoxEx3.SelectedIndex = 0;
            
            if (this.comboBoxEx5.Items.Count > 0)
                this.comboBoxEx5.SelectedIndex = 0;

            this.rb0.Checked = true;            
        }
        //initial comboBox2-6
        private void ResetCondition()
        {
            this.comboBoxEx2.SelectedIndex=-1;
            this.comboBoxEx4.SelectedIndex=-1;
            this.comboBoxEx6.SelectedIndex=-1;
            this.comboBoxEx3.SelectedIndex=-1;
            this.comboBoxEx5.SelectedIndex=-1;

            this.rb0.Checked=false;
        }

        /// <summary>
        /// read history selections to set combox default value
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="type"></param>
        private void readHistoryByTemplateID(string templateID,int type)
        {
            ResetCondition();
            
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "history")
                {
                    XmlNodeList nodelist=null ;
                    foreach (XmlNode node in element.ChildNodes)
                        if (node.Name.ToLower() == "fixedtemplate")
                            nodelist = node.ChildNodes;
                   
                    if (nodelist != null)
                    {
                        foreach (XmlNode el in nodelist)//读元素值
                        {
                            String c = el.Name.ToString();
                            if (el.Name.ToLower() == "condition" && 
                                el.Attributes["templetID"].Value.ToString().Equals(templateID))
                            {
                                bool hasAll = false;

                                int tmpIndex = Convert.ToInt32(el.ChildNodes[0].InnerText);
                                if (tmpIndex >= comboBoxEx2.Items.Count)
                                    tmpIndex = 0;

                                if (tmpIndex == comboBoxEx2.Items.Count - 1)
                                    if (hasAll)
                                        tmpIndex = 0;
                                    else
                                        hasAll = true;

                                comboBoxEx2.SelectedIndex = tmpIndex;

                                tmpIndex = Convert.ToInt32(el.ChildNodes[1].InnerText);
                                if (tmpIndex >= comboBoxEx4.Items.Count)
                                    tmpIndex = 0;

                                if (tmpIndex == comboBoxEx4.Items.Count - 1)
                                    if (hasAll)
                                        tmpIndex = 0;
                                    else
                                        hasAll = true;
                                comboBoxEx4.SelectedIndex = tmpIndex;

                                tmpIndex = Convert.ToInt32(el.ChildNodes[2].InnerText);
                                if (tmpIndex >= comboBoxEx6.Items.Count)
                                    tmpIndex = 0;
                                if (tmpIndex == comboBoxEx6.Items.Count - 1)
                                    if (hasAll)
                                        tmpIndex = 0;
                                    else
                                        hasAll = true;
                                comboBoxEx6.SelectedIndex = tmpIndex;

                                tmpIndex = Convert.ToInt32(el.ChildNodes[3].InnerText);
                                if (tmpIndex >= comboBoxEx3.Items.Count)
                                    tmpIndex = 0;
                                if (tmpIndex == comboBoxEx3.Items.Count - 1)
                                    if (hasAll)
                                        tmpIndex = 0;
                                    else
                                        hasAll = true;
                                comboBoxEx3.SelectedIndex = tmpIndex;

                                tmpIndex = Convert.ToInt32(el.ChildNodes[4].InnerText);
                                if (tmpIndex >= comboBoxEx5.Items.Count)
                                    tmpIndex = 0;
                                if (tmpIndex == comboBoxEx5.Items.Count - 1)
                                    if (hasAll)
                                        tmpIndex = 0;
                                    else
                                        hasAll = true;
                                comboBoxEx5.SelectedIndex = tmpIndex;
                                                             
                                if (el.ChildNodes[5].InnerText == "0")
                                    this.checkBox1.Checked = false;
                                else
                                    this.checkBox1.Checked = true;

                                if (el.ChildNodes[6].InnerText == "0")
                                    this.checkBox2.Checked = false;
                                else
                                    this.checkBox2.Checked = true;

                                if (el.ChildNodes[7].InnerText == "0")
                                    this.rb1.Checked = true;
                                else
                                    this.rb0.Checked = true;
                                if (el.LastChild.Name == "Decimal")
                                    textBoxX3.Text = el.LastChild.InnerText;
                                return;
                            }
                        }
                    }
                    break;
                }
            }
            UseDefaultCondition();

        }

        /// <summary>
        /// get history recorded id of forms type
        /// </summary>
        /// <returns></returns>
        private int GetFirstHistoryRecordID()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_" + ProS_Assm.UnitMnt.mMode + ".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "history")
                {
                    XmlNodeList nodelist = null;
                    foreach (XmlNode node in element.ChildNodes)
                        if (node.Name.ToLower() == "fixedtemplate")
                            nodelist = node.ChildNodes;

                    if (nodelist != null)
                    {
                        foreach (XmlNode el in nodelist)//读元素值
                        {
                            //String c = el.Name.ToString();
                            if (el.Name.ToLower() == "condition")
                            {
                                //return templete id with int type
                                return  Convert.ToInt32(el.Attributes["templetID"].Value.ToString());
                            }
                        }
                    }
                    break;
                }
            }
            return 1;
        }

        /// <summary>
        /// save the history selections to xml file
        /// </summary>
        private void WriteHistory()
        {
            string templateID=comboBoxEx1.Text.Split(new char[]{'-'})[0];

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
            //得到顶层节点列表
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode element in topM)
            {
                if (element.Name.ToLower() == "history")
                {
                    XmlNode parentNode = null;
                    foreach (XmlNode node in element.ChildNodes)
                        if (node.Name.ToLower() == "fixedtemplate")
                            parentNode = node;

                    if (parentNode != null)
                    {
                        XmlNode node = null;
                        foreach (XmlNode el in parentNode.ChildNodes)//读元素值
                            if (el.Name.ToLower() == "condition" &&
                                el.Attributes["templetID"].Value.ToString().Equals(templateID))
                            {
                                node = el;
                                break;
                            }
                        if (node != null)
                        {
                            parentNode.RemoveChild(node);

                            
                            node.ChildNodes[0].InnerText = comboBoxEx2.SelectedIndex.ToString();
                            
                            node.ChildNodes[1].InnerText = comboBoxEx4.SelectedIndex.ToString();
                            node.ChildNodes[2].InnerText = comboBoxEx6.SelectedIndex.ToString();
                            node.ChildNodes[3].InnerText = comboBoxEx3.SelectedIndex.ToString();
                            node.ChildNodes[4].InnerText = comboBoxEx5.SelectedIndex.ToString();
                            string tmpStr="1";
                            if(!this.checkBox1.Checked)
                                tmpStr="0";
                            node.ChildNodes[5].InnerText=tmpStr;
                            
                            tmpStr="1";
                            if(!this.checkBox2.Checked)
                                tmpStr="0";  
                            node.ChildNodes[6].InnerText = tmpStr;
                            
                            tmpStr="1";
                            if(!this.rb0.Checked)
                                tmpStr="0";  
                            node.ChildNodes[7].InnerText = tmpStr;
                            if (node.LastChild.Name == "Decimal")
                                node.LastChild.InnerText = textBoxX3.Text;
                            else
                            {
                                XmlElement subElem = xmldoc.CreateElement("Decimal");
                                subElem.InnerText = textBoxX3.Text;
                                node.AppendChild(subElem);
                            }
                            parentNode.PrependChild(node);
                        }
                        else
                        {
                            XmlElement newNode = xmldoc.CreateElement("Condition");
                            XmlElement subElem = xmldoc.CreateElement("Plan");
                            subElem.InnerText = comboBoxEx2.SelectedIndex.ToString();
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("Year");
                            subElem.InnerText = comboBoxEx4.SelectedIndex.ToString();
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("Hydr");
                            subElem.InnerText = comboBoxEx6.SelectedIndex.ToString();
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("SysPart");
                            subElem.InnerText = comboBoxEx3.SelectedIndex.ToString();
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("Day");
                            subElem.InnerText = comboBoxEx5.SelectedIndex.ToString();
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("RowNumEnable");
                            string tmpStr = "1";
                            if (!this.checkBox1.Checked)
                                tmpStr = "0";
                            subElem.InnerText = tmpStr;
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("DescriptionEnabled");
                            tmpStr = "1";
                            if (!this.checkBox2.Checked)
                                tmpStr = "0";
                            subElem.InnerText = tmpStr;
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("Unit");
                            tmpStr = "1";
                            if (!this.rb0.Checked)
                                tmpStr = "0";
                            subElem.InnerText = tmpStr;
                            newNode.AppendChild(subElem);

                            subElem = xmldoc.CreateElement("Decimal");
                            tmpStr = "2";
                            subElem.InnerText = tmpStr;
                            newNode.AppendChild(subElem);

                            XmlAttribute id = xmldoc.CreateAttribute("templetID");
                            id.InnerText = templateID;
                            newNode.Attributes.Append(id);

                            parentNode.PrependChild(newNode);
                        }
                    }
                    break;
                }
            }
            xmldoc.Save(Application.StartupPath + "\\TableViewConfig_"+ProS_Assm.UnitMnt.mMode+".xml");
        }

        /// <summary>
        /// scheme changed action (comboBoxEx2) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxEx2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEx2.Text != "")
            {
                comboBoxEx4.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[templateInfo.SourceTableName];
                if (comboBoxEx2.Text != "ALL")
                    dv.RowFilter = "Prj=" + this.comboBoxEx2.Text.Split('-')[0];
                DataTable distinctValues = dv.ToTable(true, "Yrs");
                foreach (DataRow row in distinctValues.Rows)
                    comboBoxEx4.Items.Add(row[0].ToString());
                if (comboBoxEx4.Items.Count > 0)
                {
                    comboBoxEx4.Items.Add("ALL");
                    comboBoxEx4.SelectedIndex = 0;
                }
                UpdateFormDescription();
            }
        }

        private void comboBoxEx4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEx4.Text != "")
            {
                comboBoxEx6.Items.Clear();
                DataView dv = new DataView();
                dv.Table = OutDS.Tables[templateInfo.SourceTableName];
                if (comboBoxEx2.Text != "ALL")
                {
                    dv.RowFilter = "Prj=" + this.comboBoxEx2.Text.Split('-')[0];
                    if(comboBoxEx4.Text!="ALL")
                        dv.RowFilter+=" and Yrs=" + this.comboBoxEx4.Text; ;
                }
                DataTable distinctValues = dv.ToTable(true, "Hyd");
                foreach (DataRow row in distinctValues.Rows)
                {
                    string str = row[0].ToString();
                    switch (str)
                    {
                        case "1":
                            str += "-枯水年";
                            break;
                        #region added by GaoYang
                        case "2":
                            str += "-平水年";
                            break;
                        case "4" :
                            str += "-丰水年";
                            break;
                        case "8" :
                            str += "-特枯年";
                            break;
                        case "16":
                            str += "-特丰年";
                            break;
                        #endregion
                        default:
                            str += "-其他";
                            break;
                    }
                    comboBoxEx6.Items.Add(str);
                }
                if (comboBoxEx6.Items.Count > 0)
                {
                    comboBoxEx6.Items.Add("ALL");
                    comboBoxEx6.SelectedIndex = 0;
                }
                UpdateFormDescription();
            }
        }

        private void comboBoxEx6_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormDescription();
        }

        private void comboBoxEx3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormDescription();
        }

        private void comboBoxEx5_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormDescription();
        }

        private void comboBoxEx7_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormDescription();
        }
        //only one all can be selected
        private bool CheckSingleAllSelected()
        {
            bool hasAll=false;

            if (comboBoxEx2.Text == "ALL")
                if (hasAll)
                    return false;
                else
                    hasAll = true;

            if (comboBoxEx4.Text == "ALL")
                if (hasAll)
                    return false;
                else
                    hasAll = true;

            if (comboBoxEx6.Text == "ALL")
                if (hasAll)
                    return false;
                else
                    hasAll = true;

            if (comboBoxEx3.Text == "ALL")
                if (hasAll)
                    return false;
                else
                    hasAll = true;

            if (comboBoxEx5.Text == "ALL")
                if (hasAll)
                    return false;
                else
                    hasAll = true;

            return true;
        }
        //creat forms
        private void buttonX3_Click(object sender, EventArgs e)
        {
            if (!CheckSingleAllSelected())
            {
                MessageBox.Show("“条件设置”中只能有一项选择“ALL”", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            WriteHistory();
            //this.Visible = false;
            
            //显示进度条
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);
            char[] temp = textBoxX3.Text.ToCharArray();
            if (textBoxX3.Text.Length > 1 || !(temp[0] >= '0' && temp[0] <= '9'))
            {
                this.myprogress.isOver = true;
                MessageBox.Show("小数位数，输入错误");
                return;
            }
            else
                deciNum = int.Parse(this.textBoxX3.Text);


            TableView tableView = new TableView();
            DataTable[] mytable = selectTable();

            //处理数据表，使之根据选择的小数位数来进行四舍五入  ---By GAO Yang 2014.08.25
            foreach (DataTable d in mytable)
            {
                foreach (DataRow row in d.Rows)
                    for (int i = 1; i < d.Columns.Count; i++)
                        //if (d.Columns[i].DataType != typeof(string))
                            row[i] = Math.Round(decimal.Parse(row[i].ToString()),deciNum);
            }
            //关闭进度条
            this.myprogress.isOver = true;  
            
            tableView.newTab(mytable);
            tableView.Text = textBoxX4.Text;
            //tableView.Owner = this;
            tableView.StartPosition = FormStartPosition.CenterScreen;
            //tableView.parentForm = this;
            //tableView.ShowDialog();
            //this.Close();
            tableView.Show();
            
        }

        //exit button action
        private void buttonX4_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //this.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "打开现有 LPSP_ProS 输入数据库文件";
            dlg.Filter = "Xml文件 |*.xml|所有文件|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filename = dlg.FileName.Split(new Char[] { '.'})[0];
                      
                AideFunc OpenXml=new AideFunc();
                
                if (OpenXml.ChkInDS(filename+".xml"))
                    OpenXml.OpenInDS();
                else
                {
                    MessageBox.Show("你打开了不正确的输入数据库文件。请重新选择！",
                        "打开文件提示", MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    return;
                }

                if (System.IO.File.Exists(filename + "_RST.xml") &&
                    System.IO.File.Exists(filename + "_MAP.xml") &&
                    System.IO.File.Exists(filename + "_GEN.xml"))
                {
                    UniVars.mOutFile = filename;

                    this.textBoxX1.Text = filename;
                    ReadOutFiles();
                    //如果用户选择终止计算则关闭窗口 添加by孙凯 2016.3.21
                    if (isClosed)
                    {
                        this.Close();
                        return;
                    }
                }
                else
                    MessageBox.Show("错误的输出文件龚永明！", "提示信息",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formDescription.Rows.Count; i++)
                formDescription.Rows[i]["编号"] = GenFormNo(i);
        }

        //单位选择改变 或  选择内容改变 事件
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rb0.Checked == true)
                this.labelX11.Text = "单位：" + rb0.Text;
            else
                this.labelX11.Text = "单位：" + rb1.Text;
        }
    }
    public class TemplateInfo
    {
        string sourceTableName = "";

        public string sumTypeId = "";

        public string SourceTableName
        {
            get { return sourceTableName; }
            set { sourceTableName = value; }
        }
        string sIDType = "0";
        public string SysIDType
        {
            get { return sIDType; }
            set { sIDType = value; }
        }
        string dIDType = "0";
        public string DayIDType
        {
            get { return dIDType; }
            set { dIDType = value; }
        }
        string unitTypeString1 = "0";
        public string UnitTypeString1
        {
            get { return unitTypeString1; }
            set { unitTypeString1 = value; }
        }

        string unitTypeString2 = "0";
        public string UnitTypeString2
        {
            get { return unitTypeString2; }
            set { unitTypeString2 = value; }
        }
    }
}
