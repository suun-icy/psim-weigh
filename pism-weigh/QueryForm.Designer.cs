namespace pism_weigh
{
    partial class QueryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelFilter = new System.Windows.Forms.Panel();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.txtSender = new System.Windows.Forms.TextBox();
            this.txtCargo = new System.Windows.Forms.TextBox();
            this.txtDriver = new System.Windows.Forms.TextBox();
            this.txtPlate = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnReprint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelFilter.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFilter
            // 
            this.panelFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFilter.Controls.Add(this.btnReset);
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.dtpEnd);
            this.panelFilter.Controls.Add(this.dtpStart);
            this.panelFilter.Controls.Add(this.label8);
            this.panelFilter.Controls.Add(this.label7);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.label6);
            this.panelFilter.Controls.Add(this.label5);
            this.panelFilter.Controls.Add(this.label4);
            this.panelFilter.Controls.Add(this.label3);
            this.panelFilter.Controls.Add(this.label2);
            this.panelFilter.Controls.Add(this.label1);
            this.panelFilter.Controls.Add(this.txtReceiver);
            this.panelFilter.Controls.Add(this.txtSender);
            this.panelFilter.Controls.Add(this.txtCargo);
            this.panelFilter.Controls.Add(this.txtDriver);
            this.panelFilter.Controls.Add(this.txtPlate);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(900, 120);
            this.panelFilter.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "车牌";
            // 
            // txtPlate
            // 
            this.txtPlate.Location = new System.Drawing.Point(55, 14);
            this.txtPlate.Name = "txtPlate";
            this.txtPlate.Size = new System.Drawing.Size(130, 21);
            this.txtPlate.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "司机";
            // 
            // txtDriver
            // 
            this.txtDriver.Location = new System.Drawing.Point(240, 14);
            this.txtDriver.Name = "txtDriver";
            this.txtDriver.Size = new System.Drawing.Size(130, 21);
            this.txtDriver.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(385, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "运输内容";
            // 
            // txtCargo
            // 
            this.txtCargo.Location = new System.Drawing.Point(445, 14);
            this.txtCargo.Name = "txtCargo";
            this.txtCargo.Size = new System.Drawing.Size(130, 21);
            this.txtCargo.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "发货";
            // 
            // txtSender
            // 
            this.txtSender.Location = new System.Drawing.Point(55, 48);
            this.txtSender.Name = "txtSender";
            this.txtSender.Size = new System.Drawing.Size(130, 21);
            this.txtSender.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(200, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "收货";
            // 
            // txtReceiver
            // 
            this.txtReceiver.Location = new System.Drawing.Point(240, 48);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(130, 21);
            this.txtReceiver.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(385, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "状态";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Items.AddRange(new object[] {
            "全部",
            "首次称重",
            "二次称重",
            "已完成",
            "已取消"});
            this.cboStatus.Location = new System.Drawing.Point(445, 48);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(130, 20);
            this.cboStatus.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(590, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "开始日期";
            // 
            // dtpStart
            // 
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(650, 14);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(110, 21);
            this.dtpStart.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(590, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "结束日期";
            // 
            // dtpEnd
            // 
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(650, 48);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(110, 21);
            this.dtpEnd.TabIndex = 15;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(650, 82);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 28);
            this.btnSearch.TabIndex = 16;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(770, 82);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 28);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.dgvRecords);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 120);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(8);
            this.panelMain.Size = new System.Drawing.Size(900, 400);
            this.panelMain.TabIndex = 1;
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToOrderColumns = true;
            this.dgvRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(8, 8);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.ReadOnly = true;
            this.dgvRecords.RowTemplate.Height = 23;
            this.dgvRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecords.Size = new System.Drawing.Size(884, 384);
            this.dgvRecords.TabIndex = 0;
            this.dgvRecords.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRecords_CellDoubleClick);
            // 
            // panelBottom
            // 
            this.panelBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Controls.Add(this.btnReprint);
            this.panelBottom.Controls.Add(this.btnDetail);
            this.panelBottom.Controls.Add(this.lblSummary);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 520);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(900, 50);
            this.panelBottom.TabIndex = 2;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
            this.lblSummary.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblSummary.Location = new System.Drawing.Point(15, 17);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(79, 14);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "暂无数据";
            // 
            // btnDetail
            // 
            this.btnDetail.Location = new System.Drawing.Point(650, 12);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(75, 28);
            this.btnDetail.TabIndex = 1;
            this.btnDetail.Text = "详情";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnReprint
            // 
            this.btnReprint.Location = new System.Drawing.Point(735, 12);
            this.btnReprint.Name = "btnReprint";
            this.btnReprint.Size = new System.Drawing.Size(75, 28);
            this.btnReprint.TabIndex = 2;
            this.btnReprint.Text = "重打";
            this.btnReprint.UseVisualStyleBackColor = true;
            this.btnReprint.Click += new System.EventHandler(this.btnReprint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(820, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(65, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 570);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelFilter);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.Name = "QueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "称重记录查询与分析";
            this.Load += new System.EventHandler(this.QueryForm_Load);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.TextBox txtPlate;
        private System.Windows.Forms.TextBox txtDriver;
        private System.Windows.Forms.TextBox txtCargo;
        private System.Windows.Forms.TextBox txtSender;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button btnReprint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}
