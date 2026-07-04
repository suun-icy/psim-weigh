using System;
using System.IO;
using Newtonsoft.Json;

namespace pism_weigh.Models
{
    /// <summary>
    /// 应用配置持久化
    /// </summary>
    public class AppConfig
    {
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "app_config.json");

        // 串口配置
        public string ComPort { get; set; } = "";
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "1";

        // 其他配置
        public string ServerUrl { get; set; } = "";
        public string PrinterName { get; set; } = "";
        public bool AutoConnect { get; set; } = false;

        /// <summary>
        /// 加载配置
        /// </summary>
        public static AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var config = JsonConvert.DeserializeObject<AppConfig>(json);
                    return config ?? new AppConfig();
                }
            }
            catch
            {
                // 读取失败返回默认配置
            }
            return new AppConfig();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // 静默处理保存失败
            }
        }
    }
}
