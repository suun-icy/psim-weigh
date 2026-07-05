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
            _btnClose.Click += BtnClose_Click;
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

            // 名称
            lblName = new Label { Text = "名称", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtName = new TextBox { Location = new Point(80, y), Size = new Size(150, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblName);
            grpConfig.Controls.Add(_txtName);
            y += 30;

            // 类型
            lblType = new Label { Text = "类型", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _cboType = new ComboBox { Location = new Point(80, y), Size = new Size(150, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboType.Items.AddRange(new[] { "Generic", "ONVIF", "Hikvision", "USB" });
            _cboType.SelectedIndex = 0;
            grpConfig.Controls.Add(lblType);
            grpConfig.Controls.Add(_cboType);
            y += 30;

            // IP地址
            lblIP = new Label { Text = "IP地址", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtIP = new TextBox { Location = new Point(80, y), Size = new Size(150, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblIP);
            grpConfig.Controls.Add(_txtIP);
            y += 30;

            // 端口
            lblPort = new Label { Text = "端口", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtPort = new TextBox { Location = new Point(80, y), Size = new Size(80, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblPort);
            grpConfig.Controls.Add(_txtPort);
            y += 30;

            // 用户名
            lblUser = new Label { Text = "用户名", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtUser = new TextBox { Location = new Point(80, y), Size = new Size(120, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblUser);
            grpConfig.Controls.Add(_txtUser);
            y += 30;

            // 密码
            lblPwd = new Label { Text = "密码", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtPwd = new TextBox { Location = new Point(80, y), Size = new Size(120, 24), BorderStyle = BorderStyle.FixedSingle, PasswordChar = '*' };
            grpConfig.Controls.Add(lblPwd);
            grpConfig.Controls.Add(_txtPwd);
            y += 30;

            // 通道号
            lblChannel = new Label { Text = "通道号", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtChannel = new TextBox { Location = new Point(80, y), Size = new Size(60, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblChannel);
            grpConfig.Controls.Add(_txtChannel);
            y += 30;

            // RTSP地址
            lblRTSP = new Label { Text = "RTSP地址", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _txtRTSP = new TextBox { Location = new Point(80, y), Size = new Size(300, 24), BorderStyle = BorderStyle.FixedSingle };
            grpConfig.Controls.Add(lblRTSP);
            grpConfig.Controls.Add(_txtRTSP);
            y += 30;

            // 分辨率
            lblRes = new Label { Text = "分辨率", Location = new Point(8, y + 3), Size = new Size(65, 24) };
            _cboResolution = new ComboBox { Location = new Point(80, y), Size = new Size(120, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboResolution.Items.AddRange(new[] { "1920x1080", "1280x720", "640x480" });
            _cboResolution.SelectedIndex = 0;
            grpConfig.Controls.Add(lblRes);
            grpConfig.Controls.Add(_cboResolution);
            y += 30;

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
            _btnDisconnect.Click += BtnDisconnect_Click;
            _btnSnapshot.Click += BtnSnapshot_Click;
            _btnRecognize.Click += BtnRecognize_Click;

            grpPreview.Controls.AddRange(new Control[] { _btnConnect, _btnDisconnect, _btnSnapshot, _btnRecognize, _lblStatus, _lblPlateResult });
            pnlRight.Controls.Add(grpPreview);
            this.Controls.Add(pnlRight);

            this.FormClosing += CameraForm_FormClosing;
        }

        private void CameraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DesignMode)
                _cameraService?.Disconnect();
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
