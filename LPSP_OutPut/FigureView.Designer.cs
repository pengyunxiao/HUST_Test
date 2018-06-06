namespace HUST_OutPut
{
    partial class FigureView
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.保存图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.页面设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.dlgSavePic = new System.Windows.Forms.SaveFileDialog();
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).BeginInit();
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
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedTabFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.tabControl1.SelectedTabIndex = -1;
            this.tabControl1.Size = new System.Drawing.Size(839, 531);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.TabLayoutType = DevComponents.DotNetBar.eTabLayoutType.FixedWithNavigationBox;
            this.tabControl1.Text = "tabControl1";
            this.tabControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyDown);
            this.tabControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存图片ToolStripMenuItem,
            this.页面设置ToolStripMenuItem,
            this.打印ToolStripMenuItem,
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem,
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(283, 136);
            // 
            // 保存图片ToolStripMenuItem
            // 
            this.保存图片ToolStripMenuItem.Name = "保存图片ToolStripMenuItem";
            this.保存图片ToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.保存图片ToolStripMenuItem.Text = "保存图片";
            this.保存图片ToolStripMenuItem.Click += new System.EventHandler(this.保存图片ToolStripMenuItem_Click);
            // 
            // 页面设置ToolStripMenuItem
            // 
            this.页面设置ToolStripMenuItem.Name = "页面设置ToolStripMenuItem";
            this.页面设置ToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.页面设置ToolStripMenuItem.Text = "页面设置";
            this.页面设置ToolStripMenuItem.Click += new System.EventHandler(this.页面设置ToolStripMenuItem_Click);
            // 
            // 打印ToolStripMenuItem
            // 
            this.打印ToolStripMenuItem.Name = "打印ToolStripMenuItem";
            this.打印ToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.打印ToolStripMenuItem.Text = "打印";
            this.打印ToolStripMenuItem.Click += new System.EventHandler(this.Print_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // dlgSavePic
            // 
            this.dlgSavePic.Filter = "*.bmp|*.bmp|*.png|*.png|*.jpg|*.jpg|*.gif|*.gif";
            // 
            // 放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem
            // 
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem.Name = "放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem";
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem.Text = "放大图片（Ctrl+鼠标左键+向上滚轮）";
            this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem.Click += new System.EventHandler(this.放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem_Click);
            // 
            // 缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem
            // 
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem.Name = "缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem";
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem.Text = "缩小图片（Ctrl+鼠标左键+向下滚轮）";
            this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem.Click += new System.EventHandler(this.缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem_Click);
            // 
            // FigureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 531);
            this.Controls.Add(this.tabControl1);
            this.KeyPreview = true;
            this.Name = "FigureView";
            this.Text = "FigureView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FigureView_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.TabControl tabControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 保存图片ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 页面设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印ToolStripMenuItem;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.SaveFileDialog dlgSavePic;
        private System.Windows.Forms.ToolStripMenuItem 放大图片Ctrl鼠标左键向上滚轮ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 缩小图片Ctrl鼠标左键向下滚轮ToolStripMenuItem;
    }
}