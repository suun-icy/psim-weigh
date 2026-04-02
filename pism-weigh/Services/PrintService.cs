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
            _template = PrintTemplate.Standard;
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
                _template = template ?? PrintTemplate.Standard;
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
            _template = template ?? PrintTemplate.Standard;
            
            var previewDialog = new PrintPreviewDialog
            {
                Document = _printDocument,
                Width = 800,
                Height = 600,
                UseAntiAlias = true
            };
            previewDialog.ShowDialog();
        }
        
        /// <summary>
        /// 打印页面处理
        /// </summary>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_record == null || _template == null) return;
            
            Graphics g = e.Graphics;
            float x = _template.MarginLeft;
            float y = _template.MarginTop;
            
            // 绘制标题
            if (!string.IsNullOrEmpty(_template.Title))
            {
                using (var titleFont = new Font("宋体", 20, FontStyle.Bold))
                {
                    var titleSize = g.MeasureString(_template.Title, titleFont);
                    float titleX = (_template.PageWidth - titleSize.Width) / 2;
                    g.DrawString(_template.Title, titleFont, Brushes.Black, titleX, y);
                    y += titleSize.Height + 10;
                }
            }
            
            // 绘制表格线
            float tableWidth = _template.PageWidth - _template.MarginLeft - _template.MarginRight;
            float rowHeight = _template.RowHeight;
            
            // 表头
            string[] headers = { "项目", "内容" };
            float[] colWidths = { tableWidth * 0.3f, tableWidth * 0.7f };
            
            using (var headerFont = new Font("宋体", 12, FontStyle.Bold))
            using (var contentFont = new Font("宋体", 12))
            using (var pen = new Pen(Color.Black, 1))
            {
                // 绘制表头背景
                var headerRect = new RectangleF(x, y, tableWidth, rowHeight);
                g.FillRectangle(Brushes.LightGray, headerRect);
                g.DrawRectangle(pen, x, y, tableWidth, rowHeight);
                
                // 绘制表头文字
                g.DrawString(headers[0], headerFont, Brushes.Black, x + 5, y + 3);
                g.DrawString(headers[1], headerFont, Brushes.Black, x + colWidths[0] + 5, y + 3);
                y += rowHeight;
                
                // 绘制数据行
                var dataRows = GetDataRows(_record);
                foreach (var row in dataRows)
                {
                    g.DrawRectangle(pen, x, y, tableWidth, rowHeight);
                    g.DrawLine(pen, x, y + rowHeight, x + tableWidth, y + rowHeight);
                    
                    g.DrawString(row.Label, contentFont, Brushes.Black, x + 5, y + 3);
                    g.DrawString(row.Value, contentFont, Brushes.Black, x + colWidths[0] + 5, y + 3);
                    
                    y += rowHeight;
                }
                
                // 底部签名区域
                y += 20;
                using (var signFont = new Font("宋体", 10))
                {
                    g.DrawString("司磅员：_________________", signFont, Brushes.Black, x, y);
                    y += 25;
                    g.DrawString("司机签字：_________________", signFont, Brushes.Black, x, y);
                    y += 25;
                    g.DrawString("打印时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), signFont, Brushes.Black, x, y);
                }
            }
        }
        
        /// <summary>
        /// 获取打印数据行
        /// </summary>
        private (string Label, string Value)[] GetDataRows(WeighRecord record)
        {
            return new[]
            {
                ("车牌号码", record.PlateNumber ?? "-"),
                ("业务类型", GetBusinessTypeText(record.BusinessType)),
                ("货物类型", record.CargoType ?? "-"),
                ("发货单位", record.Sender ?? "-"),
                ("收货单位", record.Receiver ?? "-"),
                ("司机姓名", record.DriverName ?? "-"),
                ("联系电话", record.DriverPhone ?? "-"),
                ("毛重", record.GrossWeight.ToString("F3") + " 吨"),
                ("皮重", record.TareWeight.ToString("F3") + " 吨"),
                ("净重", record.NetWeight.ToString("F3") + " 吨"),
                ("第一次称重", record.FirstWeighTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-"),
                ("第二次称重", record.SecondWeighTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-"),
                ("备注", record.Remark ?? "-")
            };
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
    /// 打印模板配置
    /// </summary>
    public class PrintTemplate
    {
        public string Title { get; set; }
        public float PageWidth { get; set; } = 210; // mm
        public float PageHeight { get; set; } = 297; // mm
        public float MarginLeft { get; set; } = 20;
        public float MarginRight { get; set; } = 20;
        public float MarginTop { get; set; } = 20;
        public float MarginBottom { get; set; } = 20;
        public float RowHeight { get; set; } = 8;
        
        /// <summary>
        /// 标准模板
        /// </summary>
        public static PrintTemplate Standard => new PrintTemplate
        {
            Title = "称重单",
            PageWidth = 210,
            PageHeight = 140, // 小票尺寸
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
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public PrintTemplate()
        {
            Title = "称重单";
        }
    }
}
