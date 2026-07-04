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
        private List<string> _senderItems;
        private List<string> _receiverItems;
        private List<string> _driverItems;
        private List<string> _operatorItems;

        // 程序化创建的控件
        private GroupBox groupBox4;
        private TextBox txtSender;
        private ListBox lstSender;
        private Button btnAddSender, btnDelSender;
        private GroupBox groupBox5;
        private TextBox txtDriver;
        private ListBox lstDriver;
        private Button btnAddDriver, btnDelDriver;

        public SettingsForm()
        {
            InitializeComponent();
            InitExtraDataGroups();
        }

        private void InitExtraDataGroups()
        {
            // 扩展 tabBasic 以容纳 5 个组，改为 3 列布局
            tabControl.Size = new Size(580, 460);
            this.ClientSize = new Size(604, 550);
            btnSave.Location = new Point(380, 480);
            btnCancel.Location = new Point(480, 480);

            // Column layout: x = 10, x = 200, x = 390; width = 180 each
            // Row 1 (y=10): 运输内容 | 收货单位 | 司磅员
            groupBox1.Location = new Point(10, 10);
            groupBox1.Size = new Size(180, 210);
            groupBox2.Location = new Point(200, 10);
            groupBox2.Size = new Size(180, 210);
            groupBox3.Location = new Point(390, 10);
            groupBox3.Size = new Size(180, 210);

            AdjustGroupControls(groupBox1, txtCargo, lstCargo, btnAddCargo, btnDelCargo);
            AdjustGroupControls(groupBox2, txtReceiver, lstReceiver, btnAddReceiver, btnDelReceiver);
            AdjustGroupControls(groupBox3, txtOperator, lstOperator, btnAddOperator, btnDelOperator);

            // Row 2 (y=230): 发货单位 | 司机
            CreateGroup(out groupBox4, out txtSender, out lstSender, out btnAddSender, out btnDelSender,
                "发货单位", 10, 230);
            CreateGroup(out groupBox5, out txtDriver, out lstDriver, out btnAddDriver, out btnDelDriver,
                "司机", 200, 230);

            btnAddSender.Click += btnAddSender_Click;
            btnDelSender.Click += btnDelSender_Click;
            lstSender.DoubleClick += lstSender_DoubleClick;
            btnAddDriver.Click += btnAddDriver_Click;
            btnDelDriver.Click += btnDelDriver_Click;
            lstDriver.DoubleClick += lstDriver_DoubleClick;

            tabBasic.Controls.Add(groupBox4);
            tabBasic.Controls.Add(groupBox5);
        }

        private void CreateGroup(out GroupBox gb, out TextBox txt, out ListBox lst,
            out Button btnAdd, out Button btnDel, string title, int x, int y)
        {
            gb = new GroupBox
            {
                Text = title,
                Location = new Point(x, y),
                Size = new Size(180, 210),
                Font = new Font("Microsoft YaHei UI", 9F)
            };
            txt = new TextBox
            {
                Location = new Point(8, 24),
                Size = new Size(124, 24),
                Font = new Font("Microsoft YaHei UI", 9F)
            };
            btnAdd = new Button
            {
                Text = "+",
                Location = new Point(136, 24),
                Size = new Size(36, 24),
                UseVisualStyleBackColor = true
            };
            lst = new ListBox
            {
                Location = new Point(8, 56),
                Size = new Size(164, 134),
                IntegralHeight = false
            };
            btnDel = new Button
            {
                Text = "删除选中",
                Location = new Point(50, 194),
                Size = new Size(80, 28),
                UseVisualStyleBackColor = true
            };
            gb.Controls.Add(txt);
            gb.Controls.Add(btnAdd);
            gb.Controls.Add(lst);
            gb.Controls.Add(btnDel);
        }

        private void AdjustGroupControls(GroupBox gb, TextBox txt, ListBox lst, Button btnAdd, Button btnDel)
        {
            txt.Location = new Point(8, 24);
            txt.Size = new Size(124, 24);
            btnAdd.Location = new Point(136, 24);
            btnAdd.Size = new Size(36, 24);
            lst.Location = new Point(8, 56);
            lst.Size = new Size(164, 134);
            btnDel.Location = new Point(50, 194);
            btnDel.Size = new Size(80, 28);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _config = AppConfig.Load();

            // 加载系统配置
            txtServerUrl.Text = _config.ServerUrl ?? "";
            txtPrinter.Text = _config.PrinterName ?? "";
            chkAutoConnect.Checked = _config.AutoConnect;

            // 从数据库加载基础数据，同时从历史记录补充
            _cargoItems = MergeData(DatabaseHelper.GetBasicData("CargoType"), "CargoType");
            _senderItems = MergeData(DatabaseHelper.GetBasicData("Sender"), "Sender");
            _receiverItems = MergeData(DatabaseHelper.GetBasicData("Receiver"), "Receiver");
            _driverItems = MergeData(DatabaseHelper.GetBasicData("Driver"), "Driver");
            _operatorItems = MergeData(DatabaseHelper.GetBasicData("Operator"), "Operator");

            _cargoItems.Sort();
            _senderItems.Sort();
            _receiverItems.Sort();
            _driverItems.Sort();
            _operatorItems.Sort();

            RefreshLists();

            // 填充打印机列表
            try { cboPrinter.Items.AddRange(Services.PrintService.GetAvailablePrinters()); }
            catch { }
        }

        /// <summary>
        /// 从数据库读取基础数据，并补充历史记录中的项
        /// </summary>
        private List<string> MergeData(List<string> dbItems, string fieldName)
        {
            if (dbItems == null) dbItems = new List<string>();
            try
            {
                var records = DatabaseHelper.GetAllWeighRecords();
                foreach (var r in records)
                {
                    string val = null;
                    switch (fieldName)
                    {
                        case "CargoType": val = r.CargoType; break;
                        case "Sender": val = r.Sender; break;
                        case "Receiver": val = r.Receiver; break;
                        case "Driver": val = r.DriverName; break;
                        case "Operator": val = r.OperatorName; break;
                    }
                    if (!string.IsNullOrWhiteSpace(val) && !dbItems.Contains(val.Trim()))
                        dbItems.Add(val.Trim());
                }
            }
            catch { }
            return dbItems;
        }

        private void RefreshLists()
        {
            lstCargo.Items.Clear();
            lstCargo.Items.AddRange(_cargoItems.ToArray());
            lstSender.Items.Clear();
            lstSender.Items.AddRange(_senderItems.ToArray());
            lstReceiver.Items.Clear();
            lstReceiver.Items.AddRange(_receiverItems.ToArray());
            lstDriver.Items.Clear();
            lstDriver.Items.AddRange(_driverItems.ToArray());
            lstOperator.Items.Clear();
            lstOperator.Items.AddRange(_operatorItems.ToArray());
        }

        /// <summary>
        /// 保存: 系统配置→AppConfig JSON, 基础数据→SQLite
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 保存系统配置到 JSON
            _config.ServerUrl = txtServerUrl.Text.Trim();
            _config.PrinterName = txtPrinter.Text.Trim();
            _config.AutoConnect = chkAutoConnect.Checked;
            _config.Save();

            // 保存基础数据到数据库
            DatabaseHelper.SaveBasicDataBatch("CargoType", _cargoItems);
            DatabaseHelper.SaveBasicDataBatch("Sender", _senderItems);
            DatabaseHelper.SaveBasicDataBatch("Receiver", _receiverItems);
            DatabaseHelper.SaveBasicDataBatch("Driver", _driverItems);
            DatabaseHelper.SaveBasicDataBatch("Operator", _operatorItems);

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
            AddItem(txtCargo, _cargoItems);
            txtCargo.Text = "";
        }
        private void btnDelCargo_Click(object sender, EventArgs e) => DeleteItem(lstCargo, _cargoItems);
        private void lstCargo_DoubleClick(object sender, EventArgs e) => SelectItem(lstCargo, txtCargo);

        // ===== 收货单位管理 =====
        private void btnAddReceiver_Click(object sender, EventArgs e)
        {
            AddItem(txtReceiver, _receiverItems);
            txtReceiver.Text = "";
        }
        private void btnDelReceiver_Click(object sender, EventArgs e) => DeleteItem(lstReceiver, _receiverItems);
        private void lstReceiver_DoubleClick(object sender, EventArgs e) => SelectItem(lstReceiver, txtReceiver);

        // ===== 司磅员管理 =====
        private void btnAddOperator_Click(object sender, EventArgs e)
        {
            AddItem(txtOperator, _operatorItems);
            txtOperator.Text = "";
        }
        private void btnDelOperator_Click(object sender, EventArgs e) => DeleteItem(lstOperator, _operatorItems);
        private void lstOperator_DoubleClick(object sender, EventArgs e) => SelectItem(lstOperator, txtOperator);

        // ===== 发货单位管理 =====
        private void btnAddSender_Click(object sender, EventArgs e)
        {
            AddItem(txtSender, _senderItems);
            txtSender.Text = "";
        }
        private void btnDelSender_Click(object sender, EventArgs e) => DeleteItem(lstSender, _senderItems);
        private void lstSender_DoubleClick(object sender, EventArgs e) => SelectItem(lstSender, txtSender);

        // ===== 司机管理 =====
        private void btnAddDriver_Click(object sender, EventArgs e)
        {
            AddItem(txtDriver, _driverItems);
            txtDriver.Text = "";
        }
        private void btnDelDriver_Click(object sender, EventArgs e) => DeleteItem(lstDriver, _driverItems);
        private void lstDriver_DoubleClick(object sender, EventArgs e) => SelectItem(lstDriver, txtDriver);

        // ===== 公共辅助方法 =====
        private void AddItem(TextBox txt, List<string> items)
        {
            var text = txt.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!items.Contains(text))
            {
                items.Add(text);
                items.Sort();
                RefreshLists();
            }
        }
        private void DeleteItem(ListBox lst, List<string> items)
        {
            if (lst.SelectedItem != null)
            {
                items.Remove(lst.SelectedItem.ToString());
                RefreshLists();
            }
        }
        private void SelectItem(ListBox lst, TextBox txt)
        {
            if (lst.SelectedItem != null)
                txt.Text = lst.SelectedItem.ToString();
        }

        private void cboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPrinter.SelectedItem != null)
                txtPrinter.Text = cboPrinter.SelectedItem.ToString();
        }
    }
}
