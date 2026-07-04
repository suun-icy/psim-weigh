using System;
using System.Drawing;
using System.Windows.Forms;
using pism_weigh.Database;

namespace pism_weigh
{
    /// <summary>
    /// 车辆进出场记录查询窗口
    /// </summary>
    public partial class VehicleLogForm : Form
    {
        public VehicleLogForm()
        {
            InitializeComponent();
        }

        private void VehicleLogForm_Load(object sender, EventArgs e)
        {
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            RefreshList();
        }

        private void RefreshList()
        {
            var logs = DatabaseHelper.GetVehicleLogs(dtpStart.Value, dtpEnd.Value, txtPlate.Text.Trim());
            dgvLogs.DataSource = null;
            dgvLogs.DataSource = logs;
            lblCount.Text = "共 " + logs.Count + " 条记录";
            lblActive.Text = "当前在场: " + DatabaseHelper.GetActiveVehicleCount() + " 辆";
        }

        private void btnSearch_Click(object sender, EventArgs e) { RefreshList(); }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }
    }
}
