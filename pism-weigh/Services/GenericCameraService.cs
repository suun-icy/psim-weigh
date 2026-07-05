using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using pism_weigh.Interfaces;
using pism_weigh.Models;
using Timer = System.Timers.Timer;

namespace pism_weigh.Services
{
    /// <summary>
    /// 通用摄像头服务 — 支持 HTTP/MJPEG 摄像头 (如 IP WebCam) / RTSP / USB
    ///
    /// 连接策略（按优先级尝试）:
    ///   1. HTTP Snapshot 轮询 (兼容 IP WebCam / 大多数网络摄像头)
    ///   2. 自定义 RTSP URL
    ///   3. AForge.NET USB 摄像头 (需安装 NuGet 包)
    ///   4. 模拟模式 (无硬件时开发调试用)
    /// </summary>
    public class GenericCameraService : ICameraService, IDisposable
    {
        private CameraConfig _config;
        private Timer _pollTimer;
        private Random _rnd = new Random();
        private string _activeSnapshotUrl;
        private string _activeMjpegUrl;
        private bool _isRealCamera;

        public event Action<Bitmap> FrameCaptured;
        public bool IsConnected { get; private set; }
        public string ServiceName { get { return "通用摄像头"; } }

        /// <summary>是否使用真实摄像头（非模拟）</summary>
        public bool IsRealCamera { get { return _isRealCamera; } }

        public bool Connect(CameraConfig config)
        {
            _config = config;
            _isRealCamera = false;

            try
            {
                // === 策略1: 构建HTTP Snapshot URL并探测 ===
                var snapshotCandidates = BuildSnapshotUrls();
                foreach (var url in snapshotCandidates)
                {
                    if (TrySnapshotUrl(url))
                    {
                        _activeSnapshotUrl = url;
                        _isRealCamera = true;
                        _pollTimer = new Timer(400);   // 250ms 间隔 ≈ 4fps
                        _pollTimer.Elapsed += PollSnapshot;
                        _pollTimer.Start();
                        IsConnected = true;
                        return true;
                    }
                }

                // === 策略2: 尝试 MJPEG 流 ===
                var mjpegCandidates = BuildMjpegUrls();
                foreach (var url in mjpegCandidates)
                {
                    if (TryMjpegUrl(url))
                    {
                        _activeMjpegUrl = url;
                        _isRealCamera = true;
                        _pollTimer = new Timer(50);    // ~20fps
                        _pollTimer.Elapsed += PollMjpeg;
                        _pollTimer.Start();
                        IsConnected = true;
                        return true;
                    }
                }

                // === 策略3: 用户指定了 RTSP URL，直接使用 ===
                if (!string.IsNullOrWhiteSpace(_config?.RTSPUrl))
                {
                    // RTSP 需要第三方库(AForge/VLC)，暂时用HTTP快照替代
                    // 尝试基于RTSP IP构建HTTP URL
                    _activeSnapshotUrl = BuildHttpFromRtsp(_config.RTSPUrl);
                    if (_activeSnapshotUrl != null && TrySnapshotUrl(_activeSnapshotUrl))
                    {
                        _isRealCamera = true;
                        _pollTimer = new Timer(400);
                        _pollTimer.Elapsed += PollSnapshot;
                        _pollTimer.Start();
                        IsConnected = true;
                        return true;
                    }
                }

                // === 策略4: 回退到模拟模式 ===
                _isRealCamera = false;
                _pollTimer = new Timer(500);
                _pollTimer.Elapsed += (s, e) =>
                {
                    var bmp = GenerateMockFrame();
                    FrameCaptured?.Invoke(bmp);
                };
                _pollTimer.Start();
                IsConnected = true;
                return true;
            }
            catch
            {
                // 最终回退
                _isRealCamera = false;
                _pollTimer = new Timer(500);
                _pollTimer.Elapsed += (s, e) =>
                {
                    var bmp = GenerateMockFrame();
                    FrameCaptured?.Invoke(bmp);
                };
                _pollTimer.Start();
                IsConnected = true;
                return true;
            }
        }

        public void Disconnect()
        {
            _pollTimer?.Stop();
            _pollTimer?.Dispose();
            _pollTimer = null;
            IsConnected = false;
            _isRealCamera = false;
        }

        public Bitmap CaptureSnapshot()
        {
            if (_isRealCamera && !string.IsNullOrEmpty(_activeSnapshotUrl))
            {
                try
                {
                    var data = DownloadWithAuth(_activeSnapshotUrl);
                    if (data != null && data.Length > 500)
                    {
                        using (var ms = new MemoryStream(data))
                            return new Bitmap(ms);
                    }
                }
                catch { }
            }
            return GenerateMockFrame();
        }

        // ===== URL 构建 =====

        /// <summary>构建 Snapshot URL 候选列表</summary>
        private List<string> BuildSnapshotUrls()
        {
            var list = new List<string>();
            var ip = _config?.IPAddress;
            if (string.IsNullOrWhiteSpace(ip)) return list;

            var port = _config.Port > 0 ? _config.Port : 8080;
            var baseUrl = string.Format("http://{0}:{1}", ip, port);

            // IP WebCam (Android) 常见端点
            list.Add(baseUrl + "/shot.jpg");
            list.Add(baseUrl + "/photo.jpg");
            list.Add(baseUrl + "/photoaf.jpg");

            // 通用 ONVIF/网络摄像头端点
            list.Add(baseUrl + "/snapshot.jpg");
            list.Add(baseUrl + "/snapshot.cgi");
            list.Add(baseUrl + "/cgi-bin/snapshot.cgi");
            list.Add(baseUrl + "/jpg/image.jpg");
            list.Add(baseUrl + "/cgi-bin/viewer/video.jpg");
            list.Add(baseUrl + "/ISAPI/Streaming/channels/" + _config.ChannelNo + "/picture");

            // 如果用户填写了RTSP，也通过RTSP构建
            if (!string.IsNullOrWhiteSpace(_config.RTSPUrl))
                list.Add(BuildHttpFromRtsp(_config.RTSPUrl));

            return list;
        }

        private List<string> BuildMjpegUrls()
        {
            var list = new List<string>();
            var ip = _config?.IPAddress;
            if (string.IsNullOrWhiteSpace(ip)) return list;

            var port = _config.Port > 0 ? _config.Port : 8080;
            var baseUrl = string.Format("http://{0}:{1}", ip, port);

            // IP WebCam MJPEG
            list.Add(baseUrl + "/video");

            // 通用 MJPEG 端点
            list.Add(baseUrl + "/mjpg/video.mjpg");
            list.Add(baseUrl + "/stream/video.mjpeg");
            list.Add(baseUrl + "/cgi-bin/mjpg/video.cgi");

            return list;
        }

        private string BuildHttpFromRtsp(string rtspUrl)
        {
            try
            {
                // rtsp://192.168.1.100:554/xxx → http://192.168.1.100:8080/shot.jpg
                var uri = new Uri(rtspUrl);
                return string.Format("http://{0}:8080/shot.jpg", uri.Host);
            }
            catch { return null; }
        }

        // ===== HTTP 探测 =====

        private bool TrySnapshotUrl(string url)
        {
            try
            {
                var data = DownloadWithAuth(url, 3000);
                if (data != null && data.Length > 500)
                {
                    // 验证是否为有效图片
                    using (var ms = new MemoryStream(data))
                    {
                        using (var img = Image.FromStream(ms, false, false))
                        {
                            return img.Width > 0 && img.Height > 0;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private bool TryMjpegUrl(string url)
        {
            try
            {
                var request = CreateRequest(url, 3000);
                request.Timeout = 3000;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var ct = response.ContentType ?? "";
                        return ct.Contains("multipart") || ct.Contains("video") || ct.Contains("mjpeg");
                    }
                }
            }
            catch { }
            return false;
        }

        private byte[] DownloadWithAuth(string url, int timeout = 5000)
        {
            var request = CreateRequest(url, timeout);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return null;
                using (var stream = response.GetResponseStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }

        private HttpWebRequest CreateRequest(string url, int timeout)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;

            // Basic Auth
            if (!string.IsNullOrEmpty(_config?.Username))
            {
                var cred = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes((_config.Username ?? "admin") + ":" + (_config.Password ?? "")));
                request.Headers[HttpRequestHeader.Authorization] = "Basic " + cred;
            }

            // 尝试 Digest Auth（某些摄像头需要）
            request.Credentials = new NetworkCredential(
                _config?.Username ?? "", _config?.Password ?? "");

            return request;
        }

        // ===== 帧轮询 =====

        private void PollSnapshot(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(_activeSnapshotUrl)) return;
            try
            {
                var data = DownloadWithAuth(_activeSnapshotUrl, 2000);
                if (data != null && data.Length > 500)
                {
                    using (var ms = new MemoryStream(data))
                    {
                        var bmp = new Bitmap(ms);
                        FrameCaptured?.Invoke(bmp);
                    }
                }
            }
            catch { /* 单帧失败不中断流 */ }
        }

        private byte[] _mjpegBuffer = new byte[1024 * 1024];
        private void PollMjpeg(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(_activeMjpegUrl)) return;
            try
            {
                // MJPEG 通过 HTTP multipart 推送，此处简化为HTTP快照轮询
                // 完整MJPEG解析需要持续HTTP连接+mjpeg-stream解析
                var data = DownloadWithAuth(_activeMjpegUrl.Replace("/video", "/shot.jpg"), 2000);
                if (data == null)
                    data = DownloadWithAuth(_activeMjpegUrl, 2000);

                if (data != null && data.Length > 500)
                {
                    using (var ms = new MemoryStream(data))
                    {
                        var bmp = new Bitmap(ms);
                        FrameCaptured?.Invoke(bmp);
                    }
                }
            }
            catch { }
        }

        // ===== 模拟画面 =====

        private Bitmap GenerateMockFrame()
        {
            var bmp = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(50, 50, 50));
                g.FillRectangle(Brushes.DarkGreen, 0, 300, 640, 180);
                g.FillRectangle(Brushes.Gray, 0, 320, 640, 160);

                var provinces = new[] { "豫", "京", "沪", "粤", "苏", "浙", "鲁", "川" };
                var letters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
                var province = provinces[_rnd.Next(provinces.Length)];
                var letter = letters[_rnd.Next(letters.Length)];
                var digits = string.Format("{0:D5}", _rnd.Next(99999));
                var plate = province + letter + digits;

                var plateX = 170 + _rnd.Next(60);
                var plateY = 150 + _rnd.Next(40);
                g.FillRectangle(Brushes.DarkBlue, plateX, plateY, 300, 80);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 80, 160)), plateX + 3, plateY + 3, 294, 74);
                g.DrawString(plate, new Font("KaiTi", 36, FontStyle.Bold), Brushes.White, plateX + 15, plateY + 15);

                g.DrawString(DateTime.Now.ToString("HH:mm:ss.fff"), new Font("Consolas", 12),
                    Brushes.LightGreen, 500, 10);
                g.DrawString("Camera: " + (_config?.Name ?? "Generic"), new Font("Microsoft YaHei UI", 10),
                    Brushes.White, 10, 10);
                g.DrawString("Mode: Simulation (未检测到真实摄像头)", new Font("Microsoft YaHei UI", 9),
                    Brushes.Orange, 10, 28);
            }
            return bmp;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
