using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pism_weigh.Database;
using pism_weigh.Interfaces;
using pism_weigh.Models;
using pism_weigh.Services;

namespace pism_weigh
{
    /// <summary>
    /// 摄像头管理与预览窗口
    /// </summary>
    public partial class CameraForm : Form
    {
        private ICameraService _cameraService;
        private ILPRService _lprService;
        private CameraConfig _editing;
        private PictureBox _picPreview;
        private Label _lblStatus, _lblPlateResult;
        private Button _btnConnect, _btnDisconnect, _btnSnapshot, _btnRecognize;

        private TextBox _txtName, _txtIP, _txtPort, _txtUser, _txtPwd, _txtRTSP, _txtChannel;
        private ComboBox _cboType, _cboResolution;
        private DataGridView _dgvCameras;
        private Button _btnSave, _btnDelete, _btnSetDefault, _btnClose;
        private CheckBox _chkEnabled;

        public CameraForm()
        {
            InitializeControls();
            Services.UIStyler.StyleForm(this, "摄像头管理");
        }

        private void InitializeControls()
        {
            this.ClientSize = new Size(960, 620);
            this.Font = new Font("Microsoft YaHei UI", 9F);

            // ===== 左侧：摄像头列表 =====
            var pnlList = new Panel { Location = new Point(12, 12), Size = new Size(420, 596), BorderStyle = BorderStyle.FixedSingle };
            _dgvCameras = new DataGridView
            {
                Location = new Point(8, 8), Size = new Size(404, 520),
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                RowHeadersVisible = false
            };
            _dgvCameras.SelectionChanged += (s, e) => SelectCamera();
            pnlList.Controls.Add(_dgvCameras);

            // 列表按钮
            _btnSave = new Button { Text = "保存配置", Location = new Point(8, 536), Size = new Size(90, 30), BackColor = Color.FromArgb(24, 144, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _btnDelete = new Button { Text = "删除", Location = new Point(106, 536), Size = new Size(60, 30) };
            _btnSetDefault = new Button { Text = "设为默认", Location = new Point(174, 536), Size = new Size(80, 30) };
            _btnClose = new Button { Text = "关闭", Location = new Point(340, 536), Size = new Size(70, 30) };
            _btnSave.Click += BtnSave_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnSetDefault.Click += BtnSetDefault_Click;
            _btnClose.Click += (s, e) => { _cameraService?.Disconnect(); Close(); };
            pnlList.Controls.AddRange(new Control[] { _btnSave, _btnDelete, _btnSetDefault, _btnClose });
            this.Controls.Add(pnlList);

            // ===== 右侧：配置 + 预览 =====
            var pnlRight = new Panel { Location = new Point(440, 12), Size = new Size(508, 596), BorderStyle = BorderStyle.FixedSingle };

            // 配置表单
            var grpConfig = new GroupBox { Text = "摄像头参数", Location = new Point(8, 8), Size = new Size(492, 280), Font = new Font("Microsoft YaHei UI", 9F) };
            int y = 24;
            _txtName = AddField(grpConfig, "名称", ref y, 80, 150);
            _cboType = AddCombo(grpConfig, "类型", ref y, 80, 150, "Generic", "ONVIF", "Hikvision", "USB");
            _txtIP = AddField(grpConfig, "IP地址", ref y, 80, 150);
            _txtPort = AddField(grpConfig, "端口", ref y, 80, 80);
            _txtUser = AddField(grpConfig, "用户名", ref y, 80, 120);
            _txtPwd = AddField(grpConfig, "密码", ref y, 80, 120);
            _txtPwd.PasswordChar = '*';
            _txtChannel = AddField(grpConfig, "通道号", ref y, 80, 60);
            _txtRTSP = AddField(grpConfig, "RTSP地址", ref y, 80, 300);
            _cboResolution = AddCombo(grpConfig, "分辨率", ref y, 80, 120, "1920x1080", "1280x720", "640x480");
            _chkEnabled = new CheckBox { Text = "启用", Location = new Point(80, y + 4), Size = new Size(60, 20), Checked = true };
            grpConfig.Controls.Add(_chkEnabled);
            pnlRight.Controls.Add(grpConfig);

            // 预览区
            var grpPreview = new GroupBox { Text = "实时预览", Location = new Point(8, 296), Size = new Size(492, 230), Font = new Font("Microsoft YaHei UI", 9F) };
            _picPreview = new PictureBox { Location = new Point(6, 16), Size = new Size(478, 152), BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
            grpPreview.Controls.Add(_picPreview);

            _btnConnect = new Button { Text = "连接", Location = new Point(8, 176), Size = new Size(70, 28), BackColor = Color.FromArgb(82, 196, 26), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _btnDisconnect = new Button { Text = "断开", Location = new Point(84, 176), Size = new Size(70, 28), Enabled = false };
            _btnSnapshot = new Button { Text = "抓拍", Location = new Point(160, 176), Size = new Size(70, 28) };
            _btnRecognize = new Button { Text = "识别车牌", Location = new Point(236, 176), Size = new Size(80, 28), BackColor = Color.FromArgb(250, 173, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _lblStatus = new Label { Text = "状态: 未连接", Location = new Point(322, 180), Size = new Size(160, 20), ForeColor = Color.Gray };
            _lblPlateResult = new Label { Text = "", Location = new Point(8, 208), Size = new Size(476, 20), Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold), ForeColor = Color.DarkGreen };

            _btnConnect.Click += BtnConnect_Click;
            _btnDisconnect.Click += (s, e) => { _cameraService?.Disconnect(); _btnConnect.Enabled = true; _btnDisconnect.Enabled = false; _lblStatus.Text = "状态: 已断开"; };
            _btnSnapshot.Click += BtnSnapshot_Click;
            _btnRecognize.Click += BtnRecognize_Click;

            grpPreview.Controls.AddRange(new Control[] { _btnConnect, _btnDisconnect, _btnSnapshot, _btnRecognize, _lblStatus, _lblPlateResult });
            pnlRight.Controls.Add(grpPreview);
            this.Controls.Add(pnlRight);

            this.Load += (s, e) => RefreshList();
            this.FormClosing += (s, e) => _cameraService?.Disconnect();
        }

        private TextBox AddField(Control parent, string label, ref int y, int lx, int tw)
        {
            parent.Controls.Add(new Label { Text = label, Location = new Point(lx - 70, y + 3), Size = new Size(65, 24) });
            var txt = new TextBox { Location = new Point(lx, y), Size = new Size(tw, 24), BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(txt);
            y += 30;
            return txt;
        }

        private ComboBox AddCombo(Control parent, string label, ref int y, int lx, int tw, params string[] items)
        {
            parent.Controls.Add(new Label { Text = label, Location = new Point(lx - 70, y + 3), Size = new Size(65, 24) });
            var cbo = new ComboBox { Location = new Point(lx, y), Size = new Size(tw, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            parent.Controls.Add(cbo);
            y += 30;
            return cbo;
        }

        private void RefreshList()
        {
            var cameras = DatabaseHelper.GetAllCameras();
            _dgvCameras.DataSource = null;
            _dgvCameras.DataSource = cameras;
            TranslateColumns();
        }

        private void TranslateColumns()
        {
            var map = new System.Collections.Generic.Dictionary<string, string>
            {
                {"Name", "名称"}, {"CameraType", "类型"}, {"IPAddress", "IP"}, {"Port", "端口"},
                {"IsEnabled", "启用"}, {"IsDefault", "默认"}, {"Resolution", "分辨率"}
            };
            foreach (DataGridViewColumn col in _dgvCameras.Columns)
                if (map.ContainsKey(col.DataPropertyName)) col.HeaderText = map[col.DataPropertyName];
        }

        private void SelectCamera()
        {
            if (_dgvCameras.CurrentRow == null) { _editing = null; return; }
            _editing = _dgvCameras.CurrentRow.DataBoundItem as CameraConfig;
            if (_editing == null) return;

            _txtName.Text = _editing.Name ?? "";
            _cboType.Text = _editing.CameraType ?? "Generic";
            _txtIP.Text = _editing.IPAddress ?? "";
            _txtPort.Text = _editing.Port.ToString();
            _txtUser.Text = _editing.Username ?? "";
            _txtPwd.Text = _editing.Password ?? "";
            _txtChannel.Text = _editing.ChannelNo.ToString();
            _txtRTSP.Text = _editing.RTSPUrl ?? "";
            _cboResolution.Text = _editing.Resolution ?? "1920x1080";
            _chkEnabled.Checked = _editing.IsEnabled;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text)) { MessageBox.Show("名称不能为空。"); return; }
            var cam = _editing ?? new CameraConfig();
            cam.Name = _txtName.Text.Trim();
            cam.CameraType = _cboType.Text;
            cam.IPAddress = _txtIP.Text.Trim();
            int.TryParse(_txtPort.Text, out int port); cam.Port = port > 0 ? port : 8000;
            cam.Username = _txtUser.Text.Trim();
            cam.Password = _txtPwd.Text.Trim();
            int.TryParse(_txtChannel.Text, out int ch); cam.ChannelNo = ch > 0 ? ch : 1;
            cam.RTSPUrl = _txtRTSP.Text.Trim();
            cam.Resolution = _cboResolution.Text;
            cam.IsEnabled = _chkEnabled.Checked;
            cam.UpdateTime = DateTime.Now;

            if (cam.IsDefault)
                DatabaseHelper.ExecuteNonQuery("UPDATE Cameras SET IsDefault=0");

            if (DatabaseHelper.SaveCamera(cam))
            {
                _editing = null;
                RefreshList();
                MessageBox.Show("保存成功。");
            }
            else MessageBox.Show("保存失败。");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_editing == null) return;
            if (MessageBox.Show("删除摄像头 '" + _editing.Name + "' ?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            if (DatabaseHelper.DeleteCamera(_editing.Id)) { _editing = null; RefreshList(); }
        }

        private void BtnSetDefault_Click(object sender, EventArgs e)
        {
            if (_editing == null) return;
            DatabaseHelper.ExecuteNonQuery("UPDATE Cameras SET IsDefault=0");
            _editing.IsDefault = true;
            DatabaseHelper.SaveCamera(_editing);
            RefreshList();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (_editing == null) { MessageBox.Show("请先选择或添加摄像头。"); return; }

            _cameraService?.Disconnect();
            switch (_editing.CameraType)
            {
                case "Hikvision":
                    _cameraService = new HikvisionCameraService();
                    break;
                case "ONVIF":
                    _cameraService = new OnvifCameraService();
                    break;
                default:
                    _cameraService = new GenericCameraService();
                    break;
            }

            _lprService = new PlateRecognizer(_editing.CameraType == "Hikvision");

            _cameraService.FrameCaptured += frame =>
            {
                if (_picPreview.IsDisposed) return;
                try { if (frame != null) _picPreview.Image?.Dispose(); _picPreview.Image = frame?.Clone() as Bitmap; }
                catch { }
            };

            if (_cameraService.Connect(_editing))
            {
                _btnConnect.Enabled = false;
                _btnDisconnect.Enabled = true;
                _lblStatus.Text = "状态: 已连接";
            }
            else
            {
                _lblStatus.Text = "状态: 连接失败";
            }
        }

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            if (_cameraService == null || !_cameraService.IsConnected)
            { MessageBox.Show("请先连接摄像头。"); return; }
            var snap = _cameraService.CaptureSnapshot();
            if (snap != null) { _picPreview.Image?.Dispose(); _picPreview.Image = snap; }
        }

        private void BtnRecognize_Click(object sender, EventArgs e)
        {
            if (_cameraService == null || !_cameraService.IsConnected)
            { MessageBox.Show("请先连接摄像头。"); return; }
            var snap = _cameraService.CaptureSnapshot();
            if (snap == null) return;
            _picPreview.Image?.Dispose(); _picPreview.Image = snap;

            var plate = _lprService?.Recognize(snap);
            _lblPlateResult.Text = !string.IsNullOrWhiteSpace(plate) ? "识别结果: " + plate : "识别结果: 未检测到车牌";
        }
    }
}
