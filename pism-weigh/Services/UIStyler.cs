using System.Drawing;
using System.Windows.Forms;

namespace pism_weigh.Services
{
    /// <summary>
    /// 全局UI样式工具 — 统一配色、控件样式、DataGridView美化
    /// </summary>
    public static class UIStyler
    {
        // 配色
        public static Color PrimaryBlue   = Color.FromArgb(24, 144, 255);    // #1890FF
        public static Color SuccessGreen  = Color.FromArgb(82, 196, 26);     // #52C41A
        public static Color WarningOrange = Color.FromArgb(250, 173, 20);    // #FAAD14
        public static Color DangerRed     = Color.FromArgb(245, 34, 45);     // #F5222D
        public static Color BgGray        = Color.FromArgb(240, 242, 245);   // #F0F2F5
        public static Color PanelBg       = Color.White;
        public static Color TextPrimary   = Color.FromArgb(38, 38, 38);
        public static Color TextSecondary = Color.FromArgb(140, 140, 140);
        public static Color BorderColor   = Color.FromArgb(217, 217, 217);

        /// <summary>应用到Form的基础样式</summary>
        public static void StyleForm(Form form, string title = null)
        {
            form.Font = new Font("Microsoft YaHei UI", 9F);
            form.BackColor = BgGray;
            if (!string.IsNullOrEmpty(title)) form.Text = title;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>美化DataGridView — 交替行色+蓝色表头+无网格线</summary>
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(240, 240, 240);
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.ScrollBars = ScrollBars.Both;

            // 禁止单元格内容换行
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // 表头样式
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 32;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 行样式
            dgv.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 9F);
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 247, 255);
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            dgv.RowTemplate.Height = 28;

            // 交替行色
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
        }

        /// <summary>美化按钮 — Flat风格+悬停效果</summary>
        public static void StyleButton(Button btn, Color? backColor = null, Color? foreColor = null)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor ?? PrimaryBlue;
            btn.ForeColor = foreColor ?? Color.White;
            btn.Font = new Font("Microsoft YaHei UI", 9F);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => { btn.BackColor = Darken(btn.BackColor, 0.85f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = backColor ?? PrimaryBlue; };

            // 禁用状态显著标记
            btn.EnabledChanged += (s, e) =>
            {
                if (!btn.Enabled)
                {
                    btn.BackColor = Color.FromArgb(220, 220, 220);
                    btn.ForeColor = Color.FromArgb(160, 160, 160);
                    btn.Cursor = Cursors.Default;
                }
                else
                {
                    btn.BackColor = backColor ?? PrimaryBlue;
                    btn.ForeColor = foreColor ?? Color.White;
                    btn.Cursor = Cursors.Hand;
                }
            };
        }

        /// <summary>美化Panel — 白色卡片式</summary>
        public static void StylePanel(Panel panel, int padding = 8)
        {
            panel.BackColor = PanelBg;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(padding);
        }

        /// <summary>美化TextBox</summary>
        public static void StyleTextBox(TextBox tb, int fontSize = 9)
        {
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font = new Font("Microsoft YaHei UI", fontSize);
            tb.BackColor = Color.White;
        }

        /// <summary>美化ComboBox</summary>
        public static void StyleComboBox(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.Font = new Font("Microsoft YaHei UI", 9F);
            cb.BackColor = Color.White;
        }

        private static Color Darken(Color color, float factor)
        {
            return Color.FromArgb(
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor));
        }
    }
}
