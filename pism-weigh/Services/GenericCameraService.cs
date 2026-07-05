using System;
using System.Drawing;
using System.Timers;
using pism_weigh.Interfaces;
using pism_weigh.Models;
using Timer = System.Timers.Timer;

namespace pism_weigh.Services
{
    /// <summary>
    /// 通用摄像头服务 — 支持 USB 摄像头 (AForge.NET) 和 RTSP 流
    /// 实现 ICameraService 接口
    /// </summary>
    public class GenericCameraService : ICameraService, IDisposable
    {
        private CameraConfig _config;
        private Timer _simTimer;
        private Random _rnd = new Random();

        public event Action<Bitmap> FrameCaptured;
        public bool IsConnected { get; private set; }
        public string ServiceName { get { return "通用摄像头"; } }

        public bool Connect(CameraConfig config)
        {
            _config = config;
            try
            {
                // 尝试连接 AForge USB 摄像头或 RTSP 流
                // 因 AForge.NET 需单独 NuGet 安装，此处使用模拟模式
                // 实际生产环境中取消注释下方 AForge 实现

                // --- AForge 实现（需安装 AForge.Video / AForge.Video.DirectShow）---
                // var videoDevices = new AForge.Video.DirectShow.FilterInfoCollection(
                //     AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
                // if (videoDevices.Count > 0)
                // {
                //     _videoSource = new AForge.Video.DirectShow.VideoCaptureDevice(
                //         videoDevices[0].MonikerString);
                //     _videoSource.NewFrame += (s, e) => FrameCaptured?.Invoke((Bitmap)e.Frame.Clone());
                //     _videoSource.Start();
                // }

                // 模拟帧生成（每 500ms 一张测试帧）
                _simTimer = new Timer(500);
                _simTimer.Elapsed += (s, e) =>
                {
                    var bmp = GenerateMockFrame();
                    FrameCaptured?.Invoke(bmp);
                };
                _simTimer.Start();
                IsConnected = true;
                return true;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        public void Disconnect()
        {
            _simTimer?.Stop();
            _simTimer?.Dispose();
            _simTimer = null;
            IsConnected = false;
        }

        public Bitmap CaptureSnapshot()
        {
            return GenerateMockFrame();
        }

        /// <summary>生成模拟画面（含随机"车牌"文字）</summary>
        private Bitmap GenerateMockFrame()
        {
            var bmp = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(50, 50, 50));

                // 模拟摄像头画面背景
                g.FillRectangle(Brushes.DarkGreen, 0, 300, 640, 180);
                g.FillRectangle(Brushes.Gray, 0, 320, 640, 160);

                // 随机"车牌"
                var provinces = new[] { "豫", "京", "沪", "粤", "苏", "浙", "鲁", "川" };
                var letters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
                var province = provinces[_rnd.Next(provinces.Length)];
                var letter = letters[_rnd.Next(letters.Length)];
                var digits = string.Format("{0:D5}", _rnd.Next(99999));
                var plate = province + letter + digits;

                // 绘制模拟车牌框
                var plateX = 170 + _rnd.Next(60);
                var plateY = 150 + _rnd.Next(40);
                g.FillRectangle(Brushes.DarkBlue, plateX, plateY, 300, 80);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 80, 160)), plateX + 3, plateY + 3, 294, 74);
                g.DrawString(plate, new Font("KaiTi", 36, FontStyle.Bold), Brushes.White, plateX + 15, plateY + 15);

                // 时间戳
                g.DrawString(DateTime.Now.ToString("HH:mm:ss.fff"), new Font("Consolas", 12),
                    Brushes.LightGreen, 500, 10);

                // 帧信息
                g.DrawString("Camera: " + (_config?.Name ?? "Generic"), new Font("Microsoft YaHei UI", 10),
                    Brushes.White, 10, 10);
                g.DrawString("Mode: Simulation", new Font("Microsoft YaHei UI", 9),
                    Brushes.Gray, 10, 28);
            }
            return bmp;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
