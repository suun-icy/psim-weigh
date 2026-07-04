using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh
{
    partial class VehicleForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private DataGridView dgvVehicles;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnReset;
        private Button btnImport;
        private Button btnClose;
        private Label lblCount;

        private GroupBox groupEdit;
        private Label lblPlate, lblVehicleType, lblBrandModel, lblRatedLoad, lblCurbWeight;
        private Label lblOwnerName, lblOwnerPhone, lblOwnerUnit;
        private Label lblFuelType, lblEmissionStandard, lblRegDate, lblRemarkE;
        private TextBox txtPlate, txtBrandModel, txtRatedLoad, txtCurbWeight;
        private TextBox txtOwnerName, txtOwnerPhone, txtOwnerUnit, txtRemark;
        private ComboBox txtVehicleType, txtFuelType, txtEmissionStandard;
        private DateTimePicker dtpRegisteredDate;
        private Label lblStatus;
        private Button btnSave, btnDelete;

        private void InitializeComponent()
        {
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.ClientSize = new Size(960, 620);
            this.Text = "车辆档案管理";
            this.StartPosition = FormStartPosition.CenterScreen;

            // ===== 搜索栏 =====
            var pnlSearch = new Panel { Location = new Point(12, 12), Size = new Size(936, 36) };
            var lblSearch = new Label { Text = "搜索:", Location = new Point(0, 10), Size = new Size(40, 20) };
            txtSearch = new TextBox { Location = new Point(45, 7), Size = new Size(200, 24) };
            btnSearch = new Button { Text = "搜索", Location = new Point(252, 6), Size = new Size(60, 28), UseVisualStyleBackColor = true };
            btnReset = new Button { Text = "重置", Location = new Point(318, 6), Size = new Size(60, 28), UseVisualStyleBackColor = true };
            btnImport = new Button { Text = "从称重记录导入", Location = new Point(470, 6), Size = new Size(120, 28), UseVisualStyleBackColor = true };
            var btnStats = new Button { Text = "车辆统计", Location = new Point(596, 6), Size = new Size(80, 28), UseVisualStyleBackColor = true };
            lblCount = new Label { Text = "共 0 辆车", Location = new Point(690, 10), Size = new Size(100, 20) };
            btnClose = new Button { Text = "关闭", Location = new Point(860, 6), Size = new Size(65, 28), UseVisualStyleBackColor = true };
            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnReset, btnImport, btnStats, lblCount, btnClose });
            btnSearch.Click += (s, e) => RefreshList();
            btnReset.Click += (s, e) => { txtSearch.Text = ""; RefreshList(); };
            btnImport.Click += btnImport_Click;
            btnStats.Click += (s, e) => { new VehicleStatsForm().ShowDialog(); };
            btnClose.Click += btnClose_Click;
            Controls.Add(pnlSearch);

            // ===== 左侧列表 =====
            dgvVehicles = new DataGridView
            {
                Location = new Point(12, 55),
                Size = new Size(560, 545),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgvVehicles.SelectionChanged += dgvVehicles_SelectionChanged;
            Controls.Add(dgvVehicles);

            // ===== 右侧编辑区 =====
            groupEdit = new GroupBox
            {
                Text = "车辆信息",
                Location = new Point(582, 55),
                Size = new Size(366, 545),
                Font = new Font("Microsoft YaHei UI", 9F)
            };

            int y = 28;
            var w1 = 75; var w2 = 260;
            lblPlate = AddLabelRow("车牌号*", 12, ref y, w1);
            txtPlate = AddTextBox(92, y - 24, w2); y += 34;

            lblVehicleType = AddLabelRow("车辆类型", 12, ref y, w1);
            txtVehicleType = AddCombo(92, y - 24, w2, "货车", "挂车", "罐车", "自卸车", "平板车", "厢式车", "搅拌车", "其他"); y += 34;

            lblBrandModel = AddLabelRow("品牌型号", 12, ref y, w1);
            txtBrandModel = AddTextBox(92, y - 24, w2); y += 34;

            lblRatedLoad = AddLabelRow("核定载重(吨)", 12, ref y, w1);
            txtRatedLoad = AddTextBox(92, y - 24, w2); y += 34;

            lblCurbWeight = AddLabelRow("整备质量(吨)", 12, ref y, w1);
            txtCurbWeight = AddTextBox(92, y - 24, w2); y += 34;

            lblOwnerName = AddLabelRow("车主姓名", 12, ref y, w1);
            txtOwnerName = AddTextBox(92, y - 24, w2); y += 34;

            lblOwnerPhone = AddLabelRow("车主电话", 12, ref y, w1);
            txtOwnerPhone = AddTextBox(92, y - 24, w2); y += 34;

            lblOwnerUnit = AddLabelRow("所属单位", 12, ref y, w1);
            txtOwnerUnit = AddTextBox(92, y - 24, w2); y += 34;

            lblFuelType = AddLabelRow("燃油类型", 12, ref y, w1);
            txtFuelType = AddCombo(92, y - 24, w2, "柴油", "汽油", "电动", "天然气", "混合动力"); y += 34;

            lblEmissionStandard = AddLabelRow("排放标准", 12, ref y, w1);
            txtEmissionStandard = AddCombo(92, y - 24, w2, "国三", "国四", "国五", "国六"); y += 34;

            lblRegDate = AddLabelRow("注册日期", 12, ref y, w1);
            dtpRegisteredDate = new DateTimePicker { Location = new Point(92, y - 24), Size = new Size(w2, 24), Format = DateTimePickerFormat.Short };
            groupEdit.Controls.Add(dtpRegisteredDate); y += 34;

            lblRemarkE = AddLabelRow("备注", 12, ref y, w1);
            txtRemark = AddTextBox(92, y - 24, w2); y += 34;

            lblStatus = new Label { Location = new Point(12, y), Size = new Size(200, 24), Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold) };
            groupEdit.Controls.Add(lblStatus); y += 30;

            btnSave = new Button { Text = "添加车辆", Location = new Point(12, y), Size = new Size(100, 32), UseVisualStyleBackColor = true };
            btnSave.Click += btnSave_Click;
            groupEdit.Controls.Add(btnSave);

            btnDelete = new Button { Text = "删除", Location = new Point(120, y), Size = new Size(70, 32), UseVisualStyleBackColor = true, Visible = false, ForeColor = Color.Red };
            btnDelete.Click += btnDelete_Click;
            groupEdit.Controls.Add(btnDelete);

            var btnTare = new Button { Text = "皮重管理", Location = new Point(200, y), Size = new Size(80, 32), UseVisualStyleBackColor = true };
            btnTare.Click += (s, e) => { if (!string.IsNullOrWhiteSpace(txtPlate.Text)) new TareManageForm(txtPlate.Text.Trim()).ShowDialog(); };
            groupEdit.Controls.Add(btnTare);

            var btnClear = new Button { Text = "清除", Location = new Point(290, y), Size = new Size(70, 32), UseVisualStyleBackColor = true };
            btnClear.Click += (s, e) => { dgvVehicles.ClearSelection(); ClearEdit(); };
            groupEdit.Controls.Add(btnClear);

            // Photo
            var btnPhoto = new Button { Text = "照片", Location = new Point(12, y), Size = new Size(50, 32), UseVisualStyleBackColor = true };
            btnPhoto.Click += btnPhoto_Click;
            groupEdit.Controls.Add(btnPhoto);

            var picVehicle = new PictureBox
            {
                Location = new Point(280, y + 40),
                Size = new Size(80, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            groupEdit.Controls.Add(picVehicle);
            picVehicle.Name = "picVehicle";

            Controls.Add(groupEdit);

            this.Load += VehicleForm_Load;
        }

        private Label AddLabelRow(string text, int x, ref int y, int w)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y), Size = new Size(w, 24) };
            groupEdit.Controls.Add(lbl);
            y += 24;
            return lbl;
        }

        private TextBox AddTextBox(int x, int y, int w)
        {
            var txt = new TextBox { Location = new Point(x, y), Size = new Size(w, 24) };
            groupEdit.Controls.Add(txt);
            return txt;
        }

        private ComboBox AddCombo(int x, int y, int w, params string[] items)
        {
            var cbo = new ComboBox { Location = new Point(x, y), Size = new Size(w, 24), DropDownStyle = ComboBoxStyle.DropDown };
            cbo.Items.AddRange(items);
            groupEdit.Controls.Add(cbo);
            return cbo;
        }
    }
}
