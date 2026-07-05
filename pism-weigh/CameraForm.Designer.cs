using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh
{
    partial class CameraForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // ===== 控件声明（设计器可见） =====
        private Panel pnlList;
        private DataGridView _dgvCameras;
        private Button _btnSave, _btnDelete, _btnSetDefault, _btnHistory, _btnClose;
        private Panel pnlRight;
        private GroupBox grpConfig, grpPreview;
        private TextBox _txtName, _txtIP, _txtPort, _txtUser, _txtPwd, _txtRTSP, _txtChannel;
        private ComboBox _cboType, _cboResolution;
        private CheckBox _chkEnabled;
        private PictureBox _picPreview;
        private Button _btnConnect, _btnDisconnect, _btnSnapshot, _btnRecognize;
        private Label _lblStatus, _lblPlateResult;
        private Label lblName, lblType, lblIP, lblPort, lblUser, lblPwd, lblChannel, lblRTSP, lblRes;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ClientSize = new Size(800, 550);
            this.MinimumSize = new Size(650, 450);
            this.Text = "摄像头管理";
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += CameraForm_Load;

            // === 左侧面板 ===
            pnlList = new Panel
            {
                Location = new Point(12, 12), Size = new Size(280, 526),
                BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            _dgvCameras = new DataGridView
            {
                Location = new Point(8, 8), Size = new Size(264, 460),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            _dgvCameras.SelectionChanged += (s, e) => SelectCamera();
            pnlList.Controls.Add(_dgvCameras);

            _btnSave = new Button { Text = "保存配置", Location = new Point(8, 474), Size = new Size(75, 30), BackColor = Color.FromArgb(24, 144, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false };
            _btnDelete = new Button { Text = "删除", Location = new Point(88, 474), Size = new Size(55, 30), UseVisualStyleBackColor = true };
            _btnSetDefault = new Button { Text = "默认", Location = new Point(148, 474), Size = new Size(55, 30), UseVisualStyleBackColor = true };
            _btnHistory = new Button { Text = "记录", Location = new Point(208, 474), Size = new Size(55, 30), UseVisualStyleBackColor = true };
            _btnClose = new Button { Text = "关闭", Location = new Point(218, 490), Size = new Size(55, 28), UseVisualStyleBackColor = true };

            _btnSave.Click += BtnSave_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnSetDefault.Click += BtnSetDefault_Click;
            _btnHistory.Click += BtnHistory_Click;
            _btnClose.Click += (s, e) => { _cameraService?.Disconnect(); Close(); };
            pnlList.Controls.AddRange(new Control[] { _btnSave, _btnDelete, _btnSetDefault, _btnHistory, _btnClose });
            this.Controls.Add(pnlList);

            // === 右侧面板 ===
            pnlRight = new Panel
            {
                Location = new Point(300, 12), Size = new Size(488, 526),
                BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // 配置区
            grpConfig = new GroupBox { Text = "摄像头参数", Location = new Point(8, 8), Size = new Size(472, 250), Font = new Font("Microsoft YaHei UI", 9F) };

            int y = 24;
            (lblName, _txtName) = AddRow(grpConfig, "名称", ref y, 150);
            (lblType, _cboType) = AddComboRow(grpConfig, "类型", ref y, 150, "Generic", "ONVIF", "Hikvision", "USB");
            (lblIP, _txtIP) = AddRow(grpConfig, "IP地址", ref y, 150);
            (lblPort, _txtPort) = AddRow(grpConfig, "端口", ref y, 80);
            (lblUser, _txtUser) = AddRow(grpConfig, "用户名", ref y, 120);
            (lblPwd, _txtPwd) = AddRow(grpConfig, "密码", ref y, 120);
            _txtPwd.PasswordChar = '*';
            (lblChannel, _txtChannel) = AddRow(grpConfig, "通道号", ref y, 60);
            (lblRTSP, _txtRTSP) = AddRow(grpConfig, "RTSP地址", ref y, 300);
            (lblRes, _cboResolution) = AddComboRow(grpConfig, "分辨率", ref y, 120, "1920x1080", "1280x720", "640x480");
            _chkEnabled = new CheckBox { Text = "启用", Location = new Point(80, y), Size = new Size(60, 24), Checked = true };
            grpConfig.Controls.Add(_chkEnabled);
            pnlRight.Controls.Add(grpConfig);

            // 预览区
            grpPreview = new GroupBox { Text = "实时预览", Location = new Point(8, 266), Size = new Size(472, 250), Font = new Font("Microsoft YaHei UI", 9F), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

            _picPreview = new PictureBox
            {
                Location = new Point(6, 16), Size = new Size(458, 154),
                BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            grpPreview.Controls.Add(_picPreview);

            _btnConnect = new Button { Text = "连接", Location = new Point(8, 176), Size = new Size(70, 28), BackColor = Color.FromArgb(82, 196, 26), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false };
            _btnDisconnect = new Button { Text = "断开", Location = new Point(84, 176), Size = new Size(70, 28), Enabled = false, UseVisualStyleBackColor = true };
            _btnSnapshot = new Button { Text = "抓拍", Location = new Point(160, 176), Size = new Size(70, 28), UseVisualStyleBackColor = true };
            _btnRecognize = new Button { Text = "识别车牌", Location = new Point(236, 176), Size = new Size(80, 28), BackColor = Color.FromArgb(250, 173, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false };
            _lblStatus = new Label { Text = "状态: 未连接", Location = new Point(322, 180), Size = new Size(140, 20), ForeColor = Color.Gray };
            _lblPlateResult = new Label { Text = "", Location = new Point(8, 210), Size = new Size(456, 20), Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold), ForeColor = Color.DarkGreen };

            _btnConnect.Click += BtnConnect_Click;
            _btnDisconnect.Click += (s, e) => { _cameraService?.Disconnect(); _btnConnect.Enabled = true; _btnDisconnect.Enabled = false; _lblStatus.Text = "状态: 已断开"; _lblStatus.ForeColor = Color.Gray; };
            _btnSnapshot.Click += BtnSnapshot_Click;
            _btnRecognize.Click += BtnRecognize_Click;

            grpPreview.Controls.AddRange(new Control[] { _btnConnect, _btnDisconnect, _btnSnapshot, _btnRecognize, _lblStatus, _lblPlateResult });
            pnlRight.Controls.Add(grpPreview);
            this.Controls.Add(pnlRight);

            this.FormClosing += (s, e) => { if (!DesignMode) _cameraService?.Disconnect(); };
        }

        private (Label, TextBox) AddRow(Control parent, string label, ref int y, int tw)
        {
            var lbl = new Label { Text = label, Location = new Point(8, y + 3), Size = new Size(65, 24) };
            var txt = new TextBox { Location = new Point(80, y), Size = new Size(tw, 24), BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            y += 30;
            return (lbl, txt);
        }

        private (Label, ComboBox) AddComboRow(Control parent, string label, ref int y, int tw, params string[] items)
        {
            var lbl = new Label { Text = label, Location = new Point(8, y + 3), Size = new Size(65, 24) };
            var cbo = new ComboBox { Location = new Point(80, y), Size = new Size(tw, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            parent.Controls.Add(lbl);
            parent.Controls.Add(cbo);
            y += 30;
            return (lbl, cbo);
        }

        private void CameraForm_Load(object sender, System.EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            Services.UIStyler.StyleForm(this, "摄像头管理");
            Services.UIStyler.StyleDataGridView(_dgvCameras);
            RefreshList();
        }
    }
}
