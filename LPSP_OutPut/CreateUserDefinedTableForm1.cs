using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LPSP_OutPut
{
    public partial class CreateUserDefinedTableForm1 : Form
    {
        public CreateUserDefinedTableForm1(string restructType="")
        {
            InitializeComponent();

            if (restructType != "")
            {
                if (restructType == "row")
                {
                    this.rbRow.Checked = true;
                    this.rbColumn.Checked = false;
                }
                else if (restructType == "column")
                {
                    this.rbRow.Checked = false;
                    this.rbColumn.Checked = true;
                }
            }
        }

        //创建下一步
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
            CreateUserDefinedTableForm2 form = new CreateUserDefinedTableForm2(this.rbRow.Checked==true? "row":"column");
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }
    }
}
