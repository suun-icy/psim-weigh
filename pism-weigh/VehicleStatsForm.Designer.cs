using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh
{
    partial class VehicleStatsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private DataGridView dgvStats;
        private DateTimePicker dtpStart, dtpEnd;
        private Button btnSearch, btnExport, btnClose;
        private Label lblCount, lblSummary;

        private void InitializeComponent()
        {
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.ClientSize = new Size(1000, 600);
            this.Text = "车辆统计分析";
            this.StartPosition = FormStartPosition.CenterScreen;

            var pnlTop = new Panel { Location = new Point(12, 12), Size = new Size(976, 36) };

            var lblStart = new Label { Text = "开始:", Location = new Point(0, 10), Size = new Size(40, 20) };
            dtpStart = new DateTimePicker { Location = new Point(42, 7), Size = new Size(110, 24), Format = DateTimePickerFormat.Short };
            var lblEnd = new Label { Text = "结束:", Location = new Point(160, 10), Size = new Size(40, 20) };
            dtpEnd = new DateTimePicker { Location = new Point(202, 7), Size = new Size(110, 24), Format = DateTimePickerFormat.Short };
            btnSearch = new Button { Text = "查询", Location = new Point(320, 6), Size = new Size(65, 28), UseVisualStyleBackColor = true };
            btnExport = new Button { Text = "导出CSV", Location = new Point(395, 6), Size = new Size(80, 28), UseVisualStyleBackColor = true };
            lblCount = new Label { Text = "共 0 辆车", Location = new Point(490, 10), Size = new Size(120, 20) };
            lblSummary = new Label { Text = "", Location = new Point(620, 10), Size = new Size(300, 20), ForeColor = Color.DarkBlue };
            btnClose = new Button { Text = "关闭", Location = new Point(900, 6), Size = new Size(65, 28), UseVisualStyleBackColor = true };

            pnlTop.Controls.AddRange(new Control[] { lblStart, dtpStart, lblEnd, dtpEnd, btnSearch, btnExport, lblCount, lblSummary, btnClose });
            Controls.Add(pnlTop);

            dgvStats = new DataGridView
            {
                Location = new Point(12, 55),
                Size = new Size(976, 530),
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            Controls.Add(dgvStats);

            Services.UIStyler.StyleDataGridView(dgvStats);

            btnSearch.Click += btnSearch_Click;
            btnExport.Click += btnExport_Click;
            btnClose.Click += btnClose_Click;
            this.Load += VehicleStatsForm_Load;
        }
    }
}
