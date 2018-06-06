using LPSP_MergeDGV;
namespace LPSP_OutPut
{
    partial class CreateUserDefinedTableForm2
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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSourceDgv = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvRow = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvColumn = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbYearLevel = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelPartition = new System.Windows.Forms.Label();
            this.cbHydrateCondition = new System.Windows.Forms.ComboBox();
            this.cbPartition = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbDayType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbScheme = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvCreated = new LPSP_MergeDGV.MergeDataGridView();
            this.contextMenuStripCreatedDgv = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.合并单元格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加列头行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.删除选中行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除选中列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清空预览ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPreStep = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbCreatedDgvName = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.添加行头列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRow)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumn)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCreated)).BeginInit();
            this.contextMenuStripCreatedDgv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "选择源表:";
            // 
            // cbSourceDgv
            // 
            this.cbSourceDgv.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbSourceDgv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSourceDgv.FormattingEnabled = true;
            this.cbSourceDgv.Location = new System.Drawing.Point(322, 5);
            this.cbSourceDgv.Name = "cbSourceDgv";
            this.cbSourceDgv.Size = new System.Drawing.Size(389, 20);
            this.cbSourceDgv.TabIndex = 2;
            this.cbSourceDgv.SelectedIndexChanged += new System.EventHandler(this.cbSourceDgv_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvRow);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(76, 445);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "行选择";
            // 
            // dgvRow
            // 
            this.dgvRow.AllowUserToAddRows = false;
            this.dgvRow.AllowUserToDeleteRows = false;
            this.dgvRow.AllowUserToResizeColumns = false;
            this.dgvRow.AllowUserToResizeRows = false;
            this.dgvRow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRow.ColumnHeadersVisible = false;
            this.dgvRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRow.Location = new System.Drawing.Point(3, 17);
            this.dgvRow.Name = "dgvRow";
            this.dgvRow.ReadOnly = true;
            this.dgvRow.RowHeadersVisible = false;
            this.dgvRow.RowTemplate.Height = 23;
            this.dgvRow.Size = new System.Drawing.Size(70, 425);
            this.dgvRow.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvColumn);
            this.groupBox2.Location = new System.Drawing.Point(3, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(859, 61);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "列选择";
            // 
            // dgvColumn
            // 
            this.dgvColumn.AllowUserToAddRows = false;
            this.dgvColumn.AllowUserToDeleteRows = false;
            this.dgvColumn.AllowUserToResizeColumns = false;
            this.dgvColumn.AllowUserToResizeRows = false;
            this.dgvColumn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumn.ColumnHeadersVisible = false;
            this.dgvColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvColumn.Location = new System.Drawing.Point(3, 17);
            this.dgvColumn.Name = "dgvColumn";
            this.dgvColumn.ReadOnly = true;
            this.dgvColumn.RowHeadersVisible = false;
            this.dgvColumn.RowTemplate.Height = 23;
            this.dgvColumn.Size = new System.Drawing.Size(853, 41);
            this.dgvColumn.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cbYearLevel);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.labelPartition);
            this.groupBox3.Controls.Add(this.cbHydrateCondition);
            this.groupBox3.Controls.Add(this.cbPartition);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.cbDayType);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cbScheme);
            this.groupBox3.Location = new System.Drawing.Point(3, 64);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(808, 49);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "筛选条件";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(283, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "水平年:";
            // 
            // cbYearLevel
            // 
            this.cbYearLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYearLevel.FormattingEnabled = true;
            this.cbYearLevel.Location = new System.Drawing.Point(333, 17);
            this.cbYearLevel.Name = "cbYearLevel";
            this.cbYearLevel.Size = new System.Drawing.Size(67, 20);
            this.cbYearLevel.TabIndex = 2;
            this.cbYearLevel.SelectedIndexChanged += new System.EventHandler(this.cbYearLevel_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(604, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "水文条件:";
            // 
            // labelPartition
            // 
            this.labelPartition.AutoSize = true;
            this.labelPartition.Location = new System.Drawing.Point(106, 20);
            this.labelPartition.Name = "labelPartition";
            this.labelPartition.Size = new System.Drawing.Size(71, 12);
            this.labelPartition.TabIndex = 0;
            this.labelPartition.Text = "系统及分区:";
            // 
            // cbHydrateCondition
            // 
            this.cbHydrateCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHydrateCondition.FormattingEnabled = true;
            this.cbHydrateCondition.Location = new System.Drawing.Point(663, 17);
            this.cbHydrateCondition.Name = "cbHydrateCondition";
            this.cbHydrateCondition.Size = new System.Drawing.Size(88, 20);
            this.cbHydrateCondition.TabIndex = 2;
            // 
            // cbPartition
            // 
            this.cbPartition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPartition.FormattingEnabled = true;
            this.cbPartition.Location = new System.Drawing.Point(179, 17);
            this.cbPartition.Name = "cbPartition";
            this.cbPartition.Size = new System.Drawing.Size(96, 20);
            this.cbPartition.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(409, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "日类型:";
            // 
            // cbDayType
            // 
            this.cbDayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDayType.FormattingEnabled = true;
            this.cbDayType.Location = new System.Drawing.Point(456, 17);
            this.cbDayType.Name = "cbDayType";
            this.cbDayType.Size = new System.Drawing.Size(142, 20);
            this.cbDayType.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "方案:";
            // 
            // cbScheme
            // 
            this.cbScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScheme.FormattingEnabled = true;
            this.cbScheme.Location = new System.Drawing.Point(42, 17);
            this.cbScheme.Name = "cbScheme";
            this.cbScheme.Size = new System.Drawing.Size(58, 20);
            this.cbScheme.TabIndex = 2;
            this.cbScheme.SelectedIndexChanged += new System.EventHandler(this.cbScheme_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(813, 64);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(46, 49);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.dgvCreated);
            this.groupBox4.Location = new System.Drawing.Point(6, 143);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(853, 302);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "生成表格预览";
            // 
            // dgvCreated
            // 
            this.dgvCreated.AllowUserToAddRows = false;
            this.dgvCreated.AllowUserToDeleteRows = false;
            this.dgvCreated.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCreated.ContextMenuStrip = this.contextMenuStripCreatedDgv;
            this.dgvCreated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCreated.Location = new System.Drawing.Point(3, 17);
            this.dgvCreated.Name = "dgvCreated";
            this.dgvCreated.RowTemplate.Height = 23;
            this.dgvCreated.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCreated.Size = new System.Drawing.Size(847, 282);
            this.dgvCreated.TabIndex = 0;
            this.dgvCreated.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvCreated_CellMouseClick);
            // 
            // contextMenuStripCreatedDgv
            // 
            this.contextMenuStripCreatedDgv.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.合并单元格ToolStripMenuItem,
            this.添加行头列ToolStripMenuItem,
            this.添加列头行ToolStripMenuItem,
            this.toolStripSeparator1,
            this.删除选中行ToolStripMenuItem,
            this.删除选中列ToolStripMenuItem,
            this.清空预览ToolStripMenuItem,
            this.toolStripSeparator2});
            this.contextMenuStripCreatedDgv.Name = "contextMenuStrip1";
            this.contextMenuStripCreatedDgv.Size = new System.Drawing.Size(153, 170);
            // 
            // 合并单元格ToolStripMenuItem
            // 
            this.合并单元格ToolStripMenuItem.Name = "合并单元格ToolStripMenuItem";
            this.合并单元格ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.合并单元格ToolStripMenuItem.Text = "合并单元格";
            this.合并单元格ToolStripMenuItem.Click += new System.EventHandler(this.合并单元格ToolStripMenuItem_Click);
            // 
            // 添加列头行ToolStripMenuItem
            // 
            this.添加列头行ToolStripMenuItem.Name = "添加列头行ToolStripMenuItem";
            this.添加列头行ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.添加列头行ToolStripMenuItem.Text = "添加列头行";
            this.添加列头行ToolStripMenuItem.Click += new System.EventHandler(this.添加列头行ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // 删除选中行ToolStripMenuItem
            // 
            this.删除选中行ToolStripMenuItem.Name = "删除选中行ToolStripMenuItem";
            this.删除选中行ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.删除选中行ToolStripMenuItem.Text = "删除选中行";
            this.删除选中行ToolStripMenuItem.Click += new System.EventHandler(this.删除选中行ToolStripMenuItem_Click);
            // 
            // 删除选中列ToolStripMenuItem
            // 
            this.删除选中列ToolStripMenuItem.Name = "删除选中列ToolStripMenuItem";
            this.删除选中列ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.删除选中列ToolStripMenuItem.Text = "删除选中列";
            this.删除选中列ToolStripMenuItem.Click += new System.EventHandler(this.删除选中列ToolStripMenuItem_Click);
            // 
            // 清空预览ToolStripMenuItem
            // 
            this.清空预览ToolStripMenuItem.Name = "清空预览ToolStripMenuItem";
            this.清空预览ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.清空预览ToolStripMenuItem.Text = "清空预览表格";
            this.清空预览ToolStripMenuItem.Click += new System.EventHandler(this.清空预览ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // btnPreStep
            // 
            this.btnPreStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreStep.Location = new System.Drawing.Point(619, 489);
            this.btnPreStep.Name = "btnPreStep";
            this.btnPreStep.Size = new System.Drawing.Size(75, 23);
            this.btnPreStep.TabIndex = 5;
            this.btnPreStep.Text = "上一步";
            this.btnPreStep.UseVisualStyleBackColor = true;
            this.btnPreStep.Click += new System.EventHandler(this.btnPreStep_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(709, 489);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "完成";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(803, 489);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(243, 126);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "表名:";
            // 
            // tbCreatedDgvName
            // 
            this.tbCreatedDgvName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCreatedDgvName.Location = new System.Drawing.Point(284, 123);
            this.tbCreatedDgvName.Name = "tbCreatedDgvName";
            this.tbCreatedDgvName.Size = new System.Drawing.Size(351, 21);
            this.tbCreatedDgvName.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.tbCreatedDgvName);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.btnAdd);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Size = new System.Drawing.Size(945, 445);
            this.splitContainer1.SplitterDistance = 76;
            this.splitContainer1.TabIndex = 7;
            // 
            // 添加行头列ToolStripMenuItem
            // 
            this.添加行头列ToolStripMenuItem.Name = "添加行头列ToolStripMenuItem";
            this.添加行头列ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.添加行头列ToolStripMenuItem.Text = "添加行头列";
            this.添加行头列ToolStripMenuItem.Click += new System.EventHandler(this.添加行头列ToolStripMenuItem_Click);
            // 
            // CreateUserDefinedTableForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 520);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnPreStep);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbSourceDgv);
            this.Name = "CreateUserDefinedTableForm2";
            this.Text = "第二步：选择行及筛选条件";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRow)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumn)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCreated)).EndInit();
            this.contextMenuStripCreatedDgv.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSourceDgv;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbScheme;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbYearLevel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelPartition;
        private System.Windows.Forms.ComboBox cbHydrateCondition;
        private System.Windows.Forms.ComboBox cbPartition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnPreStep;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbDayType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbCreatedDgvName;
        private System.Windows.Forms.DataGridView dgvRow;
        private System.Windows.Forms.DataGridView dgvColumn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCreatedDgv;
        private System.Windows.Forms.ToolStripMenuItem 删除选中行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清空预览ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem 删除选中列ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 添加列头行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 合并单元格ToolStripMenuItem;
        private LPSP_MergeDGV.MergeDataGridView dgvCreated;
        private System.Windows.Forms.ToolStripMenuItem 添加行头列ToolStripMenuItem;
    }
}