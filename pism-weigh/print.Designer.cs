namespace pism_weigh
{
    partial class print
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
            this.panelFilter = new System.Windows.Forms.Panel();
            this.labelCount = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.comboBoxBusinessType = new System.Windows.Forms.ComboBox();
            this.textBoxPlate = new System.Windows.Forms.TextBox();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewRecords = new System.Windows.Forms.DataGridView();
            this.colPlate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBusinessType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGross = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTare = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrintCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelAction = new System.Windows.Forms.Panel();
            this.buttonReprint = new System.Windows.Forms.Button();
            this.buttonDetail = new System.Windows.Forms.Button();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecords)).BeginInit();
            this.panelAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFilter
            // 
            this.panelFilter.Controls.Add(this.labelCount);
            this.panelFilter.Controls.Add(this.buttonReset);
            this.panelFilter.Controls.Add(this.buttonSearch);
            this.panelFilter.Controls.Add(this.comboBoxStatus);
            this.panelFilter.Controls.Add(this.comboBoxBusinessType);
            this.panelFilter.Controls.Add(this.textBoxPlate);
            this.panelFilter.Controls.Add(this.dateTimePickerEnd);
            this.panelFilter.Controls.Add(this.dateTimePickerStart);
            this.panelFilter.Controls.Add(this.label4);
            this.panelFilter.Controls.Add(this.label3);
            this.panelFilter.Controls.Add(this.label2);
            this.panelFilter.Controls.Add(this.label1);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1100, 64);
            this.panelFilter.TabIndex = 0;
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(1007, 26);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(53, 12);
            this.labelCount.TabIndex = 11;
            this.labelCount.Text = "记录数：0";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(928, 21);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(62, 23);
            this.buttonReset.TabIndex = 10;
            this.buttonReset.Text = "重置";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(860, 21);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(62, 23);
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "查询";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // comboBoxStatus
            // 
            this.comboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStatus.FormattingEnabled = true;
            this.comboBoxStatus.Location = new System.Drawing.Point(736, 23);
            this.comboBoxStatus.Name = "comboBoxStatus";
            this.comboBoxStatus.Size = new System.Drawing.Size(108, 20);
            this.comboBoxStatus.TabIndex = 8;
            // 
            // comboBoxBusinessType
            // 
            this.comboBoxBusinessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBusinessType.FormattingEnabled = true;
            this.comboBoxBusinessType.Location = new System.Drawing.Point(585, 23);
            this.comboBoxBusinessType.Name = "comboBoxBusinessType";
            this.comboBoxBusinessType.Size = new System.Drawing.Size(108, 20);
            this.comboBoxBusinessType.TabIndex = 7;
            // 
            // textBoxPlate
            // 
            this.textBoxPlate.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxPlate.Location = new System.Drawing.Point(449, 23);
            this.textBoxPlate.Name = "textBoxPlate";
            this.textBoxPlate.Size = new System.Drawing.Size(108, 21);
            this.textBoxPlate.TabIndex = 6;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(230, 23);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(104, 21);
            this.dateTimePickerEnd.TabIndex = 5;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(96, 23);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(104, 21);
            this.dateTimePickerStart.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(701, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "状态";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(514, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "业务类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(390, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "车牌号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "日期范围";
            // 
            // dataGridViewRecords
            // 
            this.dataGridViewRecords.AllowUserToAddRows = false;
            this.dataGridViewRecords.AllowUserToDeleteRows = false;
            this.dataGridViewRecords.AutoGenerateColumns = false;
            this.dataGridViewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPlate,
            this.colBusinessType,
            this.colStatus,
            this.colGross,
            this.colTare,
            this.colNet,
            this.colPrintCount,
            this.colCreateTime});
            this.dataGridViewRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRecords.Location = new System.Drawing.Point(0, 64);
            this.dataGridViewRecords.MultiSelect = false;
            this.dataGridViewRecords.Name = "dataGridViewRecords";
            this.dataGridViewRecords.ReadOnly = true;
            this.dataGridViewRecords.RowTemplate.Height = 23;
            this.dataGridViewRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRecords.Size = new System.Drawing.Size(1100, 467);
            this.dataGridViewRecords.TabIndex = 1;
            // 
            // colPlate
            // 
            this.colPlate.DataPropertyName = "PlateNumber";
            this.colPlate.HeaderText = "车牌号";
            this.colPlate.Name = "colPlate";
            this.colPlate.ReadOnly = true;
            this.colPlate.Width = 120;
            // 
            // colBusinessType
            // 
            this.colBusinessType.DataPropertyName = "BusinessType";
            this.colBusinessType.HeaderText = "业务类型";
            this.colBusinessType.Name = "colBusinessType";
            this.colBusinessType.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "状态";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colGross
            // 
            this.colGross.DataPropertyName = "GrossWeight";
            this.colGross.HeaderText = "毛重";
            this.colGross.Name = "colGross";
            this.colGross.ReadOnly = true;
            // 
            // colTare
            // 
            this.colTare.DataPropertyName = "TareWeight";
            this.colTare.HeaderText = "皮重";
            this.colTare.Name = "colTare";
            this.colTare.ReadOnly = true;
            // 
            // colNet
            // 
            this.colNet.DataPropertyName = "NetWeight";
            this.colNet.HeaderText = "净重";
            this.colNet.Name = "colNet";
            this.colNet.ReadOnly = true;
            // 
            // colPrintCount
            // 
            this.colPrintCount.DataPropertyName = "PrintCount";
            this.colPrintCount.HeaderText = "打印次数";
            this.colPrintCount.Name = "colPrintCount";
            this.colPrintCount.ReadOnly = true;
            // 
            // colCreateTime
            // 
            this.colCreateTime.DataPropertyName = "CreateTime";
            this.colCreateTime.HeaderText = "创建时间";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.ReadOnly = true;
            this.colCreateTime.Width = 180;
            // 
            // panelAction
            // 
            this.panelAction.Controls.Add(this.buttonReprint);
            this.panelAction.Controls.Add(this.buttonDetail);
            this.panelAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelAction.Location = new System.Drawing.Point(0, 531);
            this.panelAction.Name = "panelAction";
            this.panelAction.Size = new System.Drawing.Size(1100, 49);
            this.panelAction.TabIndex = 2;
            // 
            // buttonReprint
            // 
            this.buttonReprint.Location = new System.Drawing.Point(113, 13);
            this.buttonReprint.Name = "buttonReprint";
            this.buttonReprint.Size = new System.Drawing.Size(95, 26);
            this.buttonReprint.TabIndex = 1;
            this.buttonReprint.Text = "重复打印";
            this.buttonReprint.UseVisualStyleBackColor = true;
            this.buttonReprint.Click += new System.EventHandler(this.buttonReprint_Click);
            // 
            // buttonDetail
            // 
            this.buttonDetail.Location = new System.Drawing.Point(12, 13);
            this.buttonDetail.Name = "buttonDetail";
            this.buttonDetail.Size = new System.Drawing.Size(95, 26);
            this.buttonDetail.TabIndex = 0;
            this.buttonDetail.Text = "查看详情";
            this.buttonDetail.UseVisualStyleBackColor = true;
            this.buttonDetail.Click += new System.EventHandler(this.buttonDetail_Click);
            // 
            // print
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 580);
            this.Controls.Add(this.dataGridViewRecords);
            this.Controls.Add(this.panelAction);
            this.Controls.Add(this.panelFilter);
            this.Name = "print";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "称重记录";
            this.Load += new System.EventHandler(this.print_Load);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecords)).EndInit();
            this.panelAction.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.ComboBox comboBoxStatus;
        private System.Windows.Forms.ComboBox comboBoxBusinessType;
        private System.Windows.Forms.TextBox textBoxPlate;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.DataGridView dataGridViewRecords;
        private System.Windows.Forms.Panel panelAction;
        private System.Windows.Forms.Button buttonReprint;
        private System.Windows.Forms.Button buttonDetail;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPlate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBusinessType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGross;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTare;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrintCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreateTime;
    }
}
