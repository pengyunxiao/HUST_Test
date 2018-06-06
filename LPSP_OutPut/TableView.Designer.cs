namespace HUST_OutPut
{
    partial class TableView
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
            this.tabControl1 = new DevComponents.DotNetBar.TabControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.打印ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.打印当前表格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印所有表格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出ExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出当前表格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出所有表格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.转置表格ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除单元格所在行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除单元格所在咧ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.合并ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.AutoCloseTabs = true;
            this.tabControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.tabControl1.CanReorderTabs = true;
            this.tabControl1.CloseButtonOnTabsAlwaysDisplayed = false;
            this.tabControl1.CloseButtonOnTabsVisible = true;
            this.tabControl1.CloseButtonPosition = DevComponents.DotNetBar.eTabCloseButtonPosition.Right;
            this.tabControl1.CloseButtonVisible = true;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedTabFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.tabControl1.SelectedTabIndex = -1;
            this.tabControl1.Size = new System.Drawing.Size(804, 506);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.TabLayoutType = DevComponents.DotNetBar.eTabLayoutType.FixedWithNavigationBox;
            this.tabControl1.Text = "tabControl1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打印ToolStripMenuItem1,
            this.导出ExcelToolStripMenuItem,
            this.转置表格ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(804, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 打印ToolStripMenuItem1
            // 
            this.打印ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打印当前表格ToolStripMenuItem,
            this.打印所有表格ToolStripMenuItem});
            this.打印ToolStripMenuItem1.Name = "打印ToolStripMenuItem1";
            this.打印ToolStripMenuItem1.Size = new System.Drawing.Size(44, 21);
            this.打印ToolStripMenuItem1.Text = "打印";
            // 
            // 打印当前表格ToolStripMenuItem
            // 
            this.打印当前表格ToolStripMenuItem.Name = "打印当前表格ToolStripMenuItem";
            this.打印当前表格ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.打印当前表格ToolStripMenuItem.Text = "打印当前表格";
            this.打印当前表格ToolStripMenuItem.Click += new System.EventHandler(this.Print_Click);
            // 
            // 打印所有表格ToolStripMenuItem
            // 
            this.打印所有表格ToolStripMenuItem.Name = "打印所有表格ToolStripMenuItem";
            this.打印所有表格ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.打印所有表格ToolStripMenuItem.Text = "打印所有表格";
            this.打印所有表格ToolStripMenuItem.Click += new System.EventHandler(this.打印所有表格ToolStripMenuItem_Click);
            // 
            // 导出ExcelToolStripMenuItem
            // 
            this.导出ExcelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导出当前表格ToolStripMenuItem,
            this.导出所有表格ToolStripMenuItem});
            this.导出ExcelToolStripMenuItem.Name = "导出ExcelToolStripMenuItem";
            this.导出ExcelToolStripMenuItem.Size = new System.Drawing.Size(73, 21);
            this.导出ExcelToolStripMenuItem.Text = "导出Excel";
            // 
            // 导出当前表格ToolStripMenuItem
            // 
            this.导出当前表格ToolStripMenuItem.Name = "导出当前表格ToolStripMenuItem";
            this.导出当前表格ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.导出当前表格ToolStripMenuItem.Text = "导出当前表格";
            this.导出当前表格ToolStripMenuItem.Click += new System.EventHandler(this.SaveCurrentTab_Click);
            // 
            // 导出所有表格ToolStripMenuItem
            // 
            this.导出所有表格ToolStripMenuItem.Name = "导出所有表格ToolStripMenuItem";
            this.导出所有表格ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.导出所有表格ToolStripMenuItem.Text = "导出所有表格";
            this.导出所有表格ToolStripMenuItem.Click += new System.EventHandler(this.SaveAllTab_Click);
            // 
            // 转置表格ToolStripMenuItem
            // 
            this.转置表格ToolStripMenuItem.Name = "转置表格ToolStripMenuItem";
            this.转置表格ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.转置表格ToolStripMenuItem.Text = "转置表格";
            this.转置表格ToolStripMenuItem.Click += new System.EventHandler(this.Rotate_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除单元格所在行ToolStripMenuItem,
            this.删除单元格所在咧ToolStripMenuItem,
            this.合并ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 70);
            // 
            // 删除单元格所在行ToolStripMenuItem
            // 
            this.删除单元格所在行ToolStripMenuItem.Name = "删除单元格所在行ToolStripMenuItem";
            this.删除单元格所在行ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.删除单元格所在行ToolStripMenuItem.Text = "删除单元格所在行";
            this.删除单元格所在行ToolStripMenuItem.Click += new System.EventHandler(this.DeleteRow_Click);
            // 
            // 删除单元格所在咧ToolStripMenuItem
            // 
            this.删除单元格所在咧ToolStripMenuItem.Name = "删除单元格所在咧ToolStripMenuItem";
            this.删除单元格所在咧ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.删除单元格所在咧ToolStripMenuItem.Text = "删除单元格所在列";
            this.删除单元格所在咧ToolStripMenuItem.Click += new System.EventHandler(this.DeleteColumn_Click);
            // 
            // 合并ToolStripMenuItem
            // 
            this.合并ToolStripMenuItem.Name = "合并ToolStripMenuItem";
            this.合并ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.合并ToolStripMenuItem.Text = "合并";
            this.合并ToolStripMenuItem.Click += new System.EventHandler(this.合并ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.DeleteColumn_Click);
            // 
            // TableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 531);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "TableView";
            this.Text = "TableView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TableView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevComponents.DotNetBar.TabControl tabControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除单元格所在行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除单元格所在咧ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 打印当前表格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印所有表格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出ExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出当前表格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出所有表格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 转置表格ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 合并ToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}