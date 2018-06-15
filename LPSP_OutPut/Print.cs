using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Threading;

namespace HUST_OutPut
{
    public partial class Print : DevComponents.DotNetBar.Office2007Form
    {

        public DataGridView dgv;
        public string beizhu="";
        public string name = "";
        public string unit = "";

        private DevComponents.DotNetBar.TabControl tab = null; //将TableView里的tab传进来进行 多个表格的打印处理
        private int currentTabIndex = 0;
        private int maxTabIndex = 0;  //最大的tabindex值   作用是  选择下一个时防止越界

        public progress myprogress;

        public Print(DevComponents.DotNetBar.TabControl tab = null)
        {
            InitializeComponent();

            //创建一个进程显示-加载数据-进度条
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);

            this.vB2008Print1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseWheel);

            this.tab = tab;//打印多个表格会传入tab参数   
            if (tab != null)
            {
                this.btnFormer.Enabled = false;
                currentTabIndex = 0;
                this.maxTabIndex = tab.Tabs.Count - 1;

                //到达最前一页
                if (currentTabIndex == 0)
                    this.btnFormer.Enabled = false;
                //到达最后一页
                if (currentTabIndex == maxTabIndex)
                    this.btnBack.Enabled = false;


                SetPrintTab(currentTabIndex);
            }

            //关闭进度条
            this.myprogress.isOver = true;
        }

        //打开进度条
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //开始进度，直至Form1_Loading()函数末尾，才停止进度
            myprogress.ShowDialog();
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            int mVSValue = this.vB2008Print1.VerticalScroll.Value;

            if ((mVSValue - e.Delta) <= this.vB2008Print1.VerticalScroll.Minimum)
            {
                this.vB2008Print1.VerticalScroll.Value = this.vB2008Print1.VerticalScroll.Minimum;
            }
            else if ((mVSValue - e.Delta) >= this.vB2008Print1.VerticalScroll.Maximum)
            {
                this.vB2008Print1.VerticalScroll.Value = this.vB2008Print1.VerticalScroll.Maximum;
            }
            else
            {
                this.vB2008Print1.VerticalScroll.Value -= e.Delta;
            }

            if (this.vB2008Print1.VerticalScroll.Value == mVSValue)
            {
                return;
            }
            this.vB2008Print1.Refresh();
            this.vB2008Print1.Invalidate();
            this.vB2008Print1.Update();
        }
        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            this.vB2008Print1.Focus();
        }

        private void Print_Load(object sender, EventArgs e)
        {
            vB2008Print1.InvalidatePreview();
        }

        private void vB2008Print1_PrintDocument()
        {

            //所有的打印代码必须写在这个事件中  修改by孙凯 2015.4.26 增加多表头打印功能
            vB2008Print1.NewPage();
            //输出表格
            try
            {
                TreeView tree = null;
                //有合并单元格 打印 出异常
                if (this.dgv is VBprinter40.MulHeaderDataGridView)
                {
                    tree = (this.dgv as VBprinter40.MulHeaderDataGridView).ColHeaderTreeView;
                }
                vB2008Print1.PrintDGV(dgv, name, new Font("黑体", 12), "",
                    new Font("宋体", 10), StringAlignment.Center, "1111",
                    true, true, this.dgv.Font, "", "", unit, this.dgv.Font,
                    "", "", "", 0, true, true, 0, 0, true, tree, StringAlignment.Near,
                    StringAlignment.Center, StringAlignment.Far, StringAlignment.Near,
                    StringAlignment.Center,
                    StringAlignment.Far);

                //vB2008Print1.PrintDGV(dgv, name, new Font("黑体", 12), "", new Font("宋体", 10), StringAlignment.Center, "1111", true, true, this.dgv.Font, "", "", unit, this.dgv.Font, "", "", "", 0, true, true, 0, 0);
                // vB2008Print1.PrintDGV();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

            vB2008Print1.NewRow(5);//增加间距
           // vB2008Print1.DrawText("     ", 0);
           // vB2008Print1.DrawText(beizhu , 0);
            StringFormat myformat = new StringFormat();
            myformat.Alignment = StringAlignment.Near;
           // vB2008Print1.Currentx = 0;
           // vB2008Print1.Currenty = 0;
            vB2008Print1.DrawText(beizhu, vB2008Print1.PaperPrintWidth, dgv.Font, Color.Black, myformat);
            
            
        }
        private void vB2008Print1_HeaderFooterOut(int pages, int curpage)
        {
            //在这里输出页眉与页脚
           // vB2008Print1.PrintFooter("", "第" + curpage + "页/共" + pages + "页", "", new Font("宋体", 12), Color.Black, 0);
           // vB2008Print1.PrintHeader(plan, "", unit);
            if (curpage > 1 && curpage<=pages)
           {
              StringFormat myformat =new StringFormat();
                myformat.Alignment=StringAlignment.Near;
               vB2008Print1.Currentx=0;
               vB2008Print1.Currenty=0;
               vB2008Print1.DrawText("续表", vB2008Print1.PaperPrintWidth, dgv.Font, Color.Black, myformat);
               //vB2008Print1.DrawText("续表", 0);
         
           }
           
        }

        //选择上一个  准备打印
        private void btnFormer_Click(object sender, EventArgs e)
        {
            this.btnBack.Enabled = true;
            currentTabIndex--;
            SetPrintTab(currentTabIndex);

            //到达最前一页
            if (currentTabIndex == 0)
                this.btnFormer.Enabled = false;
        }
        //选择下一个   准备打印
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.btnFormer.Enabled = true;
            currentTabIndex++;
            SetPrintTab(currentTabIndex);

            //到达最后一页
            if (currentTabIndex == maxTabIndex)
                this.btnBack.Enabled = false;
        }

        //设置打印的数据 
        private void SetPrintTab(int index)
        {
            foreach (Control ctl in tab.Tabs[index].AttachedControl.Controls)
            {//初始显示的是  第一个tab里的表格
                DataGridView dg = (DataGridView)ctl;
                string[] arrStr = tab.Tabs[index].Tooltip.Split(new string[] { "\n" }, StringSplitOptions.None);
                if (arrStr.Length > 2)
                {
                    this.name = arrStr[0] + "\n" + arrStr[1] + "\n";
                    //if (arrStr[2].Trim() != "")
                    //    this.name += "(" + arrStr[2] + ")";///dgv.Name;
                    ///////this.beizhu = arrStr[2];
                    this.unit = arrStr[2];
                }
                else
                {//当传过来 自定义表格时 arrStr没有[2]
                    this.name = "自定义表格：" + arrStr[0];
                }

                this.dgv = dg;
            }

            //重新加载打印内容
            Print_Load(null, null);
        }

        //打印后面所有
        private void btnBackAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = currentTabIndex; i <= maxTabIndex; i++)
                {
                    SetPrintTab(i);
                    Print_Load(null, null);
                    this.vB2008Print1.Print();
                }
            }
            catch (Exception) { MessageBox.Show("连续打印出现异常，退出！"); }
        }

    }
}