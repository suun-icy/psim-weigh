using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using pism_weigh.Models;

namespace pism_weigh.Services
{
    /// <summary>
    /// 打印服务 — 通过 PrinterNative 修改 DEVMODE 解决自定义纸张大小问题
    /// 使用自定义 PrintPreviewForm（内嵌 PrintPreviewControl）替代 .NET 内置 PrintPreviewDialog
    /// </summary>
    public class PrintService
    {
        private PrintDocument _printDocument;
        private WeighRecord _record;
        private PrintTemplate _template;
        private Action _onPrintedCallback;

        // 目标纸张尺寸（1/100 英寸）：240mm≈945, 93.1mm≈367
        private const int PaperWidth = 945;
        private const int PaperHeight = 367;

        public PrintService()
        {
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
            _printDocument.OriginAtMargins = true;
        }

        /// <summary>
        /// 设置打印机
        /// </summary>
        public void SetPrinter(string printerName)
        {
            if (!string.IsNullOrEmpty(printerName))
            {
                _printDocument.PrinterSettings.PrinterName = printerName;
            }
        }

        /// <summary>
        /// 获取可用的打印机列表
        /// </summary>
        public static string[] GetAvailablePrinters()
        {
            var printers = new string[PrinterSettings.InstalledPrinters.Count];
            PrinterSettings.InstalledPrinters.CopyTo(printers, 0);
            return printers;
        }

        /// <summary>
        /// 打印预览（使用默认打印机，不弹选择对话框）
        /// </summary>
        public void PrintPreview(WeighRecord record, Action onPrinted = null)
        {
            _record = record;
            _template = PrintTemplate.WeighSlip240x93;
            _onPrintedCallback = onPrinted;

            PrepareDocument();

            var previewForm = new PrintPreviewForm(_printDocument, "磅单预览");
            previewForm.OnPrinted += () => { if (_onPrintedCallback != null) _onPrintedCallback(); };
            previewForm.ShowDialog();
        }

        /// <summary>
        /// 打印预览（先弹出打印机选择对话框）
        /// 返回 false 表示用户取消了打印机选择
        /// </summary>
        public bool PrintPreviewWithPrinterDialog(WeighRecord record, Action onPrinted = null)
        {
            _record = record;
            _template = PrintTemplate.WeighSlip240x93;
            _onPrintedCallback = onPrinted;

            PrepareDocument();

            var printDialog = new PrintDialog
            {
                Document = _printDocument,
                AllowSomePages = false,
                UseEXDialog = true
            };

            if (printDialog.ShowDialog() != DialogResult.OK)
                return false;

            _printDocument.PrinterSettings = printDialog.PrinterSettings;

            // 换打印机后设置 DEVMODE 纸张
            ApplyPaperSettings();

            var previewForm = new PrintPreviewForm(_printDocument, "磅单预览");
            previewForm.OnPrinted += () => { if (_onPrintedCallback != null) _onPrintedCallback(); };
            previewForm.ShowDialog();
            return true;
        }

        /// <summary>
        /// 应用纸张设置（DEVMODE + .NET 双保险）
        /// </summary>
        private void ApplyPaperSettings()
        {
            // 第一层：DEVMODE 级（驱动层，最可靠）
            PrinterNative.SetCustomPaper240x93(_printDocument.PrinterSettings);

            // 第二层：.NET PaperSize（兼容层）
            var matched = PrinterNative.FindMatchingPaperSize(
                _printDocument.PrinterSettings, PaperWidth, PaperHeight);
            if (matched != null)
            {
                _printDocument.DefaultPageSettings.PaperSize = matched;
            }
            else
            {
                var custom = new PaperSize("WeighSlip", PaperWidth, PaperHeight);
                custom.RawKind = (int)PaperKind.Custom;
                _printDocument.DefaultPageSettings.PaperSize = custom;
            }

            _printDocument.DefaultPageSettings.Landscape = false;
            _printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 8, 8);
        }

        /// <summary>
        /// 打印前准备文档
        /// </summary>
        private void PrepareDocument()
        {
            ApplyPaperSettings();
            _printDocument.PrintController = new StandardPrintController();
        }

        // ============================================================
        // 打印页面绘制（GDI+ 毫米级精确定位，左右4列布局）
        // ============================================================

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_record == null || _template == null) return;

            Graphics g = e.Graphics;
            g.PageUnit = GraphicsUnit.Millimeter;

            float x = _template.MarginLeft;
            float y = _template.MarginTop;
            float pageW = _template.PageWidth;
            float tableWidth = pageW - _template.MarginLeft - _template.MarginRight;

            float col1W = tableWidth * 0.12f;
            float col2W = tableWidth * 0.38f;
            float col3W = tableWidth * 0.12f;
            float col4W = tableWidth * 0.38f;

            using (var pen = new Pen(Color.Black, 0.3f))
            using (var titleFont = new Font("宋体", 13.5f, FontStyle.Bold))
            using (var labelFont = new Font("宋体", 8.25f, FontStyle.Regular))
            using (var valueFont = new Font("宋体", 8.25f, FontStyle.Regular))
            using (var timeFont = new Font("宋体", 6.75f, FontStyle.Regular))
            {
                // 标题 — 无边框，居中
                var titleSF = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var titleRect = new RectangleF(x, y, tableWidth, 15f);
                g.DrawString("磅  单", titleFont, Brushes.Black, titleRect, titleSF);
                y += 10f;

                // 时间行 — 无边框
                var timeText = "时间  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var timeSize = g.MeasureString(timeText, timeFont);
                g.DrawString(timeText, timeFont, Brushes.Black, x + 1f, y + 1f);
                y += timeSize.Height + 2f;

                float rowH = 10f;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "车牌",     _record.PlateNumber ?? "",
                    "毛重",     _record.GrossWeight.ToString("F0") + " kg");
                y += rowH;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "运输单位", _record.Sender ?? "",
                    "皮重",     _record.TareWeight.ToString("F0") + " kg");
                y += rowH;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "运输内容", _record.CargoType ?? "",
                    "净重",     _record.NetWeight.ToString("F0") + " kg");
                y += rowH;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "送货地点", _record.Receiver ?? "",
                    "毛重时间", (_record.FirstWeighTime != null ? _record.FirstWeighTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
                y += rowH;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "送货单位", _record.Receiver ?? "",
                    "皮重时间", (_record.SecondWeighTime != null ? _record.SecondWeighTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
                y += rowH;

                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "司机",     _record.DriverName ?? "",
                    "司磅员",  _record.OperatorName ?? "");
            }

            e.HasMorePages = false;
        }

        private void DrawFourColRow(Graphics g, Pen pen, Font labelFont, Font valueFont,
            float x, float y, float w1, float w2, float w3, float w4, float h,
            string label1, string value1, string label2, string value2)
        {
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            g.FillRectangle(Brushes.White, x, y, w1, h);
            g.DrawRectangle(pen, x, y, w1, h);
            if (!string.IsNullOrEmpty(label1))
                g.DrawString(label1, labelFont, Brushes.Black, new RectangleF(x + 1, y, w1 - 2, h), sfCenter);

            g.DrawRectangle(pen, x + w1, y, w2, h);
            g.DrawString(value1 ?? "", valueFont, Brushes.Black, new RectangleF(x + w1 + 1, y, w2 - 2, h), sfLeft);

            g.DrawRectangle(pen, x + w1 + w2, y, w3, h);
            if (!string.IsNullOrEmpty(label2))
                g.DrawString(label2, labelFont, Brushes.Black, new RectangleF(x + w1 + w2 + 1, y, w3 - 2, h), sfCenter);

            g.DrawRectangle(pen, x + w1 + w2 + w3, y, w4, h);
            g.DrawString(value2 ?? "", valueFont, Brushes.Black, new RectangleF(x + w1 + w2 + w3 + 1, y, w4 - 2, h), sfLeft);
        }
    }

    // ============================================================
    // 打印模板配置
    // ============================================================

    public class PrintTemplate
    {
        public string Title { get; set; }
        public float PageWidth { get; set; } = 210;
        public float PageHeight { get; set; } = 297;
        public float MarginLeft { get; set; } = 20;
        public float MarginRight { get; set; } = 20;
        public float MarginTop { get; set; } = 20;
        public float MarginBottom { get; set; } = 20;
        public float RowHeight { get; set; } = 8;

        public static PrintTemplate WeighSlip240x93 => new PrintTemplate
        {
            Title = "磅单",
            PageWidth = 240,
            PageHeight = 93.1f,
            MarginLeft = 6,
            MarginRight = 6,
            MarginTop = 3,
            MarginBottom = 2,
            RowHeight = 5
        };

        public static PrintTemplate A4 => new PrintTemplate
        {
            Title = "产品过磅单",
            PageWidth = 210,
            PageHeight = 297,
            MarginLeft = 20,
            MarginRight = 20,
            MarginTop = 20,
            RowHeight = 8
        };

        public PrintTemplate()
        {
            Title = "称重单";
        }
    }
}
