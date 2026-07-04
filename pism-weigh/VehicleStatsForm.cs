using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;

namespace pism_weigh
{
    /// <summary>
    /// 车辆统计分析窗口
    /// </summary>
    public partial class VehicleStatsForm : Form
    {
        public VehicleStatsForm()
        {
            InitializeComponent();
        }

        private void VehicleStatsForm_Load(object sender, EventArgs e)
        {
            dtpStart.Value = DateTime.Today.AddDays(-30);
            dtpEnd.Value = DateTime.Today;
            RefreshStats();
        }

        private void RefreshStats()
        {
            var stats = DatabaseHelper.GetVehicleStats(dtpStart.Value, dtpEnd.Value);
            dgvStats.DataSource = null;
            dgvStats.DataSource = stats;
            TranslateColumns(dgvStats);

            lblCount.Text = "共 " + stats.Count + " 辆车";
            if (stats.Count > 0)
            {
                lblSummary.Text = string.Format("总称重次数: {0}  |  总净重: {1:F0} kg  |  平均单车净重: {2:F0} kg",
                    stats.Sum(s => s.WeighCount),
                    stats.Sum(s => s.TotalNet),
                    stats.Average(s => s.AvgNet));
            }
        }

        private static void TranslateColumns(DataGridView dgv)
        {
            var map = new Dictionary<string, string>
            {
                {"PlateNumber", "车牌号"}, {"VehicleType", "车辆类型"}, {"OwnerName", "车主"},
                {"WeighCount", "称重次数"}, {"TotalGross", "累计毛重(kg)"}, {"TotalTare", "累计皮重(kg)"},
                {"TotalNet", "累计净重(kg)"}, {"AvgNet", "平均净重(kg)"}, {"MaxNet", "最大净重(kg)"},
                {"FirstWeigh", "首次称重"}, {"LastWeigh", "最近称重"}, {"TotalPrints", "打印次数"}
            };
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (map.ContainsKey(col.DataPropertyName))
                    col.HeaderText = map[col.DataPropertyName];
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) { RefreshStats(); }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvStats.Rows.Count == 0) return;
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = string.Format("车辆统计_{0:yyyyMMdd}.csv", DateTime.Now),
                Title = "导出车辆统计"
            };
            if (saveDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                using (var writer = new System.IO.StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                {
                    writer.Write('\uFEFF');
                    writer.WriteLine("车牌号,车辆类型,车主,称重次数,累计毛重(kg),累计皮重(kg),累计净重(kg),平均净重(kg),最大净重(kg),首次称重,最近称重,打印次数");
                    foreach (DataGridViewRow row in dgvStats.Rows)
                    {
                        var cells = new string[dgvStats.Columns.Count];
                        for (int i = 0; i < cells.Length; i++)
                            cells[i] = "\"" + (row.Cells[i].Value?.ToString() ?? "") + "\"";
                        writer.WriteLine(string.Join(",", cells));
                    }
                }
                MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }
    }
}
