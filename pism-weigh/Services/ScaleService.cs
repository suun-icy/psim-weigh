using System;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace pism_weigh.Services
{
	/// <summary>
	/// 地磅称重服务
	/// </summary>
	public class ScaleService : IDisposable
	{
		private SerialPort _serialPort;
		private readonly object _lock = new object();
		private bool _isDisposed = false;
		private Dispatcher _uiDispatcher;
		private Timer _stabilityTimer;
		private double _lastWeight = 0.0;
		private int _stableCount = 0;
		private const int RequiredStableReads = 3; // 连续几次读数一致视为稳定
		public event Action<double> WeightReceived;
		public event Action<string> ErrorOccurred;
		public string PortName { get; set; } = "COM1";
		public int BaudRate { get; set; } = 9600;
		public Parity Parity { get; set; } = Parity.None;
		public int DataBits { get; set; } = 8;
		public StopBits StopBits { get; set; } = StopBits.One;

		public bool IsConnected => _serialPort != null && _serialPort.IsOpen;
		public double CurrentWeight { get; private set; } = 0.0;
		public ScaleService()
		{
			_uiDispatcher = Dispatcher.CurrentDispatcher;
		}
		public void Connect()
		{
			if (_isDisposed) throw new ObjectDisposedException(nameof(ScaleService));
			lock (_lock)
			{
				if (_serialPort != null && _serialPort.IsOpen)
				{
					Disconnect();
				}
				_serialPort = new SerialPort
				{
					PortName = PortName,
					BaudRate = BaudRate,
					Parity = Parity,
					DataBits = DataBits,
					StopBits = StopBits,
					ReadTimeout = 1000,
					WriteTimeout = 1000
				};
				_serialPort.DataReceived += SerialPort_DataReceived;
				_serialPort.ErrorReceived += SerialPort_ErrorReceived;
				try
				{
					_serialPort.Open();
					StartStabilityCheck();
				}
				catch (Exception ex)
				{
					RaiseError("打开串口失败：" + ex.Message);
					throw;
				}
			}
		}
		public void Disconnect()
		{
			lock (_lock)
			{
				if (_stabilityTimer != null)
				{
					_stabilityTimer.Dispose();
					_stabilityTimer = null;
				}
				if (_serialPort != null)
				{
					try
					{
						if (_serialPort.IsOpen)
						{
							_serialPort.Close();
							_serialPort.Dispose();
						}
					}
					catch { }
					finally
					{
						_serialPort = null;
					}
				}
			}
		}
		private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			RaiseError("串口通信错误：" + e.EventType);
		}
		private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				int len = _serialPort.BytesToRead;
				byte[] buffer = new byte[len];

				_serialPort.Read(buffer, 0, len);

				// 👉 打印原始数据（调试用）
				string hex = BitConverter.ToString(buffer);
				Console.WriteLine("收到数据: " + hex);

				// 👉 如果是ASCII协议（部分地磅）
				string str = Encoding.ASCII.GetString(buffer);
				Console.WriteLine("字符串: " + str);

				double weight = ParseWeight(str);
				UpdateWeight(weight);
			}
			catch (Exception ex)
			{
				RaiseError("读取数据异常：" + ex.Message);
			}
		}
		private double ParseWeight(string data)
		{
			if (string.IsNullOrWhiteSpace(data)) return 0.0;
			// 去除非数字字符，保留负号和小数点
			string pattern = @"[-+]?\d*\.?\d+";
			Match match = Regex.Match(data, pattern);
			if (match.Success)
			{
				if (double.TryParse(match.Value, out double result))
				{
					return result;
				}
			}
			return 0.0;
		}
		private void UpdateWeight(double weight)
		{
			CurrentWeight = weight;

			if (WeightReceived != null)
			{
				_uiDispatcher.Invoke(() => WeightReceived(weight));
			}
		}
		private void StartStabilityCheck()
		{
			_stabilityTimer = new Timer(CheckStability, null, 0, 500);
		}
		private void CheckStability(object state)
		{
			// 此处可扩展稳定性判断逻辑
			// 目前简化为直接推送最新数据
		}
		private void RaiseError(string message)
		{
			if (ErrorOccurred != null)
			{
				_uiDispatcher.Invoke(() => ErrorOccurred(message));
			}
		}
		public void Dispose()
		{
			if (!_isDisposed)
			{
				Disconnect();
				_isDisposed = true;
			}
		}
	}
}
