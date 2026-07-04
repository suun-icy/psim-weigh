using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;
using pism_weigh.Services;

namespace pism_weigh
{
    public partial class print : Form
    {
        private readonly Action<WeighRecord> _onReprintSuccess;
        private List<WeighRecord> _allRecords = new List<WeighRecord>();

        public print(Action<WeighRecord> onReprintSuccess = null)
        {
            _onReprintSuccess = onReprintSuccess;
            InitializeComponent();
        }

        private void print_Load(object sender, EventArgs e)
        {
            InitializeFilterOptions();
            RefreshRecordList();
        }

        private void InitializeFilterOptions()
        {
            comboBoxBusinessType.Items.Clear();
            comboBoxBusinessType.Items.Add("全部");
            comboBoxBusinessType.Items.AddRange(Enum.GetNames(typeof(BusinessType)));
            comboBoxBusinessType.SelectedIndex = 0;

            comboBoxStatus.Items.Clear();
            comboBoxStatus.Items.Add("全部");
            comboBoxStatus.Items.AddRange(Enum.GetNames(typeof(WeighStatus)));
            comboBoxStatus.SelectedIndex = 0;

            dateTimePickerStart.Value = DateTime.Today.AddDays(-7);
            dateTimePickerEnd.Value = DateTime.Today;
        }

        private void RefreshRecordList()
        {
            _allRecords = DatabaseHelper.GetAllWeighRecords();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<WeighRecord> query = _allRecords;
            DateTime start = dateTimePickerStart.Value.Date;
            DateTime end = dateTimePickerEnd.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(r => r.CreateTime >= start && r.CreateTime <= end);

            string plateKeyword = textBoxPlate.Text.Trim();
            if (!string.IsNullOrWhiteSpace(plateKeyword))
            {
                query = query.Where(r => !string.IsNullOrWhiteSpace(r.PlateNumber)
                    && r.PlateNumber.IndexOf(plateKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (comboBoxBusinessType.SelectedIndex > 0)
            {
                BusinessType businessType;
                if (Enum.TryParse(comboBoxBusinessType.SelectedItem.ToString(), out businessType))
                {
                    query = query.Where(r => r.BusinessType == businessType);
                }
            }

            if (comboBoxStatus.SelectedIndex > 0)
            {
                WeighStatus status;
                if (Enum.TryParse(comboBoxStatus.SelectedItem.ToString(), out status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            var list = query.OrderByDescending(r => r.CreateTime).ToList();
            dataGridViewRecords.DataSource = list;
            labelCount.Text = "记录数：" + list.Count;
        }

        private WeighRecord GetSelectedRecord()
        {
            if (dataGridViewRecords.CurrentRow == null)
            {
                return null;
            }

            return dataGridViewRecords.CurrentRow.DataBoundItem as WeighRecord;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            textBoxPlate.Text = string.Empty;
            comboBoxBusinessType.SelectedIndex = 0;
            comboBoxStatus.SelectedIndex = 0;
            dateTimePickerStart.Value = DateTime.Today.AddDays(-7);
            dateTimePickerEnd.Value = DateTime.Today;
            ApplyFilters();
        }

        private void buttonDetail_Click(object sender, EventArgs e)
        {
            var record = GetSelectedRecord();
            if (record == null)
            {
                MessageBox.Show("请先选择一条记录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("编号：" + record.Id);
            sb.AppendLine("车牌：" + record.PlateNumber);
            sb.AppendLine("业务类型：" + record.BusinessType);
            sb.AppendLine("状态：" + record.Status);
            sb.AppendLine("毛重：" + record.GrossWeight.ToString("F3"));
            sb.AppendLine("皮重：" + record.TareWeight.ToString("F3"));
            sb.AppendLine("净重：" + record.NetWeight.ToString("F3"));
            sb.AppendLine("货物：" + record.CargoType);
            sb.AppendLine("发货单位：" + record.Sender);
            sb.AppendLine("收货单位：" + record.Receiver);
            sb.AppendLine("司机：" + record.DriverName + " " + record.DriverPhone);
            sb.AppendLine("打印次数：" + record.PrintCount);
            sb.AppendLine("创建时间：" + record.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("备注：" + record.Remark);

            MessageBox.Show(sb.ToString(), "记录详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonReprint_Click(object sender, EventArgs e)
        {
            var record = GetSelectedRecord();
            if (record == null)
            {
                MessageBox.Show("请先选择一条记录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var service = new PrintService();
            bool printSuccess = service.Print(record, PrintTemplate.WeighSlip240x93);
            if (!printSuccess)
            {
                MessageBox.Show("重打失败，请检查打印机状态。", "打印失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            record.PrintCount += 1;
            record.UpdateTime = DateTime.Now;
            bool saveSuccess = DatabaseHelper.SaveWeighRecord(record);
            if (!saveSuccess)
            {
                MessageBox.Show("打印成功，但更新打印次数失败。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshRecordList();
                return;
            }

            _onReprintSuccess?.Invoke(record);
            MessageBox.Show("重打成功。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshRecordList();
        }
    }
}
