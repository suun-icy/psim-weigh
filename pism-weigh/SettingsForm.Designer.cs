namespace pism_weigh
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSystem = new System.Windows.Forms.TabPage();
            this.chkAutoConnect = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboPrinter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPrinter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.tabBasic = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDelOperator = new System.Windows.Forms.Button();
            this.btnAddOperator = new System.Windows.Forms.Button();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.lstOperator = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDelReceiver = new System.Windows.Forms.Button();
            this.btnAddReceiver = new System.Windows.Forms.Button();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.lstReceiver = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelCargo = new System.Windows.Forms.Button();
            this.btnAddCargo = new System.Windows.Forms.Button();
            this.txtCargo = new System.Windows.Forms.TextBox();
            this.lstCargo = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabSystem.SuspendLayout();
            this.tabBasic.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSystem);
            this.tabControl.Controls.Add(this.tabBasic);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(580, 380);
            this.tabControl.TabIndex = 0;
            // 
            // tabSystem
            // 
            this.tabSystem.Controls.Add(this.chkAutoConnect);
            this.tabSystem.Controls.Add(this.label4);
            this.tabSystem.Controls.Add(this.label3);
            this.tabSystem.Controls.Add(this.cboPrinter);
            this.tabSystem.Controls.Add(this.label2);
            this.tabSystem.Controls.Add(this.txtPrinter);
            this.tabSystem.Controls.Add(this.label1);
            this.tabSystem.Controls.Add(this.txtServerUrl);
            this.tabSystem.Location = new System.Drawing.Point(4, 22);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabSystem.Size = new System.Drawing.Size(572, 354);
            this.tabSystem.TabIndex = 0;
            this.tabSystem.Text = "系统配置";
            this.tabSystem.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器地址";
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(90, 21);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(350, 21);
            this.txtServerUrl.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "打印机名称";
            // 
            // txtPrinter
            // 
            this.txtPrinter.Location = new System.Drawing.Point(90, 56);
            this.txtPrinter.Name = "txtPrinter";
            this.txtPrinter.Size = new System.Drawing.Size(250, 21);
            this.txtPrinter.TabIndex = 3;
            // 
            // cboPrinter
            // 
            this.cboPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrinter.FormattingEnabled = true;
            this.cboPrinter.Location = new System.Drawing.Point(350, 56);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.Size = new System.Drawing.Size(90, 20);
            this.cboPrinter.TabIndex = 5;
            this.cboPrinter.Text = "选择打印机";
            this.cboPrinter.SelectedIndexChanged += new System.EventHandler(this.cboPrinter_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "（串口参数在主界面设置）";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "启动自动连接";
            // 
            // chkAutoConnect
            // 
            this.chkAutoConnect.AutoSize = true;
            this.chkAutoConnect.Location = new System.Drawing.Point(100, 129);
            this.chkAutoConnect.Name = "chkAutoConnect";
            this.chkAutoConnect.Size = new System.Drawing.Size(15, 14);
            this.chkAutoConnect.TabIndex = 8;
            this.chkAutoConnect.UseVisualStyleBackColor = true;
            // 
            // tabBasic
            // 
            this.tabBasic.Controls.Add(this.groupBox3);
            this.tabBasic.Controls.Add(this.groupBox2);
            this.tabBasic.Controls.Add(this.groupBox1);
            this.tabBasic.Location = new System.Drawing.Point(4, 22);
            this.tabBasic.Name = "tabBasic";
            this.tabBasic.Size = new System.Drawing.Size(572, 354);
            this.tabBasic.TabIndex = 1;
            this.tabBasic.Text = "基础数据";
            this.tabBasic.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDelCargo);
            this.groupBox1.Controls.Add(this.btnAddCargo);
            this.groupBox1.Controls.Add(this.txtCargo);
            this.groupBox1.Controls.Add(this.lstCargo);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 330);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "运输内容";
            // 
            // lstCargo
            // 
            this.lstCargo.FormattingEnabled = true;
            this.lstCargo.ItemHeight = 12;
            this.lstCargo.Location = new System.Drawing.Point(8, 20);
            this.lstCargo.Name = "lstCargo";
            this.lstCargo.Size = new System.Drawing.Size(158, 208);
            this.lstCargo.TabIndex = 0;
            this.lstCargo.DoubleClick += new System.EventHandler(this.lstCargo_DoubleClick);
            // 
            // txtCargo
            // 
            this.txtCargo.Location = new System.Drawing.Point(8, 240);
            this.txtCargo.Name = "txtCargo";
            this.txtCargo.Size = new System.Drawing.Size(120, 21);
            this.txtCargo.TabIndex = 1;
            // 
            // btnAddCargo
            // 
            this.btnAddCargo.Location = new System.Drawing.Point(132, 238);
            this.btnAddCargo.Name = "btnAddCargo";
            this.btnAddCargo.Size = new System.Drawing.Size(34, 23);
            this.btnAddCargo.TabIndex = 2;
            this.btnAddCargo.Text = "+";
            this.btnAddCargo.UseVisualStyleBackColor = true;
            this.btnAddCargo.Click += new System.EventHandler(this.btnAddCargo_Click);
            // 
            // btnDelCargo
            // 
            this.btnDelCargo.Location = new System.Drawing.Point(132, 268);
            this.btnDelCargo.Name = "btnDelCargo";
            this.btnDelCargo.Size = new System.Drawing.Size(34, 23);
            this.btnDelCargo.TabIndex = 3;
            this.btnDelCargo.Text = "-";
            this.btnDelCargo.UseVisualStyleBackColor = true;
            this.btnDelCargo.Click += new System.EventHandler(this.btnDelCargo_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDelReceiver);
            this.groupBox2.Controls.Add(this.btnAddReceiver);
            this.groupBox2.Controls.Add(this.txtReceiver);
            this.groupBox2.Controls.Add(this.lstReceiver);
            this.groupBox2.Location = new System.Drawing.Point(195, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(175, 330);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "收货单位";
            // 
            // lstReceiver
            // 
            this.lstReceiver.FormattingEnabled = true;
            this.lstReceiver.ItemHeight = 12;
            this.lstReceiver.Location = new System.Drawing.Point(8, 20);
            this.lstReceiver.Name = "lstReceiver";
            this.lstReceiver.Size = new System.Drawing.Size(158, 208);
            this.lstReceiver.TabIndex = 0;
            this.lstReceiver.DoubleClick += new System.EventHandler(this.lstReceiver_DoubleClick);
            // 
            // txtReceiver
            // 
            this.txtReceiver.Location = new System.Drawing.Point(8, 240);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(120, 21);
            this.txtReceiver.TabIndex = 1;
            // 
            // btnAddReceiver
            // 
            this.btnAddReceiver.Location = new System.Drawing.Point(132, 238);
            this.btnAddReceiver.Name = "btnAddReceiver";
            this.btnAddReceiver.Size = new System.Drawing.Size(34, 23);
            this.btnAddReceiver.TabIndex = 2;
            this.btnAddReceiver.Text = "+";
            this.btnAddReceiver.UseVisualStyleBackColor = true;
            this.btnAddReceiver.Click += new System.EventHandler(this.btnAddReceiver_Click);
            // 
            // btnDelReceiver
            // 
            this.btnDelReceiver.Location = new System.Drawing.Point(132, 268);
            this.btnDelReceiver.Name = "btnDelReceiver";
            this.btnDelReceiver.Size = new System.Drawing.Size(34, 23);
            this.btnDelReceiver.TabIndex = 3;
            this.btnDelReceiver.Text = "-";
            this.btnDelReceiver.UseVisualStyleBackColor = true;
            this.btnDelReceiver.Click += new System.EventHandler(this.btnDelReceiver_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDelOperator);
            this.groupBox3.Controls.Add(this.btnAddOperator);
            this.groupBox3.Controls.Add(this.txtOperator);
            this.groupBox3.Controls.Add(this.lstOperator);
            this.groupBox3.Location = new System.Drawing.Point(380, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(175, 330);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "司磅员";
            // 
            // lstOperator
            // 
            this.lstOperator.FormattingEnabled = true;
            this.lstOperator.ItemHeight = 12;
            this.lstOperator.Location = new System.Drawing.Point(8, 20);
            this.lstOperator.Name = "lstOperator";
            this.lstOperator.Size = new System.Drawing.Size(158, 208);
            this.lstOperator.TabIndex = 0;
            this.lstOperator.DoubleClick += new System.EventHandler(this.lstOperator_DoubleClick);
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(8, 240);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(120, 21);
            this.txtOperator.TabIndex = 1;
            // 
            // btnAddOperator
            // 
            this.btnAddOperator.Location = new System.Drawing.Point(132, 238);
            this.btnAddOperator.Name = "btnAddOperator";
            this.btnAddOperator.Size = new System.Drawing.Size(34, 23);
            this.btnAddOperator.TabIndex = 2;
            this.btnAddOperator.Text = "+";
            this.btnAddOperator.UseVisualStyleBackColor = true;
            this.btnAddOperator.Click += new System.EventHandler(this.btnAddOperator_Click);
            // 
            // btnDelOperator
            // 
            this.btnDelOperator.Location = new System.Drawing.Point(132, 268);
            this.btnDelOperator.Name = "btnDelOperator";
            this.btnDelOperator.Size = new System.Drawing.Size(34, 23);
            this.btnDelOperator.TabIndex = 3;
            this.btnDelOperator.Text = "-";
            this.btnDelOperator.UseVisualStyleBackColor = true;
            this.btnDelOperator.Click += new System.EventHandler(this.btnDelOperator_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(430, 400);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(516, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 442);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabSystem.ResumeLayout(false);
            this.tabSystem.PerformLayout();
            this.tabBasic.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSystem;
        private System.Windows.Forms.TabPage tabBasic;
        private System.Windows.Forms.CheckBox chkAutoConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboPrinter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPrinter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDelOperator;
        private System.Windows.Forms.Button btnAddOperator;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.ListBox lstOperator;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDelReceiver;
        private System.Windows.Forms.Button btnAddReceiver;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.ListBox lstReceiver;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDelCargo;
        private System.Windows.Forms.Button btnAddCargo;
        private System.Windows.Forms.TextBox txtCargo;
        private System.Windows.Forms.ListBox lstCargo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
