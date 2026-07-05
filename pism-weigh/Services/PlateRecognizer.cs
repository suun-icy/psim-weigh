using System;
using System.Drawing;
using System.Text.RegularExpressions;
using pism_weigh.Interfaces;

namespace pism_weigh.Services
{
    /// <summary>
    /// 车牌识别引擎 — 支持 OCR 和 ANPR 两种模式
    /// </summary>
    public class PlateRecognizer : ILPRService
    {
        private readonly Random _rnd = new Random();
        private bool _useAnpr;

        public event Action<string> PlateRecognized;
        public bool IsAvailable { get { return true; } }
        public string EngineName { get { return _useAnpr ? "海康ANPR" : "OCR识别"; } }

        public PlateRecognizer(bool useAnpr = false)
        {
            _useAnpr = useAnpr;
        }

        /// <summary>
        /// 从图片中识别车牌号
        /// 实际生产环境中替换为 OpenCvSharp + Tesseract 或海康 ANPR SDK
        /// </summary>
        public string Recognize(Bitmap image)
        {
            if (image == null) return null;

            // --- 真实 OCR 实现（需安装 Emgu.CV + Tesseract）---
            // using (var ocr = new TesseractEngine("./tessdata", "chi_sim", EngineMode.Default))
            // {
            //     ocr.SetVariable("tessedit_char_whitelist", "京津沪渝...ABCDEFGHJKLMNPQRSTUVWXYZ0123456789");
            //     using (var page = ocr.Process(image, PageSegMode.SingleLine))
            //         return page.GetText().Trim().Replace(" ", "");
            // }

            // --- 海康 ANPR 模式 ---
            // if (_useAnpr)
            // {
            //     var anprData = HikAnprSdk.Recognize(image);
            //     return anprData?.PlateNumber;
            // }

            // 模拟识别（从图像中提取随机车牌）
            var provinces = new[] { "豫", "京", "沪", "粤", "苏", "浙", "鲁", "川", "鄂", "湘",
                                    "冀", "晋", "辽", "吉", "黑", "皖", "闽", "赣", "豫", "桂",
                                    "琼", "渝", "贵", "云", "藏", "陕", "甘", "青", "宁", "新" };
            var result = provinces[_rnd.Next(provinces.Length)] +
                        "ABCDEFGHJKLMNPQRSTUVWXYZ"[_rnd.Next(22)] +
                        string.Format("{0:D5}", _rnd.Next(99999));

            // 过滤明显不合理的车牌格式
            if (!Regex.IsMatch(result, @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤川青藏琼宁][A-Z][A-HJ-NP-Z0-9]{5}$"))
                return null;

            PlateRecognized?.Invoke(result);
            return result;
        }

        /// <summary>切换识别引擎模式</summary>
        public void SetMode(bool useAnpr)
        {
            _useAnpr = useAnpr;
        }
    }
}
