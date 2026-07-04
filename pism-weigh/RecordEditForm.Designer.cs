namespace pism_weigh
{
    partial class RecordEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBoxMain;
        private System.Windows.Forms.Label lblRecordId;
        private System.Windows.Forms.TextBox txtRecordId;
        private System.Windows.Forms.Label lblPlate;
        private System.Windows.Forms.TextBox txtPlate;
        private System.Windows.Forms.Label lblGrossWeight;
        private System.Windows.Forms.TextBox txtGrossWeight;
        private System.Windows.Forms.Label lblTareWeight;
        private System.Windows.Forms.TextBox txtTareWeight;
        private System.Windows.Forms.Label lblNetWeight;
        private System.Windows.Forms.TextBox txtNetWeight;
        private System.Windows.Forms.Label lblCargoType;
        private System.Windows.Forms.TextBox txtCargoType;
        private System.Windows.Forms.Label lblSender;
        private System.Windows.Forms.TextBox txtSender;
        private System.Windows.Forms.Label lblReceiver;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.Label lblDriverName;
        private System.Windows.Forms.TextBox txtDriverName;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.ComboBox cboOperator;
        private System.Windows.Forms.Label lblBusinessType;
        private System.Windows.Forms.ComboBox cboBusinessType;
        private System.Windows.Forms.Label lblRemark;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label lblFirstWeighTime;
        private System.Windows.Forms.TextBox txtFirstWeighTime;
        private System.Windows.Forms.Label lblSecondWeighTime;
        private System.Windows.Forms.TextBox txtSecondWeighTime;
        private System.Windows.Forms.Label lblCreateTime;
        private System.Windows.Forms.Label lblCompleteTime;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;

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
            this.groupBoxMain = new System.Windows.Forms.GroupBox();
            this.lblRecordId = new System.Windows.Forms.Label();
            this.txtRecordId = new System.Windows.Forms.TextBox();
            this.lblPlate = new System.Windows.Forms.Label();
            this.txtPlate = new System.Windows.Forms.TextBox();
            this.lblGrossWeight = new System.Windows.Forms.Label();
            this.txtGrossWeight = new System.Windows.Forms.TextBox();
            this.lblTareWeight = new System.Windows.Forms.Label();
            this.txtTareWeight = new System.Windows.Forms.TextBox();
            this.lblNetWeight = new System.Windows.Forms.Label();
            this.txtNetWeight = new System.Windows.Forms.TextBox();
            this.lblCargoType = new System.Windows.Forms.Label();
            this.txtCargoType = new System.Windows.Forms.TextBox();
            this.lblSender = new System.Windows.Forms.Label();
            this.txtSender = new System.Windows.Forms.TextBox();
            this.lblReceiver = new System.Windows.Forms.Label();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.lblDriverName = new System.Windows.Forms.Label();
            this.txtDriverName = new System.Windows.Forms.TextBox();
            this.lblOperator = new System.Windows.Forms.Label();
            this.cboOperator = new System.Windows.Forms.ComboBox();
            this.lblBusinessType = new System.Windows.Forms.Label();
            this.cboBusinessType = new System.Windows.Forms.ComboBox();
            this.lblRemark = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.lblFirstWeighTime = new System.Windows.Forms.Label();
            this.txtFirstWeighTime = new System.Windows.Forms.TextBox();
            this.lblSecondWeighTime = new System.Windows.Forms.Label();
            this.txtSecondWeighTime = new System.Windows.Forms.TextBox();
            this.lblCreateTime = new System.Windows.Forms.Label();
            this.lblCompleteTime = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBoxMain.SuspendLayout();
            this.SuspendLayout();

            // groupBoxMain
            this.groupBoxMain.Controls.Add(this.txtSecondWeighTime);
            this.groupBoxMain.Controls.Add(this.lblSecondWeighTime);
            this.groupBoxMain.Controls.Add(this.txtFirstWeighTime);
            this.groupBoxMain.Controls.Add(this.lblFirstWeighTime);
            this.groupBoxMain.Controls.Add(this.txtRemark);
            this.groupBoxMain.Controls.Add(this.lblRemark);
            this.groupBoxMain.Controls.Add(this.cboBusinessType);
            this.groupBoxMain.Controls.Add(this.lblBusinessType);
            this.groupBoxMain.Controls.Add(this.cboOperator);
            this.groupBoxMain.Controls.Add(this.lblOperator);
            this.groupBoxMain.Controls.Add(this.txtDriverName);
            this.groupBoxMain.Controls.Add(this.lblDriverName);
            this.groupBoxMain.Controls.Add(this.txtReceiver);
            this.groupBoxMain.Controls.Add(this.lblReceiver);
            this.groupBoxMain.Controls.Add(this.txtSender);
            this.groupBoxMain.Controls.Add(this.lblSender);
            this.groupBoxMain.Controls.Add(this.txtCargoType);
            this.groupBoxMain.Controls.Add(this.lblCargoType);
            this.groupBoxMain.Controls.Add(this.txtNetWeight);
            this.groupBoxMain.Controls.Add(this.lblNetWeight);
            this.groupBoxMain.Controls.Add(this.txtTareWeight);
            this.groupBoxMain.Controls.Add(this.lblTareWeight);
            this.groupBoxMain.Controls.Add(this.txtGrossWeight);
            this.groupBoxMain.Controls.Add(this.lblGrossWeight);
            this.groupBoxMain.Controls.Add(this.txtPlate);
            this.groupBoxMain.Controls.Add(this.lblPlate);
            this.groupBoxMain.Controls.Add(this.txtRecordId);
            this.groupBoxMain.Controls.Add(this.lblRecordId);
            this.groupBoxMain.Location = new System.Drawing.Point(12, 12);
            this.groupBoxMain.Name = "groupBoxMain";
            this.groupBoxMain.Size = new System.Drawing.Size(560, 370);
            this.groupBoxMain.Text = "称重记录编辑";
            this.groupBoxMain.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);

            // labels & textboxes (y offsets: 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330)
            int y = 30;
            AddFieldRow("编  号:", lblRecordId, txtRecordId, y, true);
            y += 30; AddFieldRow("车  牌:", lblPlate, txtPlate, y);
            y += 30; AddFieldRow("毛  重:", lblGrossWeight, txtGrossWeight, y);
            y += 30; AddFieldRow("皮  重:", lblTareWeight, txtTareWeight, y);
            y += 30; AddFieldRow("净  重:", lblNetWeight, txtNetWeight, y, true);
            y += 30; AddFieldRow("运输内容:", lblCargoType, txtCargoType, y);
            y += 30; AddFieldRow("发货单位:", lblSender, txtSender, y);
            y += 30; AddFieldRow("收货单位:", lblReceiver, txtReceiver, y);
            y += 30; AddFieldRow("司  机:", lblDriverName, txtDriverName, y);
            y += 30; AddFieldRow("司磅员:", lblOperator, cboOperator, y);
            y += 30; AddFieldRow("业务类型:", lblBusinessType, cboBusinessType, y, cbo: true);
            y += 30; AddFieldRow("备  注:", lblRemark, txtRemark, y);

            this.txtRecordId.ReadOnly = true;
            this.txtRecordId.BackColor = System.Drawing.SystemColors.Control;
            this.txtNetWeight.ReadOnly = true;
            this.txtNetWeight.BackColor = System.Drawing.SystemColors.Control;

            // first/second weigh time
            this.lblFirstWeighTime.Text = "首次称重:";
            this.lblFirstWeighTime.Location = new System.Drawing.Point(10, y + 10);
            this.lblFirstWeighTime.Size = new System.Drawing.Size(65, 24);
            this.txtFirstWeighTime.Location = new System.Drawing.Point(80, y + 10);
            this.txtFirstWeighTime.Size = new System.Drawing.Size(190, 24);
            y += 30;

            this.lblSecondWeighTime.Text = "二次称重:";
            this.lblSecondWeighTime.Location = new System.Drawing.Point(10, y + 10);
            this.lblSecondWeighTime.Size = new System.Drawing.Size(65, 24);
            this.txtSecondWeighTime.Location = new System.Drawing.Point(80, y + 10);
            this.txtSecondWeighTime.Size = new System.Drawing.Size(190, 24);

            // time labels
            this.lblCreateTime.Location = new System.Drawing.Point(20, 392);
            this.lblCreateTime.Size = new System.Drawing.Size(260, 24);
            this.lblCompleteTime.Location = new System.Drawing.Point(300, 392);
            this.lblCompleteTime.Size = new System.Drawing.Size(260, 24);

            // buttons
            this.btnSave.Text = "保存修改";
            this.btnSave.Location = new System.Drawing.Point(200, 426);
            this.btnSave.Size = new System.Drawing.Size(90, 32);
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            this.btnCancel.Text = "作废记录";
            this.btnCancel.Location = new System.Drawing.Point(300, 426);
            this.btnCancel.Size = new System.Drawing.Size(90, 32);
            this.btnCancel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            this.btnClose.Text = "关闭";
            this.btnClose.Location = new System.Drawing.Point(460, 426);
            this.btnClose.Size = new System.Drawing.Size(90, 32);
            this.btnClose.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btnClose.Click += new System.EventHandler((s, ev) => this.Close());

            // Form
            this.ClientSize = new System.Drawing.Size(584, 470);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblCompleteTime);
            this.Controls.Add(this.lblCreateTime);
            this.Controls.Add(this.groupBoxMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "编辑称重记录";
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.groupBoxMain.ResumeLayout(false);
            this.groupBoxMain.PerformLayout();
            this.ResumeLayout(false);
        }

        private void AddFieldRow(string labelText, System.Windows.Forms.Label lbl, System.Windows.Forms.Control ctrl, int y, bool readOnly = false, bool cbo = false)
        {
            lbl.Text = labelText;
            lbl.Location = new System.Drawing.Point(10, y + 3);
            lbl.Size = new System.Drawing.Size(65, 24);

            if (cbo)
            {
                var combo = ctrl as System.Windows.Forms.ComboBox;
                combo.Location = new System.Drawing.Point(80, y);
                combo.Size = new System.Drawing.Size(200, 24);
                combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            }
            else
            {
                var txt = ctrl as System.Windows.Forms.TextBox;
                txt.Location = new System.Drawing.Point(80, y);
                txt.Size = new System.Drawing.Size(200, 24);
                if (readOnly)
                {
                    txt.ReadOnly = true;
                    txt.BackColor = System.Drawing.SystemColors.Control;
                }
            }
        }
    }
}
