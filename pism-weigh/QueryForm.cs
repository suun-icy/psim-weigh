using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;
using pism_weigh.Services;

namespace pism_weigh
{
    public partial class QueryForm : Form
    {
        private List<WeighRecord> _records;

        public QueryForm()
        {
            InitializeComponent();
            dgvRecords.AutoGenerateColumns = true;
            dgvRecords.DataBindingComplete += DgvRecords_DataBindingComplete;
            dgvRecords.CellFormatting += DgvRecords_CellFormatting;
        }

        private void DgvRecords_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            var colName = dgvRecords.Columns[e.ColumnIndex].DataPropertyName;

            if (colName == "BusinessType")
            {
                switch ((Models.BusinessType)(int)e.Value)
                {
                    case Models.BusinessType.PurchaseIn: e.Value = "采购入库"; break;
                    case Models.BusinessType.SalesOut: e.Value = "销售出库"; break;
                    case Models.BusinessType.Transfer: e.Value = "内部调拨"; break;
                    default: e.Value = "其他"; break;
                }
                e.FormattingApplied = true;
            }
            else if (colName == "Status")
            {
                switch ((Models.WeighStatus)(int)e.Value)
                {
                    case Models.WeighStatus.FirstWeigh: e.Value = "首次称重"; break;
                    case Models.WeighStatus.SecondWeigh: e.Value = "二次称重"; break;
                    case Models.WeighStatus.Completed: e.Value = "已完成"; break;
                    case Models.WeighStatus.Cancelled: e.Value = "已取消"; break;
                }
                e.FormattingApplied = true;
            }
        }

        private void DgvRecords_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var headers = new Dictionary<string, string>
            {
                { "Id", "编号" },
                { "PlateNumber", "车牌" },
                { "Province", "省份" },
                { "PlateCode", "车牌号" },
                { "GrossWeight", "毛重(kg)" },
                { "TareWeight", "皮重(kg)" },
                { "NetWeight", "净重(kg)" },
                { "CargoType", "运输内容" },
                { "Sender", "发货单位" },
                { "Receiver", "收货单位" },
                { "DriverName", "司机" },
                { "DriverPhone", "电话" },
                { "BusinessType", "业务类型" },
                { "Status", "状态" },
                { "FirstWeighTime", "首次称重" },
                { "SecondWeighTime", "二次称重" },
                { "CompleteTime", "完成时间" },
                { "OperatorName", "司磅员" },
                { "PrintCount", "打印次数" },
                { "Remark", "备注" },
                { "CreateTime", "创建时间" }
            };

            foreach (DataGridViewColumn col in dgvRecords.Columns)
            {
                if (headers.TryGetValue(col.DataPropertyName, out string header))
                {
                    col.HeaderText = header;
                }
            }
        }

        private void QueryForm_Load(object sender, EventArgs e)
        {
            dtpStart.Value = DateTime.Today.AddDays(-30);
            dtpEnd.Value = DateTime.Today;
            btnSearch.PerformClick();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WeighStatus? status = null;
                if (cboStatus.SelectedIndex > 0)
                {
                    status = (WeighStatus)(cboStatus.SelectedIndex - 1);
                }

                _records = DatabaseHelper.SearchRecords(
                    plateNumber: txtPlate.Text.Trim(),
                    driverName: txtDriver.Text.Trim(),
                    cargoType: txtCargo.Text.Trim(),
                    sender: txtSender.Text.Trim(),
                    receiver: txtReceiver.Text.Trim(),
                    startDate: dtpStart.Value.Date,
                    endDate: dtpEnd.Value.Date,
                    status: status);

                dgvRecords.DataSource = _records;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败：" + ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtPlate.Text = "";
            txtDriver.Text = "";
            txtCargo.Text = "";
            txtSender.Text = "";
            txtReceiver.Text = "";
            cboStatus.SelectedIndex = 0;
            dtpStart.Value = DateTime.Today.AddDays(-30);
            dtpEnd.Value = DateTime.Today;
        }

        private void UpdateSummary()
        {
            if (_records == null || _records.Count == 0)
            {
                lblSummary.Text = "无匹配记录";
                return;
            }

            int count = _records.Count;
            decimal totalGross = _records.Sum(r => r.GrossWeight);
            decimal totalTare = _records.Sum(r => r.TareWeight);
            decimal totalNet = _records.Sum(r => r.NetWeight);
            decimal avgNet = totalNet / count;
            int totalPrints = _records.Sum(r => r.PrintCount);

            lblSummary.Text = string.Format(
                "共 {0} 条记录  |  总毛重: {1:F0} kg  |  总皮重: {2:F0} kg  |  总净重: {3:F0} kg  |  平均净重: {4:F0} kg  |  打印次数: {5}",
                count, totalGross, totalTare, totalNet, avgNet, totalPrints);
        }

        private void dgvRecords_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var record = _records[e.RowIndex];
            OpenEditForm(record);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var record = GetSelectedRecord();
            if (record == null) return;
            OpenEditForm(record);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_records == null || _records.Count == 0)
            {
                MessageBox.Show("没有可导出的数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*",
                DefaultExt = "csv",
                FileName = string.Format("称重记录_{0:yyyyMMddHHmmss}.csv", DateTime.Now),
                Title = "导出称重记录"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var writer = new System.IO.StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                {
                    // 写入 BOM（确保 Excel 正确识别 UTF-8 中文）
                    writer.Write('\uFEFF');
                    // 写入表头
                    writer.WriteLine(string.Join(",",
                        "\"编号\"", "\"车牌\"", "\"业务类型\"", "\"状态\"",
                        "\"毛重(kg)\"", "\"皮重(kg)\"", "\"净重(kg)\"",
                        "\"运输内容\"", "\"发货单位\"", "\"收货单位\"",
                        "\"司机\"", "\"司磅员\"", "\"首次称重时间\"",
                        "\"二次称重时间\"", "\"完成时间\"", "\"打印次数\"", "\"备注\""));

                    foreach (var r in _records)
                    {
                        writer.WriteLine(string.Join(",",
                            CsvCell(r.Id.ToString()),
                            CsvCell(r.PlateNumber ?? ""),
                            CsvCell(GetBizText(r.BusinessType)),
                            CsvCell(GetStatusText(r.Status)),
                            CsvCell(r.GrossWeight.ToString("F0")),
                            CsvCell(r.TareWeight.ToString("F0")),
                            CsvCell(r.NetWeight.ToString("F0")),
                            CsvCell(r.CargoType ?? ""),
                            CsvCell(r.Sender ?? ""),
                            CsvCell(r.Receiver ?? ""),
                            CsvCell(r.DriverName ?? ""),
                            CsvCell(r.OperatorName ?? ""),
                            CsvCell(FormatTimeCsv(r.FirstWeighTime)),
                            CsvCell(FormatTimeCsv(r.SecondWeighTime)),
                            CsvCell(FormatTimeCsv(r.CompleteTime)),
                            CsvCell(r.PrintCount.ToString()),
                            CsvCell(r.Remark ?? "")
                        ));
                    }
                }

                MessageBox.Show("导出成功！\n文件: " + saveDialog.FileName, "成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败: " + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string CsvCell(string text)
        {
            return "\"" + (text ?? "").Replace("\"", "\"\"") + "\"";
        }

        private static string FormatTimeCsv(DateTime? dt)
        {
            if (dt == null || dt.Value == DateTime.MinValue) return "";
            return dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static string GetBizText(BusinessType t)
        {
            switch (t)
            {
                case BusinessType.PurchaseIn: return "采购入库";
                case BusinessType.SalesOut: return "销售出库";
                case BusinessType.Transfer: return "内部调拨";
                default: return "其他";
            }
        }

        private static string GetStatusText(WeighStatus s)
        {
            switch (s)
            {
                case WeighStatus.FirstWeigh: return "首次称重";
                case WeighStatus.Completed: return "已完成";
                case WeighStatus.Cancelled: return "已作废";
                default: return s.ToString();
            }
        }

        private void OpenEditForm(WeighRecord record)
        {
            var editForm = new RecordEditForm(record);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                btnSearch_Click(this, EventArgs.Empty); // 刷新列表
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            var record = GetSelectedRecord();
            if (record == null) return;
            ShowDetail(record);
        }

        private void ShowDetail(WeighRecord record)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("═══════ 称重记录详情 ═══════");
            sb.AppendLine();
            sb.AppendLine("编号：" + record.Id);
            sb.AppendLine("车牌：" + record.PlateNumber);
            sb.AppendLine("业务类型：" + record.BusinessType);
            sb.AppendLine("状态：" + record.Status);
            sb.AppendLine("────────────────────────");
            sb.AppendLine("毛重：" + record.GrossWeight.ToString("F0") + " kg");
            sb.AppendLine("皮重：" + record.TareWeight.ToString("F0") + " kg");
            sb.AppendLine("净重：" + record.NetWeight.ToString("F0") + " kg");
            sb.AppendLine("────────────────────────");
            sb.AppendLine("货物类型：" + (string.IsNullOrEmpty(record.CargoType) ? "-" : record.CargoType));
            sb.AppendLine("发货单位：" + (string.IsNullOrEmpty(record.Sender) ? "-" : record.Sender));
            sb.AppendLine("收货单位：" + (string.IsNullOrEmpty(record.Receiver) ? "-" : record.Receiver));
            sb.AppendLine("司机：" + (string.IsNullOrEmpty(record.DriverName) ? "-" : record.DriverName));
            sb.AppendLine("────────────────────────");
            sb.AppendLine("首次称重：" + (record.FirstWeighTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-"));
            sb.AppendLine("二次称重：" + (record.SecondWeighTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-"));
            sb.AppendLine("完成时间：" + record.CompleteTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("操作员：" + (string.IsNullOrEmpty(record.OperatorName) ? "-" : record.OperatorName));
            sb.AppendLine("打印次数：" + record.PrintCount);
            sb.AppendLine("────────────────────────");
            sb.AppendLine("创建时间：" + record.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("备注：" + (string.IsNullOrEmpty(record.Remark) ? "-" : record.Remark));

            MessageBox.Show(sb.ToString(), "记录详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            var record = GetSelectedRecord();
            if (record == null) return;

            var confirm = MessageBox.Show("确定重新打印该记录？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var service = new PrintService();
            service.PrintPreviewWithPrinterDialog(record);
        }

        private WeighRecord GetSelectedRecord()
        {
            if (dgvRecords.CurrentRow == null)
            {
                MessageBox.Show("请先选择一条记录。");
                return null;
            }
            return dgvRecords.CurrentRow.DataBoundItem as WeighRecord;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
