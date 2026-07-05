using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using pism_weigh.Interfaces;

namespace pism_weigh.Services
{
    /// <summary>
    /// 车牌识别引擎 — 基于图像分析的实时识别
    /// 
    /// 实现思路:
    ///   1. 检测图像中蓝色区域(中国车牌底色特征)
    ///   2. 判断画面中是否有疑似车牌对象
    ///   3. 有则尝试基础文字区域检测,无则返回null
    ///   
    /// 生产环境可替换为: Emgu.CV + Tesseract / 海康ANPR SDK
    /// </summary>
    public class PlateRecognizer : ILPRService
    {
        private bool _useAnpr;
        private int _frameCount;

        public event Action<string> PlateRecognized;
        public bool IsAvailable { get { return true; } }
        public string EngineName { get { return _useAnpr ? "海康ANPR" : "图像检测"; } }

        /// <summary>最近检测到的稳定车牌号(连续多帧确认)</summary>
        public string LastStablePlate { get; private set; }

        /// <summary>画面中是否检测到疑似车辆</summary>
        public bool VehicleDetected { get; private set; }

        /// <summary>蓝区像素占比(诊断用)</summary>
        public double BluePixelRatio { get; private set; }

        public PlateRecognizer(bool useAnpr = false)
        {
            _useAnpr = useAnpr;
            _frameCount = 0;
        }

        /// <summary>
        /// 从摄像头拍摄的真实画面中识别车牌号
        /// 不做模拟数据,没检测到则返回null
        /// </summary>
        public string Recognize(Bitmap image)
        {
            if (image == null) return null;

            _frameCount++;

            // --- 步骤1: 检测蓝色区域(中国车牌特征) ---
            var blueAnalysis = DetectBlueRegion(image);
            BluePixelRatio = blueAnalysis;
            VehicleDetected = blueAnalysis > 0.001; // 超过0.1%蓝色像素

            // --- 步骤2: 基于真实画面内容分析 ---
            string detectedPlate = null;

            if (VehicleDetected)
            {
                // 尝试从蓝色区域中提取可能的文本特征
                detectedPlate = ExtractPlateText(image);

                // 如果提取到了可靠的文本
                if (!string.IsNullOrWhiteSpace(detectedPlate) && IsValidPlate(detectedPlate))
                {
                    LastStablePlate = detectedPlate;
                    PlateRecognized?.Invoke(detectedPlate);
                    return detectedPlate;
                }
            }

            // --- 步骤3: 返回null表示未识别到(不生成随机假数据) ---
            // 注意: 此处不产生模拟数据,调用方应根据null做相关处理
            // 
            // 完整OCR实现参考:
            //   - Emgu.CV + Tesseract 4.x (LSTM)
            //   - OpenCvSharp4 + EasyOCR
            //   - 海康SDK NET_DVR_SetAlarmCallback + ANPR事件

            return null;
        }

        /// <summary>
        /// 检测图像中的蓝色像素占比(中国车牌底色)
        /// </summary>
        private double DetectBlueRegion(Bitmap image)
        {
            try
            {
                // 缩放到160x120加速分析
                using (var thumb = new Bitmap(image, 160, 120))
                {
                    var data = thumb.LockBits(
                        new Rectangle(0, 0, thumb.Width, thumb.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    int bluePixels = 0;
                    int totalPixels = thumb.Width * thumb.Height;
                    int stride = data.Stride;

                    unsafe
                    {
                        byte* ptr = (byte*)data.Scan0;
                        for (int y = 0; y < thumb.Height; y++)
                        {
                            for (int x = 0; x < thumb.Width; x++)
                            {
                                int offset = y * stride + x * 3;
                                byte B = ptr[offset];
                                byte G = ptr[offset + 1];
                                byte R = ptr[offset + 2];

                                // 检测蓝底色: B > 120, B > R+30, B > G+20
                                if (B > 120 && B > R + 30 && B > G + 20)
                                    bluePixels++;
                            }
                        }
                    }

                    thumb.UnlockBits(data);
                    return (double)bluePixels / totalPixels;
                }
            }
            catch { return 0; }
        }

        /// <summary>
        /// 从蓝色区域附近尝试提取文本特征
        /// (简化实现 — 生产环境替换为Tesseract OCR)
        /// </summary>
        private string ExtractPlateText(Bitmap image)
        {
            try
            {
                // 缩放到工作尺寸
                using (var thumb = new Bitmap(image, 320, 240))
                {
                    var data = thumb.LockBits(
                        new Rectangle(0, 0, thumb.Width, thumb.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    int stride = data.Stride;
                    int blueClipLeft = thumb.Width, blueClipRight = 0;
                    int blueClipTop = thumb.Height, blueClipBottom = 0;
                    int blueCount = 0;

                    unsafe
                    {
                        byte* ptr = (byte*)data.Scan0;
                        for (int y = 0; y < thumb.Height; y++)
                        {
                            for (int x = 0; x < thumb.Width; x++)
                            {
                                int offset = y * stride + x * 3;
                                byte B = ptr[offset];
                                byte G = ptr[offset + 1];
                                byte R = ptr[offset + 2];

                                if (B > 120 && B > R + 30 && B > G + 20)
                                {
                                    blueCount++;
                                    if (x < blueClipLeft) blueClipLeft = x;
                                    if (x > blueClipRight) blueClipRight = x;
                                    if (y < blueClipTop) blueClipTop = y;
                                    if (y > blueClipBottom) blueClipBottom = y;
                                }
                            }
                        }
                    }
                    thumb.UnlockBits(data);

                    // 蓝色区域太少,不构成车牌
                    if (blueCount < 200) return null;

                    // 蓝色区域宽高比检查(中国车牌约 440:140 ≈ 3.14)
                    int bw = blueClipRight - blueClipLeft;
                    int bh = blueClipBottom - blueClipTop;
                    if (bw < 40 || bh < 10 || bw < bh) return null;

                    return null; // 基础图像检测通过但OCR不可用,返回null
                }
            }
            catch { return null; }
        }

        /// <summary>验证车牌号格式</summary>
        private static bool IsValidPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate)) return false;
            return Regex.IsMatch(plate,
                @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤川青藏琼宁][A-Z][A-HJ-NP-Z0-9]{5}$");
        }

        public void SetMode(bool useAnpr)
        {
            _useAnpr = useAnpr;
        }
    }
}
