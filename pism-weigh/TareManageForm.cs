using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;

namespace pism_weigh
{
    /// <summary>
    /// 车辆皮重管理窗口
    /// </summary>
    public partial class TareManageForm : Form
    {
        private string _plateNumber;
        private TareRecord _latestTare;

        public TareManageForm(string plateNumber)
        {
            _plateNumber = plateNumber;
            InitializeComponent();
        }

        private void TareManageForm_Load(object sender, EventArgs e)
        {
            lblPlate.Text = "车牌: " + _plateNumber;
            RefreshList();
        }

        private void RefreshList()
        {
            var records = DatabaseHelper.GetTareRecords(_plateNumber);
            _latestTare = records.FirstOrDefault();
            lblCurrentTare.Text = _latestTare != null
                ? "当前预设皮重: " + _latestTare.TareWeight.ToString("F0") + " kg (" + _latestTare.CreateTime.ToString("yyyy-MM-dd HH:mm") + ")"
                : "暂无预设皮重";
            dgvRecords.DataSource = null;
            dgvRecords.DataSource = records;
            TranslateColumns(dgvRecords);
        }

        private static void TranslateColumns(DataGridView dgv)
        {
            var map = new System.Collections.Generic.Dictionary<string, string>
            {
                {"PlateNumber", "车牌号"}, {"TareWeight", "皮重(kg)"},
                {"WeighDate", "称重日期"}, {"Source", "来源"}, {"OperatorName", "操作员"},
                {"Remark", "备注"}, {"CreateTime", "记录时间"}
            };
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (map.ContainsKey(col.DataPropertyName))
                    col.HeaderText = map[col.DataPropertyName];
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal w;
            if (!decimal.TryParse(txtTareWeight.Text.Trim(), out w) || w <= 0)
            {
                MessageBox.Show("请输入有效的皮重值 (>0)。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DatabaseHelper.SaveTareRecord(_plateNumber, w, "Manual",
                Environment.UserName, txtRemark.Text.Trim()))
            {
                txtTareWeight.Text = "";
                txtRemark.Text = "";
                RefreshList();
                MessageBox.Show("皮重记录已保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("保存失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRecords.CurrentRow == null) return;
            var r = dgvRecords.CurrentRow.DataBoundItem as TareRecord;
            if (r == null) return;

            if (MessageBox.Show("确定删除此皮重记录？", "确认",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (DatabaseHelper.DeleteTareRecord(r.Id))
                RefreshList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFromHistory_Click(object sender, EventArgs e)
        {
            try
            {
                // 从历史称重记录中获取此车牌的皮重值
                var records = DatabaseHelper.GetWeighRecordsByPlate(_plateNumber);
                var tareValues = records
                    .Where(r => r.TareWeight > 0 && r.Status == WeighStatus.Completed)
                    .OrderByDescending(r => r.CompleteTime)
                    .Take(5)
                    .ToList();

                if (tareValues.Count == 0)
                {
                    MessageBox.Show("没有找到该车牌的历史皮重记录。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var msg = "找到以下历史皮重值:\n";
                foreach (var r in tareValues)
                    msg += "  " + r.TareWeight.ToString("F0") + " kg (" + r.CompleteTime.ToString("yyyy-MM-dd") + ")\n";
                msg += "\n使用最新值 " + tareValues[0].TareWeight.ToString("F0") + " kg ?";

                if (MessageBox.Show(msg, "历史皮重", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseHelper.SaveTareRecord(_plateNumber, tareValues[0].TareWeight, "Weigh",
                        Environment.UserName, "从历史称重记录提取");
                    RefreshList();
                    MessageBox.Show("已从历史记录提取皮重。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("提取失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
