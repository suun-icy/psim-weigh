using System;
using System.Collections.Generic;
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
            TranslateColumns(dgvLogs);
            lblCount.Text = "共 " + logs.Count + " 条记录";
            lblActive.Text = "当前在场: " + DatabaseHelper.GetActiveVehicleCount() + " 辆";
        }

        private static void TranslateColumns(DataGridView dgv)
        {
            var map = new Dictionary<string, string>
            {
                {"PlateNumber", "车牌号"}, {"Direction", "方向"}, {"LogTime", "时间"},
                {"RelatedWeighId", "关联称重ID"}, {"GrossWeight", "毛重(kg)"}, {"TareWeight", "皮重(kg)"},
                {"OperatorName", "操作员"}, {"Remark", "备注"}, {"CreateTime", "创建时间"}
            };
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (map.ContainsKey(col.DataPropertyName))
                    col.HeaderText = map[col.DataPropertyName];
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) { RefreshList(); }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }
    }
}
