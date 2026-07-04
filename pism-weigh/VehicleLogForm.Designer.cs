using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh
{
    partial class VehicleLogForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private DataGridView dgvLogs;
        private DateTimePicker dtpStart, dtpEnd;
        private TextBox txtPlate;
        private Button btnSearch, btnClose;
        private Label lblCount, lblActive;

        private void InitializeComponent()
        {
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.ClientSize = new Size(900, 550);
            this.Text = "车辆进出场记录";
            this.StartPosition = FormStartPosition.CenterScreen;

            var pnlTop = new Panel { Location = new Point(12, 12), Size = new Size(876, 36) };

            var lblStart = new Label { Text = "开始:", Location = new Point(0, 10), Size = new Size(40, 20) };
            dtpStart = new DateTimePicker { Location = new Point(42, 7), Size = new Size(110, 24), Format = DateTimePickerFormat.Short };
            var lblEnd = new Label { Text = "结束:", Location = new Point(160, 10), Size = new Size(40, 20) };
            dtpEnd = new DateTimePicker { Location = new Point(202, 7), Size = new Size(110, 24), Format = DateTimePickerFormat.Short };
            var lblPlate = new Label { Text = "车牌:", Location = new Point(320, 10), Size = new Size(40, 20) };
            txtPlate = new TextBox { Location = new Point(362, 7), Size = new Size(100, 24) };
            btnSearch = new Button { Text = "搜索", Location = new Point(470, 6), Size = new Size(65, 28), UseVisualStyleBackColor = true };
            btnSearch.Click += btnSearch_Click;

            lblCount = new Label { Text = "共 0 条记录", Location = new Point(540, 10), Size = new Size(120, 20) };
            lblActive = new Label { Text = "当前在场: 0 辆", Location = new Point(660, 10), Size = new Size(120, 20), ForeColor = Color.DarkGreen };
            btnClose = new Button { Text = "关闭", Location = new Point(800, 6), Size = new Size(65, 28), UseVisualStyleBackColor = true };
            btnClose.Click += btnClose_Click;

            pnlTop.Controls.AddRange(new Control[] { lblStart, dtpStart, lblEnd, dtpEnd, lblPlate, txtPlate, btnSearch, lblCount, lblActive, btnClose });
            Controls.Add(pnlTop);

            dgvLogs = new DataGridView
            {
                Location = new Point(12, 55),
                Size = new Size(876, 480),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            Controls.Add(dgvLogs);

            this.Load += VehicleLogForm_Load;
        }
    }
}
