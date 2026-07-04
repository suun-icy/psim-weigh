using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace pism_weigh.Services
{
    /// <summary>
    /// 自定义打印预览窗体 — 替代 .NET 内置 PrintPreviewDialog
    /// 关键区别：用自己的"打印"按钮替代 PrintPreviewDialog 内置打印按钮，
    /// 在打印前通过 PrinterNative 修改 DEVMODE 确保纸张大小不被重置
    /// </summary>
    public class PrintPreviewForm : Form
    {
        private readonly PrintDocument _document;
        private readonly ToolStripButton _btnPrint;
        private readonly ToolStripButton _btnPageSetup;
        private readonly ToolStripButton _btnZoomIn;
        private readonly ToolStripButton _btnZoomOut;
        private readonly ToolStripButton _btnClose;
        private readonly PrintPreviewControl _preview;
        private readonly ToolStrip _toolbar;

        /// <summary>
        /// 用户点击"打印"后回调。可在回调中更新数据库（如打印次数）
        /// </summary>
        public event Action OnPrinted;

        public PrintPreviewForm(PrintDocument document, string title = "打印预览")
        {
            if (document == null) throw new ArgumentNullException("document");
            _document = document;

            Text = title;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Microsoft YaHei UI", 9F);

            _toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };

            _btnPrint = new ToolStripButton("打印") { Image = null };
            _btnPageSetup = new ToolStripButton("打印设置") { Image = null };
            _btnZoomIn = new ToolStripButton("放大") { Image = null };
            _btnZoomOut = new ToolStripButton("缩小") { Image = null };
            _btnClose = new ToolStripButton("关闭") { Image = null };

            _btnPrint.Click += BtnPrint_Click;
            _btnPageSetup.Click += BtnPageSetup_Click;
            _btnZoomIn.Click += (s, e) => { if (_preview.Zoom < 3.0) _preview.Zoom *= 1.25; };
            _btnZoomOut.Click += (s, e) => { if (_preview.Zoom > 0.15) _preview.Zoom /= 1.25; };
            _btnClose.Click += (s, e) => Close();

            _toolbar.Items.Add(_btnPrint);
            _toolbar.Items.Add(_btnPageSetup);
            _toolbar.Items.Add(new ToolStripSeparator());
            _toolbar.Items.Add(_btnZoomIn);
            _toolbar.Items.Add(_btnZoomOut);
            _toolbar.Items.Add(new ToolStripSeparator());
            _toolbar.Items.Add(_btnClose);

            _preview = new PrintPreviewControl
            {
                Dock = DockStyle.Fill,
                Document = _document,
                Zoom = 1.0,
                AutoZoom = true
            };

            Controls.Add(_preview);
            Controls.Add(_toolbar);
        }

        /// <summary>
        /// 判断是否为虚拟打印机（如 PDF、XPS 等），这类打印机通常能正确处理自定义纸张
        /// </summary>
        private bool IsVirtualPrinter(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string lower = name.ToLowerInvariant();
            return lower.Contains("pdf") || lower.Contains("xps") ||
                   lower.Contains("onenote") || lower.Contains("fax");
        }

        /// <summary>
        /// 打印按钮 — DEVMODE 方式设置纸张后直接打印
        /// </summary>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var printerName = _document.PrinterSettings.PrinterName;

                // 1. 尝试 DEVMODE 级别设置纸张大小
                PrinterNative.SetCustomPaper240x93(_document.PrinterSettings);

                // 2. 同时尝试 .NET 层面设置（双保险）
                var matched = PrinterNative.FindMatchingPaperSize(
                    _document.PrinterSettings, 945, 367);
                if (matched != null)
                {
                    _document.DefaultPageSettings.PaperSize = matched;
                }
                else
                {
                    var custom = new PaperSize("WeighSlip", 945, 367);
                    custom.RawKind = (int)PaperKind.Custom;
                    _document.DefaultPageSettings.PaperSize = custom;
                }
                _document.DefaultPageSettings.Landscape = false;
                _document.DefaultPageSettings.Margins = new Margins(10, 10, 8, 8);

                // 3. 对虚拟打印机（PDF 等），额外在 QueryPageSettings 中强制设置
                if (!IsVirtualPrinter(printerName))
                {
                    _document.OriginAtMargins = true;
                    _document.PrintController = new StandardPrintController();
                }

                _document.Print();

                if (OnPrinted != null) OnPrinted();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印失败：" + ex.Message, "打印错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 打印设置 — 弹出打印机选择 + 页面设置
        /// </summary>
        private void BtnPageSetup_Click(object sender, EventArgs e)
        {
            // 先选择打印机
            var printDialog = new PrintDialog
            {
                Document = _document,
                AllowSomePages = false,
                UseEXDialog = true
            };

            if (printDialog.ShowDialog(this) == DialogResult.OK)
            {
                _document.PrinterSettings = printDialog.PrinterSettings;

                // 换打印机后重新设置纸张
                PrinterNative.SetCustomPaper240x93(_document.PrinterSettings);
                var matched = PrinterNative.FindMatchingPaperSize(
                    _document.PrinterSettings, 945, 367);
                if (matched != null)
                    _document.DefaultPageSettings.PaperSize = matched;
                else
                {
                    var custom = new PaperSize("WeighSlip", 945, 367);
                    custom.RawKind = (int)PaperKind.Custom;
                    _document.DefaultPageSettings.PaperSize = custom;
                }
                _document.DefaultPageSettings.Landscape = false;

                // 刷新预览
                _preview.InvalidatePreview();
            }

            // 再显示页面设置对话框（可调整边距等）
            var pageSetupDialog = new PageSetupDialog
            {
                Document = _document,
                AllowPaper = true,
                AllowMargins = true,
                AllowOrientation = true,
                ShowNetwork = false
            };
            pageSetupDialog.ShowDialog(this);
            _preview.InvalidatePreview();
        }
    }
}
