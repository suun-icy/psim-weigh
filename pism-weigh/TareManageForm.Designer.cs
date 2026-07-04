using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh
{
    partial class TareManageForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Label lblPlate;
        private Label lblCurrentTare;
        private Label lblWeight;
        private Label lblRemark;
        private TextBox txtTareWeight;
        private TextBox txtRemark;
        private Button btnAdd;
        private Button btnFromHistory;
        private Button btnDelete;
        private Button btnClose;
        private DataGridView dgvRecords;

        private void InitializeComponent()
        {
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.ClientSize = new Size(580, 420);
            this.Text = "车辆皮重管理";
            this.StartPosition = FormStartPosition.CenterParent;

            lblPlate = new Label
            {
                Text = "车牌: -",
                Location = new Point(12, 12),
                Size = new Size(200, 24),
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold)
            };
            Controls.Add(lblPlate);

            lblCurrentTare = new Label
            {
                Text = "当前预设皮重: 暂无",
                Location = new Point(230, 12),
                Size = new Size(340, 24),
                ForeColor = Color.DarkBlue
            };
            Controls.Add(lblCurrentTare);

            var pnlAdd = new Panel { Location = new Point(12, 45), Size = new Size(556, 60), BorderStyle = BorderStyle.FixedSingle };

            lblWeight = new Label { Text = "皮重(kg):", Location = new Point(8, 20), Size = new Size(60, 24) };
            Controls.Add(lblWeight); // note: need to add to panel, let me fix
            lblWeight.Parent = pnlAdd;
            lblWeight.Location = new Point(8, 20);

            txtTareWeight = new TextBox { Location = new Point(75, 18), Size = new Size(100, 24) };
            txtTareWeight.Parent = pnlAdd;

            lblRemark = new Label { Text = "备注:", Location = new Point(185, 20), Size = new Size(40, 24) };
            lblRemark.Parent = pnlAdd;

            txtRemark = new TextBox { Location = new Point(230, 18), Size = new Size(150, 24) };
            txtRemark.Parent = pnlAdd;

            btnAdd = new Button { Text = "添加皮重", Location = new Point(390, 16), Size = new Size(75, 28), UseVisualStyleBackColor = true };
            btnAdd.Parent = pnlAdd;
            btnAdd.Click += btnAdd_Click;

            btnFromHistory = new Button { Text = "从历史提取", Location = new Point(470, 16), Size = new Size(80, 28), UseVisualStyleBackColor = true };
            btnFromHistory.Parent = pnlAdd;
            btnFromHistory.Click += btnFromHistory_Click;

            Controls.Add(pnlAdd);

            dgvRecords = new DataGridView
            {
                Location = new Point(12, 115),
                Size = new Size(556, 255),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            Controls.Add(dgvRecords);

            btnDelete = new Button { Text = "删除选中", Location = new Point(12, 378), Size = new Size(80, 30), UseVisualStyleBackColor = true };
            btnDelete.Click += btnDelete_Click;
            Controls.Add(btnDelete);

            btnClose = new Button { Text = "关闭", Location = new Point(488, 378), Size = new Size(80, 30), UseVisualStyleBackColor = true };
            btnClose.Click += btnClose_Click;
            Controls.Add(btnClose);

            Services.UIStyler.StyleDataGridView(dgvRecords);

            this.Load += TareManageForm_Load;
        }
    }
}
