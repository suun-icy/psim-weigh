using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace pism_weigh.Services
{
    /// <summary>
    /// 打印机原生 API — 通过修改 DEVMODE 底层结构精准控制纸张尺寸
    /// 解决 .NET PaperSize 对很多打印机无效、始终回退 A4 的问题
    /// </summary>
    public static class PrinterNative
    {
        // ===== DEVMODE 常量 =====
        private const short DMPAPER_USER = 256;

        private const int DM_PAPERSIZE   = 0x0002;
        private const int DM_PAPERWIDTH  = 0x0008;
        private const int DM_PAPERLENGTH = 0x0004;
        private const int DM_ORIENTATION = 0x0001;
        private const int DMORIENT_PORTRAIT = 1;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMem);

        /// <summary>
        /// 通过直接修改 DEVMODE 设置自定义纸张大小
        /// 240mm × 93.1mm → dmPaperWidth=2400, dmPaperLength=931 (十分之一毫米)
        /// </summary>
        public static bool SetCustomPaper240x93(PrinterSettings settings)
        {
            return SetCustomPaper(settings, 2400, 931);
        }

        /// <summary>
        /// 通过直接修改 DEVMODE 设置自定义纸张大小
        /// </summary>
        /// <param name="settings">打印机设置</param>
        /// <param name="widthTenthsMm">宽（十分之一毫米）</param>
        /// <param name="heightTenthsMm">高（十分之一毫米）</param>
        public static bool SetCustomPaper(PrinterSettings settings, int widthTenthsMm, int heightTenthsMm)
        {
            try
            {
                IntPtr hDevMode = settings.GetHdevmode();
                if (hDevMode == IntPtr.Zero) return false;

                IntPtr pDevMode = GlobalLock(hDevMode);
                if (pDevMode == IntPtr.Zero) return false;

                try
                {
                    // 先假设 ANSI（dmDeviceName = 32 字节）
                    int devNameLen = 32;
                    short dmSize = Marshal.ReadInt16(pDevMode, devNameLen + 4);
                    short dmSpec = Marshal.ReadInt16(pDevMode, devNameLen);
                    short dmDriver = Marshal.ReadInt16(pDevMode, devNameLen + 2);

                    // dmSize 对当前 DEVMODE 应该在合理范围（标准约 120~800 字节）
                    // 如果明显不对，尝试 Unicode 版本（dmDeviceName = 64 字节）
                    if (dmSize < 100 || dmSize > 1000 || dmSpec < 0 || dmSpec > 10)
                    {
                        devNameLen = 64; // Unicode (WCHAR * 32)
                        dmSize = Marshal.ReadInt16(pDevMode, devNameLen + 4);
                        if (dmSize < 100 || dmSize > 1000)
                            return false; // 无法识别 DEVMODE 格式
                    }

                    int fieldsOff = devNameLen + 8;  // dmFields 偏移
                    int orientOff = fieldsOff + 4;    // dmOrientation 偏移
                    int paperOff  = orientOff + 2;    // dmPaperSize 偏移
                    int lengthOff = paperOff  + 2;    // dmPaperLength 偏移
                    int widthOff  = lengthOff + 2;    // dmPaperWidth 偏移

                    // 设置 dmFields 标志位
                    int dmFields = Marshal.ReadInt32(pDevMode, fieldsOff);
                    dmFields |= DM_PAPERSIZE | DM_PAPERWIDTH | DM_PAPERLENGTH | DM_ORIENTATION;
                    Marshal.WriteInt32(pDevMode, fieldsOff, dmFields);

                    // 竖向
                    Marshal.WriteInt16(pDevMode, orientOff, DMORIENT_PORTRAIT);

                    // 自定义纸张标识
                    Marshal.WriteInt16(pDevMode, paperOff, DMPAPER_USER);

                    // 纸张长度 / 宽度（十分之一毫米）
                    Marshal.WriteInt16(pDevMode, lengthOff, (short)heightTenthsMm);
                    Marshal.WriteInt16(pDevMode, widthOff, (short)widthTenthsMm);
                }
                finally
                {
                    GlobalUnlock(hDevMode);
                }

                settings.SetHdevmode(hDevMode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试在打印机 PaperSizes 集合中查找匹配尺寸
        /// </summary>
        public static PaperSize FindMatchingPaperSize(PrinterSettings settings, int width, int height)
        {
            try
            {
                foreach (PaperSize ps in settings.PaperSizes)
                {
                    if (ps.Width == width && ps.Height == height)
                        return ps;
                }
            }
            catch { }
            return null;
        }
    }
}
