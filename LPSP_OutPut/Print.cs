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

        private DevComponents.DotNetBar.TabControl tab = null; //��TableView���tab���������� ������Ĵ�ӡ����
        private int currentTabIndex = 0;
        private int maxTabIndex = 0;  //����tabindexֵ   ������  ѡ����һ��ʱ��ֹԽ��

        public progress myprogress;

        public Print(DevComponents.DotNetBar.TabControl tab = null)
        {
            InitializeComponent();

            //����һ��������ʾ-��������-������
            Thread thdSub = new Thread(new ThreadStart(this.progressB));
            thdSub.Start();
            Thread.Sleep(100);

            this.vB2008Print1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseWheel);

            this.tab = tab;//��ӡ������ᴫ��tab����   
            if (tab != null)
            {
                this.btnFormer.Enabled = false;
                currentTabIndex = 0;
                this.maxTabIndex = tab.Tabs.Count - 1;

                //������ǰһҳ
                if (currentTabIndex == 0)
                    this.btnFormer.Enabled = false;
                //�������һҳ
                if (currentTabIndex == maxTabIndex)
                    this.btnBack.Enabled = false;


                SetPrintTab(currentTabIndex);
            }

            //�رս�����
            this.myprogress.isOver = true;
        }

        //�򿪽�����
        public void progressB()
        {
            this.myprogress = new progress();
            this.myprogress.Start(); //��ʼ���ȣ�ֱ��Form1_Loading()����ĩβ����ֹͣ����
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

            //���еĴ�ӡ�������д������¼���  �޸�by�￭ 2015.4.26 ���Ӷ��ͷ��ӡ����
            vB2008Print1.NewPage();
            //������
            try
            {
                TreeView tree = null;
                //�кϲ���Ԫ�� ��ӡ ���쳣
                if (this.dgv is VBprinter40.MulHeaderDataGridView)
                {
                    tree = (this.dgv as VBprinter40.MulHeaderDataGridView).ColHeaderTreeView;
                }
                vB2008Print1.PrintDGV(dgv, name, new Font("����", 12), "",
                    new Font("����", 10), StringAlignment.Center, "1111",
                    true, true, this.dgv.Font, "", "", unit, this.dgv.Font,
                    "", "", "", 0, true, true, 0, 0, true, tree, StringAlignment.Near,
                    StringAlignment.Center, StringAlignment.Far, StringAlignment.Near,
                    StringAlignment.Center,
                    StringAlignment.Far);

                //vB2008Print1.PrintDGV(dgv, name, new Font("����", 12), "", new Font("����", 10), StringAlignment.Center, "1111", true, true, this.dgv.Font, "", "", unit, this.dgv.Font, "", "", "", 0, true, true, 0, 0);
                // vB2008Print1.PrintDGV();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

            vB2008Print1.NewRow(5);//���Ӽ��
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
            //���������ҳü��ҳ��
           // vB2008Print1.PrintFooter("", "��" + curpage + "ҳ/��" + pages + "ҳ", "", new Font("����", 12), Color.Black, 0);
           // vB2008Print1.PrintHeader(plan, "", unit);
            if (curpage > 1 && curpage<=pages)
           {
              StringFormat myformat =new StringFormat();
                myformat.Alignment=StringAlignment.Near;
               vB2008Print1.Currentx=0;
               vB2008Print1.Currenty=0;
               vB2008Print1.DrawText("����", vB2008Print1.PaperPrintWidth, dgv.Font, Color.Black, myformat);
               //vB2008Print1.DrawText("����", 0);
         
           }
           
        }

        //ѡ����һ��  ׼����ӡ
        private void btnFormer_Click(object sender, EventArgs e)
        {
            this.btnBack.Enabled = true;
            currentTabIndex--;
            SetPrintTab(currentTabIndex);

            //������ǰһҳ
            if (currentTabIndex == 0)
                this.btnFormer.Enabled = false;
        }
        //ѡ����һ��   ׼����ӡ
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.btnFormer.Enabled = true;
            currentTabIndex++;
            SetPrintTab(currentTabIndex);

            //�������һҳ
            if (currentTabIndex == maxTabIndex)
                this.btnBack.Enabled = false;
        }

        //���ô�ӡ������ 
        private void SetPrintTab(int index)
        {
            foreach (Control ctl in tab.Tabs[index].AttachedControl.Controls)
            {//��ʼ��ʾ����  ��һ��tab��ı��
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
                {//�������� �Զ�����ʱ arrStrû��[2]
                    this.name = "�Զ�����" + arrStr[0];
                }

                this.dgv = dg;
            }

            //���¼��ش�ӡ����
            Print_Load(null, null);
        }

        //��ӡ��������
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
            catch (Exception) { MessageBox.Show("������ӡ�����쳣���˳���"); }
        }

    }
}