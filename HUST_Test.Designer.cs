namespace HUST_Test
{
  partial class HUST_Test
  {
    /// <summary>
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows 窗体设计器生成的代码

    /// <summary>
    /// 设计器支持所需的方法 - 不要修改
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btnSheet = new System.Windows.Forms.Button();
      this.cmbSheet = new System.Windows.Forms.ComboBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnPos = new System.Windows.Forms.Button();
      this.cmbPos = new System.Windows.Forms.ComboBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btnCurve = new System.Windows.Forms.Button();
      this.cmbCurve = new System.Windows.Forms.ComboBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.txtPath = new System.Windows.Forms.TextBox();
      this.btnPath = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btnSheet);
      this.groupBox1.Controls.Add(this.cmbSheet);
      this.groupBox1.Location = new System.Drawing.Point(314, 113);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(153, 104);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "表单输出";
      // 
      // btnSheet
      // 
      this.btnSheet.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.btnSheet.ForeColor = System.Drawing.Color.DarkRed;
      this.btnSheet.Location = new System.Drawing.Point(72, 60);
      this.btnSheet.Name = "btnSheet";
      this.btnSheet.Size = new System.Drawing.Size(65, 29);
      this.btnSheet.TabIndex = 1;
      this.btnSheet.Text = "输出";
      this.btnSheet.UseVisualStyleBackColor = true;
      this.btnSheet.Click += new System.EventHandler(this.btnSheet_Click);
      // 
      // cmbSheet
      // 
      this.cmbSheet.FormattingEnabled = true;
      this.cmbSheet.Location = new System.Drawing.Point(17, 29);
      this.cmbSheet.Name = "cmbSheet";
      this.cmbSheet.Size = new System.Drawing.Size(120, 20);
      this.cmbSheet.TabIndex = 0;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnPos);
      this.groupBox2.Controls.Add(this.cmbPos);
      this.groupBox2.Location = new System.Drawing.Point(89, 113);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(153, 104);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "工作位置图";
      // 
      // btnPos
      // 
      this.btnPos.BackColor = System.Drawing.SystemColors.Control;
      this.btnPos.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.btnPos.ForeColor = System.Drawing.Color.DarkRed;
      this.btnPos.Location = new System.Drawing.Point(73, 60);
      this.btnPos.Name = "btnPos";
      this.btnPos.Size = new System.Drawing.Size(65, 29);
      this.btnPos.TabIndex = 1;
      this.btnPos.Text = "输出";
      this.btnPos.UseVisualStyleBackColor = false;
      this.btnPos.Click += new System.EventHandler(this.btnPos_Click);
      // 
      // cmbPos
      // 
      this.cmbPos.FormattingEnabled = true;
      this.cmbPos.Location = new System.Drawing.Point(17, 29);
      this.cmbPos.Name = "cmbPos";
      this.cmbPos.Size = new System.Drawing.Size(120, 20);
      this.cmbPos.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btnCurve);
      this.groupBox3.Controls.Add(this.cmbCurve);
      this.groupBox3.Location = new System.Drawing.Point(89, 232);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(153, 104);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "发电曲线图";
      // 
      // btnCurve
      // 
      this.btnCurve.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.btnCurve.ForeColor = System.Drawing.Color.DarkRed;
      this.btnCurve.Location = new System.Drawing.Point(72, 60);
      this.btnCurve.Name = "btnCurve";
      this.btnCurve.Size = new System.Drawing.Size(65, 29);
      this.btnCurve.TabIndex = 1;
      this.btnCurve.Text = "输出";
      this.btnCurve.UseVisualStyleBackColor = true;
      this.btnCurve.Click += new System.EventHandler(this.btnCurve_Click);
      // 
      // cmbCurve
      // 
      this.cmbCurve.FormattingEnabled = true;
      this.cmbCurve.Location = new System.Drawing.Point(17, 29);
      this.cmbCurve.Name = "cmbCurve";
      this.cmbCurve.Size = new System.Drawing.Size(120, 20);
      this.cmbCurve.TabIndex = 0;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.txtPath);
      this.groupBox4.Controls.Add(this.btnPath);
      this.groupBox4.ForeColor = System.Drawing.Color.DarkRed;
      this.groupBox4.Location = new System.Drawing.Point(89, 24);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(378, 77);
      this.groupBox4.TabIndex = 3;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "选择并确认：图表文件目录路径";
      // 
      // txtPath
      // 
      this.txtPath.Location = new System.Drawing.Point(17, 25);
      this.txtPath.Multiline = true;
      this.txtPath.Name = "txtPath";
      this.txtPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtPath.Size = new System.Drawing.Size(290, 36);
      this.txtPath.TabIndex = 2;
      // 
      // btnPath
      // 
      this.btnPath.Location = new System.Drawing.Point(317, 26);
      this.btnPath.Name = "btnPath";
      this.btnPath.Size = new System.Drawing.Size(52, 29);
      this.btnPath.TabIndex = 1;
      this.btnPath.Text = "新路径";
      this.btnPath.UseVisualStyleBackColor = true;
      this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
      // 
      // button1
      // 
      this.button1.BackColor = System.Drawing.SystemColors.Info;
      this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.button1.Location = new System.Drawing.Point(377, 270);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(90, 51);
      this.button1.TabIndex = 4;
      this.button1.Text = "退出";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // HUST_Test
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(544, 376);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "HUST_Test";
      this.Text = "通用功能模块测试";
      this.Load += new System.EventHandler(this.HUST_Test_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btnSheet;
    private System.Windows.Forms.ComboBox cmbSheet;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button btnPos;
    private System.Windows.Forms.ComboBox cmbPos;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Button btnCurve;
    private System.Windows.Forms.ComboBox cmbCurve;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.TextBox txtPath;
    private System.Windows.Forms.Button btnPath;
    private System.Windows.Forms.Button button1;
  }
}

