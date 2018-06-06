using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace HUST_OutPut
{
    public partial class ResetFile : Form
    {
        private string FilePath;
        private string srcFilePath;
        private string srcTable;
        private DataTable formDescription = new DataTable();

        public ResetFile()
        {
            InitializeComponent();
        }

        public ResetFile(string filePath,string srcFile,string tbName)
        {
            InitializeComponent();
            this.FilePath = filePath;
            this.srcFilePath = srcFile;
            this.srcTable = tbName;
            this.FilePathText.Text = filePath;
            PrepareFormDescription();
        }

        private void PrepareFormDescription()
        {
            try
            {
                formDescription.Columns.Add("编号", typeof(String));
                formDescription.Columns.Add("文件名称", typeof(String));
                formDescription.Columns.Add("表格名称", typeof(String));

                //add items
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(this.FilePath);
                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlNode node in topM)
                {
                    if (node.Name.ToLower() == "file")
                    {
                        int count = 0;
                        foreach (XmlNode chlNode in node.ChildNodes)
                        {
                            DataRow row = formDescription.NewRow();
                            row["编号"] = count++;
                            row["文件名称"] = chlNode.Attributes["file"].Value.ToString();
                            row["表格名称"] = chlNode.Attributes["table"].Value.ToString();
                            formDescription.Rows.Add(row);
                        }
                    }

                }



                dataGridViewX1.DataSource = formDescription;
                dataGridViewX1.Columns[1].FillWeight = 400;
                dataGridViewX1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewX1.Columns[2].FillWeight = 400;
                dataGridViewX1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewX1.AllowUserToAddRows = false;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Deal_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(this.FilePath);
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlNode node in topM)
            {
                if (node.Name.ToLower() == "file")
                {
                    foreach (XmlNode chlNode in node.ChildNodes)
                    {
                        string tmpFilePath = dataGridViewX1.SelectedRows[0].Cells[1].Value.ToString();//chlNode.Attributes["file"].Value.ToString();
                        string tmpTableName = dataGridViewX1.SelectedRows[0].Cells[2].Value.ToString();//chlNode.Attributes["table"].Value.ToString();
                        if (chlNode.Attributes["file"].Value.ToString() == tmpFilePath
                            && chlNode.Attributes["table"].Value.ToString() == tmpTableName)
                        {
                            chlNode.Attributes["file"].Value = tmpFilePath;
                            chlNode.Attributes["table"].Value = tmpTableName;
                        }
                        xmldoc.Save(this.FilePath);
                    }
                    break;
                }

            }
            this.Close();
        }

    }
}
