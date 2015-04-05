namespace PRTools
{
    partial class prtools
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.prMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxRows = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dasdFileLoadVar = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sortVar = new System.Windows.Forms.ComboBox();
            this.trNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deviceType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.createExcel = new System.Windows.Forms.Button();
            this.fileOpen = new System.Windows.Forms.Button();
            this.fileProcess = new System.Windows.Forms.Button();
            this.fileTextBox = new System.Windows.Forms.TextBox();
            this.fileLable = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolFileOpen = new System.Windows.Forms.ToolStripButton();
            this.fileText = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.dasdTopOutputVar = new System.Windows.Forms.ComboBox();
            this.daseTopOutputLine = new System.Windows.Forms.TextBox();
            this.prMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // prMenuStrip
            // 
            this.prMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem});
            this.prMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.prMenuStrip.Name = "prMenuStrip";
            this.prMenuStrip.Size = new System.Drawing.Size(975, 25);
            this.prMenuStrip.TabIndex = 0;
            this.prMenuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenMenu,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.FileToolStripMenuItem.Text = "文件(&F)";
            // 
            // fileOpenMenu
            // 
            this.fileOpenMenu.Name = "fileOpenMenu";
            this.fileOpenMenu.Size = new System.Drawing.Size(118, 22);
            this.fileOpenMenu.Text = "打开(&O)";
            this.fileOpenMenu.Click += new System.EventHandler(this.fileOpenMenu_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.ExitToolStripMenuItem.Text = "退出(&X)";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.daseTopOutputLine);
            this.groupBox1.Controls.Add(this.dasdTopOutputVar);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxRows);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dasdFileLoadVar);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sortVar);
            this.groupBox1.Controls.Add(this.trNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.deviceType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.createExcel);
            this.groupBox1.Controls.Add(this.fileOpen);
            this.groupBox1.Controls.Add(this.fileProcess);
            this.groupBox1.Controls.Add(this.fileTextBox);
            this.groupBox1.Controls.Add(this.fileLable);
            this.groupBox1.Controls.Add(this.typeComboBox);
            this.groupBox1.Controls.Add(this.typeLabel);
            this.groupBox1.Location = new System.Drawing.Point(9, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(954, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件处理";
            // 
            // textBoxRows
            // 
            this.textBoxRows.Location = new System.Drawing.Point(102, 68);
            this.textBoxRows.Name = "textBoxRows";
            this.textBoxRows.ReadOnly = true;
            this.textBoxRows.Size = new System.Drawing.Size(69, 21);
            this.textBoxRows.TabIndex = 17;
            this.textBoxRows.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "当前处理行数:";
            // 
            // dasdFileLoadVar
            // 
            this.dasdFileLoadVar.FormattingEnabled = true;
            this.dasdFileLoadVar.Items.AddRange(new object[] {
            "否",
            "是"});
            this.dasdFileLoadVar.Location = new System.Drawing.Point(635, 42);
            this.dasdFileLoadVar.Name = "dasdFileLoadVar";
            this.dasdFileLoadVar.Size = new System.Drawing.Size(45, 20);
            this.dasdFileLoadVar.TabIndex = 15;
            this.dasdFileLoadVar.Text = "否";
            this.dasdFileLoadVar.SelectedIndexChanged += new System.EventHandler(this.dasdFileLoadVar_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(495, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "DASD文件Load到数据库：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(330, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "是否开启DASD排序：";
            // 
            // sortVar
            // 
            this.sortVar.FormattingEnabled = true;
            this.sortVar.Items.AddRange(new object[] {
            "否",
            "是"});
            this.sortVar.Location = new System.Drawing.Point(444, 42);
            this.sortVar.Name = "sortVar";
            this.sortVar.Size = new System.Drawing.Size(45, 20);
            this.sortVar.TabIndex = 12;
            this.sortVar.Text = "否";
            // 
            // trNum
            // 
            this.trNum.Location = new System.Drawing.Point(272, 42);
            this.trNum.Name = "trNum";
            this.trNum.Size = new System.Drawing.Size(51, 21);
            this.trNum.TabIndex = 11;
            this.trNum.Text = "60";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "TR间隔（秒）:";
            // 
            // deviceType
            // 
            this.deviceType.Location = new System.Drawing.Point(101, 43);
            this.deviceType.Name = "deviceType";
            this.deviceType.Size = new System.Drawing.Size(69, 21);
            this.deviceType.TabIndex = 9;
            this.deviceType.Text = "33909";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Device Type:";
            // 
            // createExcel
            // 
            this.createExcel.Location = new System.Drawing.Point(856, 15);
            this.createExcel.Name = "createExcel";
            this.createExcel.Size = new System.Drawing.Size(75, 23);
            this.createExcel.TabIndex = 7;
            this.createExcel.Text = "生成";
            this.createExcel.UseVisualStyleBackColor = true;
            this.createExcel.Click += new System.EventHandler(this.createExcel_Click);
            // 
            // fileOpen
            // 
            this.fileOpen.Location = new System.Drawing.Point(690, 15);
            this.fileOpen.Name = "fileOpen";
            this.fileOpen.Size = new System.Drawing.Size(75, 23);
            this.fileOpen.TabIndex = 6;
            this.fileOpen.Text = "打开";
            this.fileOpen.UseVisualStyleBackColor = true;
            this.fileOpen.Click += new System.EventHandler(this.fileOpen_Click);
            // 
            // fileProcess
            // 
            this.fileProcess.Location = new System.Drawing.Point(772, 15);
            this.fileProcess.Name = "fileProcess";
            this.fileProcess.Size = new System.Drawing.Size(75, 23);
            this.fileProcess.TabIndex = 5;
            this.fileProcess.Text = "执行分析";
            this.fileProcess.UseVisualStyleBackColor = true;
            this.fileProcess.Click += new System.EventHandler(this.fileProcess_Click);
            // 
            // fileTextBox
            // 
            this.fileTextBox.Location = new System.Drawing.Point(236, 17);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.ReadOnly = true;
            this.fileTextBox.Size = new System.Drawing.Size(434, 21);
            this.fileTextBox.TabIndex = 4;
            // 
            // fileLable
            // 
            this.fileLable.AutoSize = true;
            this.fileLable.Location = new System.Drawing.Point(188, 21);
            this.fileLable.Name = "fileLable";
            this.fileLable.Size = new System.Drawing.Size(41, 12);
            this.fileLable.TabIndex = 3;
            this.fileLable.Text = "文件：";
            // 
            // typeComboBox
            // 
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Items.AddRange(new object[] {
            "1.CPU 使用分析",
            "2.DASD使用分析",
            "3.TTRN交易分析",
            "4.TRNR TPS分析"});
            this.typeComboBox.Location = new System.Drawing.Point(63, 18);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(108, 20);
            this.typeComboBox.TabIndex = 2;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(16, 21);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(41, 12);
            this.typeLabel.TabIndex = 0;
            this.typeLabel.Text = "类型：";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 546);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(975, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolFileOpen});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(975, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolFileOpen
            // 
            this.toolFileOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolFileOpen.Image = global::PRTools.Properties.Resources._16;
            this.toolFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolFileOpen.Name = "toolFileOpen";
            this.toolFileOpen.Size = new System.Drawing.Size(23, 22);
            this.toolFileOpen.Text = "fileOpen";
            this.toolFileOpen.Click += new System.EventHandler(this.toolFileOpen_Click);
            // 
            // fileText
            // 
            this.fileText.Location = new System.Drawing.Point(9, 159);
            this.fileText.Name = "fileText";
            this.fileText.ReadOnly = true;
            this.fileText.Size = new System.Drawing.Size(954, 357);
            this.fileText.TabIndex = 6;
            this.fileText.Text = "";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 520);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(954, 21);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(188, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "DASD（TOP）输出：";
            // 
            // dasdTopOutputVar
            // 
            this.dasdTopOutputVar.FormattingEnabled = true;
            this.dasdTopOutputVar.Items.AddRange(new object[] {
            "否",
            "是"});
            this.dasdTopOutputVar.Location = new System.Drawing.Point(298, 68);
            this.dasdTopOutputVar.Name = "dasdTopOutputVar";
            this.dasdTopOutputVar.Size = new System.Drawing.Size(45, 20);
            this.dasdTopOutputVar.TabIndex = 19;
            this.dasdTopOutputVar.Text = "是";
            // 
            // daseTopOutputLine
            // 
            this.daseTopOutputLine.Location = new System.Drawing.Point(349, 68);
            this.daseTopOutputLine.Name = "daseTopOutputLine";
            this.daseTopOutputLine.Size = new System.Drawing.Size(39, 21);
            this.daseTopOutputLine.TabIndex = 20;
            this.daseTopOutputLine.Text = "10";
            // 
            // prtools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 568);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.fileText);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.prMenuStrip);
            this.MainMenuStrip = this.prMenuStrip;
            this.Name = "prtools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PRTools";
            this.prMenuStrip.ResumeLayout(false);
            this.prMenuStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip prMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button fileProcess;
        private System.Windows.Forms.TextBox fileTextBox;
        private System.Windows.Forms.Label fileLable;
        private System.Windows.Forms.Button fileOpen;
        private System.Windows.Forms.Button createExcel;
        private System.Windows.Forms.RichTextBox fileText;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox deviceType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox trNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripButton toolFileOpen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox sortVar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox dasdFileLoadVar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBoxRows;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox dasdTopOutputVar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox daseTopOutputLine;
    }
}

