using System;
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

        public CameraForm()
        {
            InitializeComponent();
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

            if (cam.IsDefault) DatabaseHelper.ExecuteNonQuery("UPDATE Cameras SET IsDefault=0");
            if (DatabaseHelper.SaveCamera(cam)) { _editing = null; RefreshList(); MessageBox.Show("保存成功。"); }
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            _cameraService?.Disconnect();
            Close();
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            _cameraService?.Disconnect();
            _btnConnect.Enabled = true;
            _btnDisconnect.Enabled = false;
            _lblStatus.Text = "状态: 已断开";
            _lblStatus.ForeColor = System.Drawing.Color.Gray;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            SelectCamera();
            if (_editing == null) { MessageBox.Show("请先选择或添加摄像头。"); return; }

            _cameraService?.Disconnect();
            switch (_editing.CameraType)
            {
                case "Hikvision": _cameraService = new HikvisionCameraService(); break;
                case "ONVIF": _cameraService = new OnvifCameraService(); break;
                default: _cameraService = new GenericCameraService(); break;
            }

            _lprService = new PlateRecognizer(_editing.CameraType == "Hikvision");

            _cameraService.FrameCaptured += frame =>
            {
                if (_picPreview.IsDisposed) return;
                try { _picPreview.Image?.Dispose(); _picPreview.Image = frame?.Clone() as System.Drawing.Bitmap; } catch { }
            };

            if (_cameraService.Connect(_editing))
            {
                _btnConnect.Enabled = false; _btnDisconnect.Enabled = true;
                bool isReal = (_cameraService is GenericCameraService gcs) ? gcs.IsRealCamera : true;
                _lblStatus.Text = isReal ? "状态: 已连接 (实时)" : "状态: 模拟模式";
                _lblStatus.ForeColor = isReal ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkOrange;
            }
            else { _lblStatus.Text = "状态: 连接失败"; _lblStatus.ForeColor = System.Drawing.Color.Red; }
        }

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            if (_cameraService == null || !_cameraService.IsConnected) { MessageBox.Show("请先连接摄像头。"); return; }
            var snap = _cameraService.CaptureSnapshot();
            if (snap != null)
            {
                _picPreview.Image?.Dispose(); _picPreview.Image = snap;
                var imgPath = Services.RecognitionManager.SaveSnapshotImage(snap, "snap_" + DateTime.Now.Ticks);
                _lblPlateResult.Text = "已保存抓拍: " + (imgPath ?? "失败");
            }
        }

        private void BtnRecognize_Click(object sender, EventArgs e)
        {
            if (_cameraService == null || !_cameraService.IsConnected) { MessageBox.Show("请先连接摄像头。"); return; }
            var snap = _cameraService.CaptureSnapshot();
            if (snap == null) return;
            _picPreview.Image?.Dispose(); _picPreview.Image = snap;

            var plate = (_lprService is PlateRecognizer pr) ? pr.RecognizeImmediate(snap) : _lprService?.Recognize(snap);
            if (!string.IsNullOrWhiteSpace(plate))
            {
                var record = Services.RecognitionManager.SaveRecognition(plate, snap, _editing?.Name ?? "N/A", _editing?.CameraType ?? "Generic", "Manual");
                _lblPlateResult.Text = string.Format("识别结果: {0}  |  已保存 ({1})", plate, record.ImagePath != null ? "含图片" : "无图片");
            }
            else _lblPlateResult.Text = "识别结果: 未检测到车牌";
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            var form = new Form { Text = "车牌识别记录", ClientSize = new System.Drawing.Size(860, 480), StartPosition = FormStartPosition.CenterParent, Font = new System.Drawing.Font("Microsoft YaHei UI", 9F) };
            var dgv = new DataGridView { Location = new System.Drawing.Point(12, 12), Size = new System.Drawing.Size(836, 420), AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, RowHeadersVisible = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgv.DataSource = DatabaseHelper.GetRecognitionRecords(DateTime.Today.AddDays(-30), DateTime.Now, null);
            Services.UIStyler.StyleDataGridView(dgv);
            var btnClose = new Button { Text = "关闭", Location = new System.Drawing.Point(770, 440), Size = new System.Drawing.Size(75, 28), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnClose.Click += (s, ev) => form.Close();
            form.Controls.Add(dgv); form.Controls.Add(btnClose);
            form.ShowDialog();
        }
    }
}
