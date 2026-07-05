using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;

namespace pism_weigh
{
    /// <summary>
    /// 称重记录编辑窗体 — 支持修改记录字段并写入审计日志 (ModifyHistoryItem)
    /// </summary>
    public partial class RecordEditForm : Form
    {
        private readonly WeighRecord _original;
        private WeighRecord _working;
        private readonly List<string> _auditLog = new List<string>();

        public WeighRecord EditedRecord { get; private set; }
        public bool IsCancelled { get; private set; }

        public RecordEditForm(WeighRecord record)
        {
            _original = record;
            _working = CloneRecord(record);

            InitializeComponent();
            Services.UIStyler.StyleForm(this, "编辑称重记录", new System.Drawing.Size(620, 500), new System.Drawing.Size(500, 400));
            PopulateFields();
        }

        private WeighRecord CloneRecord(WeighRecord source)
        {
            return new WeighRecord
            {
                Id = source.Id,
                PlateNumber = source.PlateNumber,
                Province = source.Province,
                PlateCode = source.PlateCode,
                GrossWeight = source.GrossWeight,
                TareWeight = source.TareWeight,
                NetWeight = source.NetWeight,
                FirstWeighTime = source.FirstWeighTime,
                SecondWeighTime = source.SecondWeighTime,
                CompleteTime = source.CompleteTime,
                CreateTime = source.CreateTime,
                UpdateTime = source.UpdateTime,
                BusinessType = source.BusinessType,
                Status = source.Status,
                OperatorName = source.OperatorName,
                CargoType = source.CargoType,
                Sender = source.Sender,
                Receiver = source.Receiver,
                DriverName = source.DriverName,
                DriverPhone = source.DriverPhone,
                PrintCount = source.PrintCount,
                Remark = source.Remark
            };
        }

        private void PopulateFields()
        {
            txtRecordId.Text = _working.Id.ToString();
            txtPlate.Text = _working.PlateNumber ?? "";
            txtGrossWeight.Text = _working.GrossWeight.ToString("F0");
            txtTareWeight.Text = _working.TareWeight.ToString("F0");
            txtNetWeight.Text = _working.NetWeight.ToString("F0");
            txtCargoType.Text = _working.CargoType ?? "";
            txtSender.Text = _working.Sender ?? "";
            txtReceiver.Text = _working.Receiver ?? "";
            txtDriverName.Text = _working.DriverName ?? "";
            txtRemark.Text = _working.Remark ?? "";
            txtFirstWeighTime.Text = FormatNullableDateTime(_working.FirstWeighTime);
            txtSecondWeighTime.Text = FormatNullableDateTime(_working.SecondWeighTime);
            lblCreateTime.Text = "创建时间: " + _working.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            lblCompleteTime.Text = "完成时间: " + FormatNullableDateTime(_working.CompleteTime);

            // 先填充下拉列表，再设置选中值（避免 DropDownList 因 items 为空抛异常）
            cboBusinessType.Items.Clear();
            cboBusinessType.Items.Add("PurchaseIn");
            cboBusinessType.Items.Add("SalesOut");
            cboBusinessType.Items.Add("Transfer");
            EnsureComboText(cboBusinessType, _working.BusinessType.ToString());

            cboOperator.Items.Clear();
            try
            {
                var records = DatabaseHelper.GetAllWeighRecords();
                var operators = new System.Collections.Generic.HashSet<string>();
                foreach (var r in records)
                {
                    if (!string.IsNullOrWhiteSpace(r.OperatorName))
                        operators.Add(r.OperatorName);
                }
                foreach (var op in operators) cboOperator.Items.Add(op);
            }
            catch { }
            EnsureComboText(cboOperator, _working.OperatorName ?? "");

            // 如果是已作废记录，禁用编辑但允许恢复
            if (_working.Status == WeighStatus.Cancelled)
            {
                btnCancel.Text = "恢复记录";
                groupBoxMain.Enabled = false;
            }
        }

        private string FormatNullableDateTime(DateTime? dt)
        {
            if (dt == null || dt.Value == DateTime.MinValue) return "";
            return dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 安全设置 ComboBox 文本（处理 DropDownList 模式下值不在列表中的情况）
        /// </summary>
        private void EnsureComboText(ComboBox combo, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (combo.Items.Contains(text))
            {
                combo.Text = text;
            }
            else
            {
                combo.Items.Insert(0, text);
                combo.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            // 收集变更并记录审计
            _working.UpdateTime = DateTime.Now;
            ApplyField("PlateNumber", txtPlate.Text.Trim(), _original.PlateNumber);
            ApplyField("OperatorName", cboOperator.Text.Trim(), _original.OperatorName);
            ApplyField("CargoType", txtCargoType.Text.Trim(), _original.CargoType);
            ApplyField("Sender", txtSender.Text.Trim(), _original.Sender);
            ApplyField("Receiver", txtReceiver.Text.Trim(), _original.Receiver);
            ApplyField("DriverName", txtDriverName.Text.Trim(), _original.DriverName);
            ApplyField("Remark", txtRemark.Text.Trim(), _original.Remark);

            // 重量字段
            decimal gross, tare;
            if (decimal.TryParse(txtGrossWeight.Text.Trim(), out gross))
            {
                if (gross != _original.GrossWeight)
                {
                    LogChange("GrossWeight", _original.GrossWeight.ToString("F0"), gross.ToString("F0"));
                    _working.GrossWeight = gross;
                }
            }
            if (decimal.TryParse(txtTareWeight.Text.Trim(), out tare))
            {
                if (tare != _original.TareWeight)
                {
                    LogChange("TareWeight", _original.TareWeight.ToString("F0"), tare.ToString("F0"));
                    _working.TareWeight = tare;
                }
            }
            _working.NetWeight = _working.GrossWeight - _working.TareWeight;

            // 业务类型
            BusinessType newBiz;
            if (Enum.TryParse(cboBusinessType.Text, out newBiz) && newBiz != _original.BusinessType)
            {
                LogChange("BusinessType", _original.BusinessType.ToString(), newBiz.ToString());
                _working.BusinessType = newBiz;
            }

            // 时间字段
            DateTime newFirst;
            if (TryParseDateTime(txtFirstWeighTime.Text.Trim(), out newFirst) &&
                newFirst != (_original.FirstWeighTime ?? DateTime.MinValue))
            {
                LogChange("FirstWeighTime",
                    (_original.FirstWeighTime.HasValue ? _original.FirstWeighTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""),
                    newFirst.ToString("yyyy-MM-dd HH:mm:ss"));
                _working.FirstWeighTime = newFirst;
            }
            DateTime newSecond;
            if (TryParseDateTime(txtSecondWeighTime.Text.Trim(), out newSecond) &&
                newSecond != (_original.SecondWeighTime ?? DateTime.MinValue))
            {
                LogChange("SecondWeighTime",
                    (_original.SecondWeighTime.HasValue ? _original.SecondWeighTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""),
                    newSecond.ToString("yyyy-MM-dd HH:mm:ss"));
                _working.SecondWeighTime = newSecond;
            }

            if (!DatabaseHelper.SaveWeighRecord(_working))
            {
                MessageBox.Show("保存记录失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 将审计日志追加到 Remark
            if (_auditLog.Count > 0)
            {
                var auditStr = "[EDIT " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + string.Join("; ", _auditLog);
                _working.Remark = (_working.Remark ?? "") + " " + auditStr;
                DatabaseHelper.SaveWeighRecord(_working);
            }

            EditedRecord = _working;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ApplyField(string fieldName, string newValue, string oldValue)
        {
            if (newValue != (oldValue ?? ""))
            {
                LogChange(fieldName, oldValue ?? "", newValue);
            }
        }

        private void LogChange(string fieldName, string oldValue, string newValue)
        {
            _auditLog.Add(string.Format("{0}: '{1}' → '{2}'", fieldName, oldValue, newValue));
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("车牌号不能为空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPlate.Focus();
                return false;
            }

            decimal test;
            if (!decimal.TryParse(txtGrossWeight.Text.Trim(), out test))
            {
                MessageBox.Show("毛重格式不正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGrossWeight.Focus();
                return false;
            }
            if (!decimal.TryParse(txtTareWeight.Text.Trim(), out test))
            {
                MessageBox.Show("皮重格式不正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTareWeight.Focus();
                return false;
            }
            return true;
        }

        private bool TryParseDateTime(string text, out DateTime result)
        {
            return DateTime.TryParseExact(text,
                new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_working.Status == WeighStatus.Cancelled)
            {
                // 恢复已作废记录
                var confirm = MessageBox.Show("确定恢复此记录为已完成状态？", "确认",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                _working.Status = WeighStatus.Completed;
                _working.UpdateTime = DateTime.Now;
                _auditLog.Add(string.Format("Status: 'Cancelled' → 'Completed'"));

                if (!DatabaseHelper.SaveWeighRecord(_working))
                {
                    MessageBox.Show("恢复记录失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("记录已恢复。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // 作废当前记录
                var confirm = MessageBox.Show("确定作废此称重记录？\n作废后可在编辑界面恢复。", "确认",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                _working.Status = WeighStatus.Cancelled;
                _working.UpdateTime = DateTime.Now;
                _auditLog.Add(string.Format("Status: '{0}' → 'Cancelled'", _original.Status.ToString()));

                if (!DatabaseHelper.SaveWeighRecord(_working))
                {
                    MessageBox.Show("作废记录失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("记录已作废。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            IsCancelled = true;
            EditedRecord = _working;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
