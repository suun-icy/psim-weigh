using System;
using System.Drawing;
using System.Timers;
using pism_weigh.Interfaces;
using pism_weigh.Models;
using Timer = System.Timers.Timer;

namespace pism_weigh.Services
{
    /// <summary>
    /// 海康威视摄像头服务 — 对接 HCNetSDK
    /// 支持 SDK 模式（预览+ANPR报警回调）和 ISAPI 模式（HTTP抓图）
    /// </summary>
    public class HikvisionCameraService : ICameraService, IDisposable
    {
        private CameraConfig _config;
        private Timer _pollTimer;
        private Random _rnd = new Random();
        private int _userId = -1;
        private int _previewHandle = -1;

        public event Action<Bitmap> FrameCaptured;
        public bool IsConnected { get; private set; }
        public string ServiceName { get { return "海康威视"; } }

        /// <summary>ANPR 识别结果回调（结构化摄像头专用）</summary>
        public event Action<string> AnprResult;

        public bool Connect(CameraConfig config)
        {
            _config = config;
            try
            {
                // --- 海康 SDK 实现（需安装 HCNetSDK.dll）---
                // NET_DVR_DEVICEINFO_V30 devInfo = new NET_DVR_DEVICEINFO_V30();
                // _userId = NET_DVR_Login_V30(config.IPAddress, (short)config.Port,
                //     config.Username, config.Password, ref devInfo);
                // if (_userId < 0) return false;
                // NET_DVR_CLIENTINFO clientInfo = new NET_DVR_CLIENTINFO();
                // _previewHandle = NET_DVR_RealPlay_V30(_userId, ref clientInfo, null, IntPtr.Zero, true);

                // 模拟模式
                _pollTimer = new Timer(600);
                _pollTimer.Elapsed += (s, e) =>
                {
                    var bmp = GenerateHikvisionFrame();
                    FrameCaptured?.Invoke(bmp);
                };
                _pollTimer.Start();
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
            _pollTimer?.Stop();
            _pollTimer?.Dispose();
            _pollTimer = null;
            IsConnected = false;
        }

        public Bitmap CaptureSnapshot()
        {
            // --- ISAPI 抓图 ---
            // var url = string.Format("http://{0}:{1}/ISAPI/Streaming/channels/{2}/picture",
            //     _config.IPAddress, _config.Port, _config.ChannelNo);
            // using (var client = new System.Net.WebClient())
            // {
            //     client.Credentials = new System.Net.NetworkCredential(_config.Username, _config.Password);
            //     var data = client.DownloadData(url);
            //     using (var ms = new System.IO.MemoryStream(data))
            //         return new Bitmap(ms);
            // }
            return GenerateHikvisionFrame();
        }

        /// <summary>手动触发一次 ANPR 识别（模拟）</summary>
        public string TriggerAnpr()
        {
            var provinces = new[] { "豫", "京", "沪", "粤", "苏" };
            var plate = provinces[_rnd.Next(provinces.Length)] +
                       "ACDEFGHJKLMNPQRSTUVWXYZ"[_rnd.Next(22)] +
                       string.Format("{0:D5}", _rnd.Next(99999));
            AnprResult?.Invoke(plate);
            return plate;
        }

        private Bitmap GenerateHikvisionFrame()
        {
            var bmp = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(20, 20, 30));

                // 海康 OSD 模拟
                g.FillRectangle(Brushes.DarkGreen, 0, 380, 640, 100);
                g.DrawString("CAM " + (_config?.ChannelNo ?? 1), new Font("Consolas", 10, FontStyle.Bold),
                    Brushes.White, 10, 385);
                g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), new Font("Consolas", 10),
                    Brushes.White, 460, 385);

                // 模拟车辆
                var plate = TriggerAnpr();
                g.FillRectangle(Brushes.DarkBlue, 200, 160, 240, 65);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 70, 150)), 202, 162, 236, 61);
                g.DrawString(plate, new Font("KaiTi", 30, FontStyle.Bold), Brushes.White, 212, 172);

                g.DrawString("HIKVISION | " + (_config?.IPAddress ?? "N/A") + " | ANPR READY",
                    new Font("Microsoft YaHei UI", 9), Brushes.LimeGreen, 10, 455);
            }
            return bmp;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
