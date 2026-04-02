using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pism_weigh.Services
{
    /// <summary>
    /// 地磅仪表串口服务
    /// </summary>
    public class ScaleService : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isConnected;
        private string _currentPort;
        private int _baudRate;
        
        // 重量数据回调
        public event Action<decimal> WeightReceived;
        
        // 连接状态变化回调
        public event Action<bool> ConnectionStateChanged;
        
        // 错误信息回调
        public event Action<string> ErrorOccurred;
        
        /// <summary>
        /// 当前重量值（吨）
        /// </summary>
        public decimal CurrentWeight { get; private set; }
        
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected => _isConnected;
        
        /// <summary>
        /// 可用的串口号列表
        /// </summary>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
        
        /// <summary>
        /// 连接串口
        /// </summary>
        public bool Connect(string portName, int baudRate = 9600, int dataBits = 8, 
                           Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }
                
                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    DtrEnable = true,
                    RtsEnable = true
                };
                
                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();
                
                _currentPort = portName;
                _baudRate = baudRate;
                _isConnected = true;
                
                ConnectionStateChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"连接串口失败：{ex.Message}");
                _isConnected = false;
                return false;
            }
        }
        
        /// <summary>
        /// 断开串口连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Dispose();
                    _serialPort = null;
                }
                
                _isConnected = false;
                ConnectionStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"断开串口失败：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 串口数据接收处理
        /// </summary>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(100); // 等待数据接收完整
                
                if (!_serialPort.IsOpen) return;
                
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead == 0) return;
                
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                
                // 解析重量数据
                string weightStr = ParseWeightData(buffer);
                if (!string.IsNullOrEmpty(weightStr) && decimal.TryParse(weightStr, out decimal weight))
                {
                    CurrentWeight = weight;
                    WeightReceived?.Invoke(weight);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"读取串口数据失败：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 解析重量数据
        /// 支持多种地磅仪表协议格式
        /// </summary>
        private string ParseWeightData(byte[] data)
        {
            try
            {
                // 方式 1: ASCII 格式，如 "GS,+   12.345kg" 或 "=12.345"
                string asciiData = Encoding.ASCII.GetString(data).Trim();
                
                // 查找等号后的数字
                if (asciiData.Contains("="))
                {
                    int eqIndex = asciiData.IndexOf('=');
                    string weightPart = asciiData.Substring(eqIndex + 1).Trim();
                    return ExtractNumber(weightPart);
                }
                
                // 查找 GS,前缀的格式
                if (asciiData.ToUpper().Contains("GS,"))
                {
                    int gsIndex = asciiData.ToUpper().IndexOf("GS,");
                    string weightPart = asciiData.Substring(gsIndex + 3).Trim();
                    return ExtractNumber(weightPart);
                }
                
                // 方式 2: 直接数字格式
                string numberStr = ExtractNumber(asciiData);
                if (!string.IsNullOrEmpty(numberStr))
                {
                    return numberStr;
                }
                
                // 方式 3: 带 STX/ETX 的格式 (0x02...0x03)
                if (data.Length > 2 && data[0] == 0x02)
                {
                    int etxIndex = Array.IndexOf(data, (byte)0x03);
                    if (etxIndex > 1)
                    {
                        string content = Encoding.ASCII.GetString(data, 1, etxIndex - 1);
                        return ExtractNumber(content);
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// 从字符串中提取数字
        /// </summary>
        private string ExtractNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            
            StringBuilder sb = new StringBuilder();
            bool hasDecimalPoint = false;
            
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
                else if (c == '.' || c == ',')
                {
                    if (!hasDecimalPoint)
                    {
                        sb.Append('.');
                        hasDecimalPoint = true;
                    }
                }
                else if (c == '-' && sb.Length == 0)
                {
                    sb.Append(c);
                }
            }
            
            return sb.Length > 0 ? sb.ToString() : null;
        }
        
        /// <summary>
        /// 发送指令到仪表
        /// </summary>
        public bool SendCommand(string command)
        {
            try
            {
                if (!_isConnected || !_serialPort.IsOpen)
                {
                    return false;
                }
                
                _serialPort.Write(command);
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"发送指令失败：{ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 获取稳定重量（等待重量稳定后返回）
        /// </summary>
        public async Task<decimal> GetStableWeightAsync(int timeoutSeconds = 10, decimal stabilityThreshold = 0.01m)
        {
            DateTime startTime = DateTime.Now;
            decimal lastWeight = CurrentWeight;
            int stableCount = 0;
            const int requiredStableCount = 5;
            
            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            {
                await Task.Delay(200);
                
                if (Math.Abs(CurrentWeight - lastWeight) < stabilityThreshold)
                {
                    stableCount++;
                    if (stableCount >= requiredStableCount)
                    {
                        return CurrentWeight;
                    }
                }
                else
                {
                    stableCount = 0;
                    lastWeight = CurrentWeight;
                }
            }
            
            throw new TimeoutException("等待重量稳定超时");
        }
        
        public void Dispose()
        {
            Disconnect();
        }
    }
}
