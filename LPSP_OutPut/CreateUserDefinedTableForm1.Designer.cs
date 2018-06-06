namespace LPSP_OutPut
{
    partial class CreateUserDefinedTableForm1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbRow = new System.Windows.Forms.RadioButton();
            this.rbColumn = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbRow
            // 
            this.rbRow.AutoSize = true;
            this.rbRow.Checked = true;
            this.rbRow.Location = new System.Drawing.Point(36, 22);
            this.rbRow.Name = "rbRow";
            this.rbRow.Size = new System.Drawing.Size(71, 16);
            this.rbRow.TabIndex = 0;
            this.rbRow.TabStop = true;
            this.rbRow.Text = "按行组合";
            this.rbRow.UseVisualStyleBackColor = true;
            // 
            // rbColumn
            // 
            this.rbColumn.AutoSize = true;
            this.rbColumn.Location = new System.Drawing.Point(123, 22);
            this.rbColumn.Name = "rbColumn";
            this.rbColumn.Size = new System.Drawing.Size(119, 16);
            this.rbColumn.TabIndex = 0;
            this.rbColumn.Text = "按列组合(同类表)";
            this.rbColumn.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(106, 60);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "下一步";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(198, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // CreateUserDefinedTableForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 95);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbColumn);
            this.Controls.Add(this.rbRow);
            this.Name = "CreateUserDefinedTableForm1";
            this.Text = "第一步：选择组合方式";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbRow;
        private System.Windows.Forms.RadioButton rbColumn;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}