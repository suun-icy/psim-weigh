using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using pism_weigh.Models;

namespace pism_weigh.Services
{
    /// <summary>
    /// 打印服务 - 支持多种打印模板
    /// </summary>
    public class PrintService
    {
        private PrintDocument _printDocument;
        private WeighRecord _record;
        private PrintTemplate _template;

        // 打印配置
        public string PrinterName { get; set; }
        public int Copies { get; set; } = 1;

        public event Action<bool, string> PrintCompleted;

        public PrintService()
        {
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
            _template = PrintTemplate.WeighSlip240x93;

            // 设置默认纸张为 24cm×9.31cm
            SetDefaultPaperSize();
        }

        /// <summary>
        /// 设置默认纸张大小
        /// </summary>
        private void SetDefaultPaperSize()
        {
            try
            {
                // PaperSize 单位是 1/100 英寸：240mm≈945, 93.1mm≈367
                var paperSize = new PaperSize("WeighSlip", 945, 367);
                paperSize.RawKind = (int)PaperKind.Custom;
                _printDocument.DefaultPageSettings.PaperSize = paperSize;
                _printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 8, 8);
            }
            catch { }
        }

        /// <summary>
        /// 打印预览（先弹出打印机选择对话框）
        /// </summary>
        public bool PrintPreviewWithDialog(WeighRecord record, PrintTemplate template = null)
        {
            _record = record;
            _template = template ?? PrintTemplate.WeighSlip240x93;

            SetDefaultPaperSize();

            var printDialog = new PrintDialog
            {
                Document = _printDocument,
                AllowSomePages = false,
                UseEXDialog = true
            };

            if (printDialog.ShowDialog() != DialogResult.OK)
                return false;

            _printDocument.PrinterSettings = printDialog.PrinterSettings;

            var previewDialog = new PrintPreviewDialog
            {
                Document = _printDocument,
                Width = 1000,
                Height = 700,
                UseAntiAlias = true
            };
            previewDialog.ShowDialog();
            return true;
        }

        /// <summary>
        /// 设置打印机
        /// </summary>
        public void SetPrinter(string printerName)
        {
            if (!string.IsNullOrEmpty(printerName))
            {
                _printDocument.PrinterSettings.PrinterName = printerName;
                PrinterName = printerName;
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
        /// 打印称重单
        /// </summary>
        public bool Print(WeighRecord record, PrintTemplate template = null)
        {
            try
            {
                _record = record;
                _template = template ?? PrintTemplate.WeighSlip240x93;
                _printDocument.PrinterSettings.Copies = (short)Copies;

                _printDocument.Print();
                return true;
            }
            catch (Exception ex)
            {
                PrintCompleted?.Invoke(false, "打印失败：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        public void PrintPreview(WeighRecord record, PrintTemplate template = null)
        {
            _record = record;
            _template = template ?? PrintTemplate.WeighSlip240x93;

            var previewDialog = new PrintPreviewDialog
            {
                Document = _printDocument,
                Width = 1000,
                Height = 700,
                UseAntiAlias = true
            };
            previewDialog.ShowDialog();
        }

        /// <summary>
        /// 打印页面处理 - 磅单格式（左右4列布局）
        /// </summary>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_record == null || _template == null) return;

            Graphics g = e.Graphics;
            g.PageUnit = GraphicsUnit.Millimeter;

            float x = _template.MarginLeft;
            float y = _template.MarginTop;
            float pageW = _template.PageWidth;
            float tableWidth = pageW - _template.MarginLeft - _template.MarginRight;

            // 四列宽度比例: 12% | 38% | 12% | 38%
            float col1W = tableWidth * 0.12f;  // 左标签列
            float col2W = tableWidth * 0.38f;  // 左数据列
            float col3W = tableWidth * 0.12f;  // 右标签列
            float col4W = tableWidth * 0.38f;  // 右数据列

            using (var pen = new Pen(Color.Black, 0.3f))
            using (var titleFont = new Font("宋体", 5f, FontStyle.Bold))       // 磅单标题 ≈16px
            using (var labelFont = new Font("宋体", 3.5f, FontStyle.Regular))  // 标签文字 ≈12px
            using (var valueFont = new Font("宋体", 3.5f, FontStyle.Regular))  // 数据文字
            using (var timeFont = new Font("宋体", 3f, FontStyle.Regular))     // 时间行文字 ≈10px
            {
                // ===== 第1行：标题（有边框）=====
                var titleRect = new RectangleF(x, y, tableWidth, 7f);
                g.FillRectangle(Brushes.White, titleRect);
                g.DrawRectangle(pen, x, y, tableWidth, 7f);

                var titleSF = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("磅  单", titleFont, Brushes.Black, titleRect, titleSF);
                y += 7f;

                // ===== 第2行：时间行（无边框）=====
                var timeText = "时间  " + DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                var timeSize = g.MeasureString(timeText, timeFont);
                g.DrawString(timeText, timeFont, Brushes.Black, x + 1f, y + 1f);
                y += timeSize.Height + 1f;

                float rowH = 6f;

                // ===== 第3行：车牌 | 数据 | 毛重 | 数据 =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "车牌", _record.PlateNumber ?? "",
                    "毛重", _record.GrossWeight.ToString("F0") + " kg");
                y += rowH;

                // ===== 第4行：运输单位 | 数据 | 皮重 | 数据 =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "运输单位", _record.Sender ?? "",
                    "皮重", _record.TareWeight.ToString("F0") + " kg");
                y += rowH;

                // ===== 第5行：运输内容 | 数据 | 净重 | 数据 =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "运输内容", _record.CargoType ?? "",
                    "净重", _record.NetWeight.ToString("F0") + " kg");
                y += rowH;

                // ===== 第6行：送货地点 | 数据 | 毛重时间 | 数据 =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "送货地点", _record.Receiver ?? "",
                    "毛重时间", _record.FirstWeighTime?.ToString("MM/dd HH:mm") ?? "");
                y += rowH;

                // ===== 第7行：送货单位 | 数据 | 皮重时间 | 数据 =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "送货单位", _record.Receiver ?? "",
                    "皮重时间", _record.SecondWeighTime?.ToString("MM/dd HH:mm") ?? "");
                y += rowH;

                // ===== 第8行：司机 | 数据 | (空) | (空) =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "司机", _record.DriverName ?? "",
                    "", "");
                y += rowH;

                // ===== 第9行：司磅员 | 数据 | (空) | (空) =====
                DrawFourColRow(g, pen, labelFont, valueFont, x, y, col1W, col2W, col3W, col4W, rowH,
                    "司磅员", _record.OperatorName ?? "",
                    "", "");
                y += rowH;
            }

            e.HasMorePages = false;
        }

        /// <summary>
        /// 绘制一行四列（带边框和文字居中/左对齐）
        /// </summary>
        private void DrawFourColRow(Graphics g, Pen pen, Font labelFont, Font valueFont,
            float x, float y, float w1, float w2, float w3, float w4, float h,
            string label1, string value1, string label2, string value2)
        {
            var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            // 列1：标签
            var r1 = new RectangleF(x, y, w1, h);
            g.FillRectangle(Brushes.White, r1);
            g.DrawRectangle(pen, x, y, w1, h);
            if (!string.IsNullOrEmpty(label1))
                g.DrawString(label1, labelFont, Brushes.Black, new RectangleF(x + 1, y, w1 - 2, h), sfCenter);

            // 列2：数据（左对齐）
            var r2 = new RectangleF(x + w1, y, w2, h);
            g.DrawRectangle(pen, x + w1, y, w2, h);
            g.DrawString(value1 ?? "", valueFont, Brushes.Black, new RectangleF(x + w1 + 1, y, w2 - 2, h), sfLeft);

            // 列3：标签
            var r3 = new RectangleF(x + w1 + w2, y, w3, h);
            g.DrawRectangle(pen, x + w1 + w2, y, w3, h);
            if (!string.IsNullOrEmpty(label2))
                g.DrawString(label2, labelFont, Brushes.Black, new RectangleF(x + w1 + w2 + 1, y, w3 - 2, h), sfCenter);

            // 列4：数据（左对齐）
            var r4 = new RectangleF(x + w1 + w2 + w3, y, w4, h);
            g.DrawRectangle(pen, x + w1 + w2 + w3, y, w4, h);
            g.DrawString(value2 ?? "", valueFont, Brushes.Black, new RectangleF(x + w1 + w2 + w3 + 1, y, w4 - 2, h), sfLeft);
        }

        private string GetBusinessTypeText(BusinessType type)
        {
            switch (type)
            {
                case BusinessType.PurchaseIn: return "采购入库";
                case BusinessType.SalesOut: return "销售出库";
                case BusinessType.Transfer: return "内部调拨";
                default: return "其他";
            }
        }

        /// <summary>
        /// 打印设置对话框
        /// </summary>
        public bool ShowPrintDialog()
        {
            var dialog = new PrintDialog
            {
                Document = _printDocument,
                UseEXDialog = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PrinterName = _printDocument.PrinterSettings.PrinterName;
                Copies = _printDocument.PrinterSettings.Copies;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 打印模板配置（单位：mm）
    /// </summary>
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

        /// <summary>
        /// 磅单模板：24cm × 9.31cm（匹配磅单打印纸）
        /// </summary>
        public static PrintTemplate WeighSlip240x93 => new PrintTemplate
        {
            Title = "磅单",
            PageWidth = 240,
            PageHeight = 93.1f,
            MarginLeft = 6,
            MarginRight = 6,
            MarginTop = 3,
            MarginBottom = 2,
            RowHeight = 6
        };

        /// <summary>
        /// 标准模板
        /// </summary>
        public static PrintTemplate Standard => new PrintTemplate
        {
            Title = "称重单",
            PageWidth = 210,
            PageHeight = 140,
            MarginLeft = 10,
            MarginRight = 10,
            MarginTop = 10,
            RowHeight = 7
        };

        /// <summary>
        /// A4 模板
        /// </summary>
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

        /// <summary>
        /// 80mm 小票模板
        /// </summary>
        public static PrintTemplate Receipt80 => new PrintTemplate
        {
            Title = "称重单",
            PageWidth = 80,
            PageHeight = 200,
            MarginLeft = 5,
            MarginRight = 5,
            MarginTop = 5,
            RowHeight = 6
        };

        public PrintTemplate()
        {
            Title = "称重单";
        }
    }
}
