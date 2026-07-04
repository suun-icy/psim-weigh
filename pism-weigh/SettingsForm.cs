using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Models;

namespace pism_weigh
{
    public partial class SettingsForm : Form
    {
        private AppConfig _config;
        private List<string> _cargoItems;
        private List<string> _receiverItems;
        private List<string> _operatorItems;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _config = AppConfig.Load();

            // 加载系统配置
            txtServerUrl.Text = _config.ServerUrl ?? "";
            txtPrinter.Text = _config.PrinterName ?? "";
            chkAutoConnect.Checked = _config.AutoConnect;

            // 加载基础数据
            LoadBasicData();

            // 填充打印机列表
            try { cboPrinter.Items.AddRange(Services.PrintService.GetAvailablePrinters()); }
            catch { }
        }

        private void LoadBasicData()
        {
            try
            {
                var records = DatabaseHelper.GetAllWeighRecords();
                _cargoItems = records.Where(r => !string.IsNullOrWhiteSpace(r.CargoType))
                    .Select(r => r.CargoType.Trim()).Distinct().OrderBy(x => x).ToList();
                _receiverItems = records.Where(r => !string.IsNullOrWhiteSpace(r.Receiver))
                    .Select(r => r.Receiver.Trim()).Distinct().OrderBy(x => x).ToList();
                _operatorItems = records.Where(r => !string.IsNullOrWhiteSpace(r.OperatorName))
                    .Select(r => r.OperatorName.Trim()).Distinct().OrderBy(x => x).ToList();

                RefreshLists();
            }
            catch { }
        }

        private void RefreshLists()
        {
            lstCargo.Items.Clear();
            lstCargo.Items.AddRange(_cargoItems.ToArray());

            lstReceiver.Items.Clear();
            lstReceiver.Items.AddRange(_receiverItems.ToArray());

            lstOperator.Items.Clear();
            lstOperator.Items.AddRange(_operatorItems.ToArray());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _config.ServerUrl = txtServerUrl.Text.Trim();
            _config.PrinterName = txtPrinter.Text.Trim();
            _config.AutoConnect = chkAutoConnect.Checked;
            _config.Save();
            MessageBox.Show("配置已保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ===== 运输内容管理 =====
        private void btnAddCargo_Click(object sender, EventArgs e)
        {
            var text = txtCargo.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!_cargoItems.Contains(text))
            {
                _cargoItems.Add(text);
                _cargoItems.Sort();
                RefreshLists();
            }
            txtCargo.Text = "";
        }

        private void btnDelCargo_Click(object sender, EventArgs e)
        {
            if (lstCargo.SelectedItem != null)
            {
                _cargoItems.Remove(lstCargo.SelectedItem.ToString());
                RefreshLists();
            }
        }

        private void lstCargo_DoubleClick(object sender, EventArgs e)
        {
            if (lstCargo.SelectedItem != null)
                txtCargo.Text = lstCargo.SelectedItem.ToString();
        }

        // ===== 收货单位管理 =====
        private void btnAddReceiver_Click(object sender, EventArgs e)
        {
            var text = txtReceiver.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!_receiverItems.Contains(text))
            {
                _receiverItems.Add(text);
                _receiverItems.Sort();
                RefreshLists();
            }
            txtReceiver.Text = "";
        }

        private void btnDelReceiver_Click(object sender, EventArgs e)
        {
            if (lstReceiver.SelectedItem != null)
            {
                _receiverItems.Remove(lstReceiver.SelectedItem.ToString());
                RefreshLists();
            }
        }

        private void lstReceiver_DoubleClick(object sender, EventArgs e)
        {
            if (lstReceiver.SelectedItem != null)
                txtReceiver.Text = lstReceiver.SelectedItem.ToString();
        }

        // ===== 司磅员管理 =====
        private void btnAddOperator_Click(object sender, EventArgs e)
        {
            var text = txtOperator.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!_operatorItems.Contains(text))
            {
                _operatorItems.Add(text);
                _operatorItems.Sort();
                RefreshLists();
            }
            txtOperator.Text = "";
        }

        private void btnDelOperator_Click(object sender, EventArgs e)
        {
            if (lstOperator.SelectedItem != null)
            {
                _operatorItems.Remove(lstOperator.SelectedItem.ToString());
                RefreshLists();
            }
        }

        private void lstOperator_DoubleClick(object sender, EventArgs e)
        {
            if (lstOperator.SelectedItem != null)
                txtOperator.Text = lstOperator.SelectedItem.ToString();
        }

        private void cboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPrinter.SelectedItem != null)
                txtPrinter.Text = cboPrinter.SelectedItem.ToString();
        }
    }
}
