using System;

namespace pism_weigh.Models
{
    /// <summary>
    /// 摄像头配置实体
    /// </summary>
    public class CameraConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CameraType { get; set; }     // Hikvision / Generic / USB
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ChannelNo { get; set; }
        public string RTSPUrl { get; set; }
        public string Resolution { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public CameraConfig()
        {
            Id = Guid.NewGuid().ToString("N");
            Port = 8000;
            ChannelNo = 1;
            Resolution = "1920x1080";
            IsEnabled = true;
            IsDefault = false;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }
    }
}
