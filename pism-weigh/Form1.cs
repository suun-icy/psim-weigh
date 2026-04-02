using Newtonsoft.Json;
using pism_weigh.Database;
using pism_weigh.Models;
using pism_weigh.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pism_weigh
{
    public partial class Form1 : Form
    {
        
        private long receive_count = 0;
        private StringBuilder sb = new StringBuilder();
        string[] str = new string[50];
        int i = 0;
        private ScaleService scaleService = new ScaleService();
        private const int StableReadThreshold = 3;
        private const double StableDeltaThreshold = 0.005;
        private const string WeightUnit = " t";
        private double? _lastCurrentWeight;
        private double? _stableWeight;
        private int _stableReadCount;
        private int _printCount;
		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置默认值
            comboBox2.Text = "600";
            comboBox3.Text = "8";
            comboBox4.Text = "None";
            comboBox5.Text = "1";
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            comboBox7.Items.Add("皖");
            comboBox7.Items.Add("京");
            comboBox7.Items.Add("沪");
            comboBox7.Items.Add("津");
            comboBox7.Items.Add("渝");
            comboBox7.Items.Add("鲁");
            comboBox7.Items.Add("冀");
            comboBox7.Items.Add("晋");
            comboBox7.Items.Add("蒙");
            comboBox7.Items.Add("辽");
            comboBox7.Items.Add("吉");
            comboBox7.Items.Add("黑");
            comboBox7.Items.Add("苏");
            comboBox7.Items.Add("浙");
            comboBox7.Items.Add("闽");
            comboBox7.Items.Add("赣");
            comboBox7.Items.Add("豫");
            comboBox7.Items.Add("湘");
            comboBox7.Items.Add("鄂");
            comboBox7.Items.Add("粤");
            comboBox7.Items.Add("桂");
            comboBox7.Items.Add("琼");
            comboBox7.Items.Add("川");
            comboBox7.Items.Add("贵");
            comboBox7.Items.Add("云");
            comboBox7.Items.Add("藏");
            comboBox7.Items.Add("陕");
            comboBox7.Items.Add("甘");
            comboBox7.Items.Add("青");
            comboBox7.Items.Add("宁");
            comboBox7.Items.Add("新");
            comboBox7.Items.Add("港");
            comboBox7.Items.Add("澳");
            comboBox7.Items.Add("台");

            LoadPlateHistoryFromDatabase();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //将可能产生异常的代码放置在try块中
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态
                    serialPort1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    button1.BackColor = Color.ForestGreen;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                   // textBox_receive.Text = "";  //清空接收区
                    //textBox_send.Text = "";     //清空发送区
                    label6.Text = "串口已关闭";
                    label6.ForeColor = Color.Red;

                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    textBox_receive.Text = "";
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);

                    if (comboBox4.Text.Equals("None"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("Odd"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("Even"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    else if (comboBox4.Text.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;
                    else if (comboBox4.Text.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;
                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
					//scaleService.Connect();     //打开串口
                    button1.Text = "关闭串口";

                    button1.BackColor = Color.Firebrick;
                    label6.Text = "串口已打开";
                    label6.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                //捕获可能发生的异常并进行处理

                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //车牌判空
            if (textBox5.Text == null || comboBox7.SelectedIndex == -1)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }

            if (textBox6.Text == "")
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("称重为空");
                return;
            }
            else
            {
                //填充数据
                textBox1.Text = textBox6.Text;
            }

            

        }

		private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				int len = serialPort1.BytesToRead;
				byte[] buffer = new byte[len];

				serialPort1.Read(buffer, 0, len);

				receive_count += len;

				this.Invoke((Action)(() =>
				{
					// 👉 HEX显示
					if (radioButton2.Checked)
					{
						string hex = BitConverter.ToString(buffer).Replace("-", " ");
						textBox_receive.AppendText(hex + "\r\n");
					}
					else
					{
						// 👉 ASCII解析
						string asciiFrame = Encoding.ASCII.GetString(buffer);
						textBox_receive.AppendText(asciiFrame + "\r\n");

						if (ScaleService.TryParseWeightFrame(asciiFrame, out double currentWeight))
						{
							textBoxCurrentWeight.Text = FormatWeight(currentWeight);
							UpdateStableWeight(currentWeight);
						}
						else
						{
							textBoxCurrentWeight.Text = "--";
						}
					}

					label7.Text = "Rx:" + receive_count + " Bytes";
				}));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

        private void UpdateStableWeight(double currentWeight)
        {
            if (!_lastCurrentWeight.HasValue)
            {
                _stableReadCount = 1;
            }
            else if (Math.Abs(currentWeight - _lastCurrentWeight.Value) <= StableDeltaThreshold)
            {
                _stableReadCount++;
            }
            else
            {
                _stableReadCount = 1;
            }

            _lastCurrentWeight = currentWeight;
            if (_stableReadCount >= StableReadThreshold)
            {
                _stableWeight = currentWeight;
                textBox6.Text = FormatWeight(currentWeight);
            }
        }

        private static string FormatWeight(double weight)
        {
            return $"{weight.ToString("F3", CultureInfo.InvariantCulture)}{WeightUnit}";
        }

        private bool TryParseWeightText(string text, string fieldName, out double weight)
        {
            weight = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show($"{fieldName}为空");
                return false;
            }

            string numericText = text.Replace(WeightUnit, string.Empty).Trim();
            if (double.TryParse(numericText, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out weight)
                || double.TryParse(numericText, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out weight))
            {
                return true;
            }

            MessageBox.Show($"{fieldName}格式错误：{text}");
            return false;
        }

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox_receive.Text = "";  //清空接收文本框
           
            receive_count = 0;          //计数清零
            label7.Text = "Rx:" + receive_count.ToString() + "Bytes";   //刷新界面
        }

        private void button6_Click(object sender, EventArgs e)
        {
            print print = new print(OnRecordReprinted);
            print.ShowDialog();
        }

        private void OnRecordReprinted(Models.WeighRecord record)
        {
            if (record == null)
            {
                return;
            }

            label7.Text = "最近重打：" + record.PlateNumber + "（打印次数 " + record.PrintCount + "）";
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //车牌判空
            if (textBox5.Text == null || comboBox7.SelectedIndex == -1) {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }

            if (textBox6.Text == "")
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("称重为空");
                return;
            }
            else {
                //填充数据
                textBox2.Text = textBox6.Text;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    //串口处于开启状态，将发送区文本发送
                    serialPort1.Write(textBox4.Text);
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //车牌判空
            if (textBox5.Text == null || comboBox7.SelectedIndex == -1)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }
            

            //重量是否为空
            if (textBox1.Text == ""|| textBox2.Text == "")
            {
                MessageBox.Show("称重错误,请重试");
                return ;

            }
            else
            {
                //是否空车重于重车
                if (!TryParseWeightText(textBox1.Text, "重车重量", out double roughWeight)
                    || !TryParseWeightText(textBox2.Text, "空车重量", out double tareWeight))
                {
                    return;
                }

                if (roughWeight < tareWeight)
                {
                    MessageBox.Show("重车重量不能小于空车重量");
                    return;
                }

                double netWeight = roughWeight - tareWeight;
                textBox3.Text = FormatWeight(netWeight);
            }

        }

        private void textBox_receive_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //没有用户信息
            if (user.token == null || user.token.Equals(""))
            {
                //弹出登录界面
                UserLogin userLogin = new UserLogin();
                userLogin.ShowDialog();
            }
            //收集数据
            PmsWeightinfo weightinfo = new PmsWeightinfo();
            Boolean cargoComeOut = false;
            if (radioButton3.Checked)
            {
                cargoComeOut = false;
            }
            else {
                cargoComeOut = true;
            }

            string cargoPlate = $"{comboBox7.Text}{textBox5.Text?.Trim()}";
            if (string.IsNullOrWhiteSpace(comboBox7.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("车牌为空");
                return;
            }

            if (!TryParseWeightText(textBox1.Text, "重车重量", out double roughWeight)
                || !TryParseWeightText(textBox2.Text, "空车重量", out double tareWeight)
                || !TryParseWeightText(textBox3.Text, "净重", out double netWeight))
            {
                return;
            }

            weightinfo.cargoPlate = cargoPlate;
            weightinfo.roughWeight = roughWeight;
            weightinfo.tare = tareWeight;
            weightinfo.netWeight = netWeight;
            weightinfo.psimType = textBox1.Text;
            weightinfo.cargoComeOut = cargoComeOut;
            _printCount++;
            weightinfo.printCount = _printCount;
            weightinfo.printUser = user.userName;
            weightinfo.createDate = new DateTime();
            //开始上传数据至数据库
            NameValueCollection valueCollection = new NameValueCollection();
            valueCollection.Set("Authorization", user.token);
            HTTPS.HttppPost("http://10.102.84.200:9999/pms/weightinfo/addWeightinfo", JsonConvert.SerializeObject(weightinfo), "utf-8", "application/json", valueCollection);
            EnsureCurrentPlateInHistory();

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
                //打开浏览器登录界面

            
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LoadPlateHistoryFromDatabase()
        {
            try
            {
                var allRecords = DatabaseHelper.GetAllWeighRecords();
                plateHistoryCache = allRecords
                    .Select(r => r.PlateNumber)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                RefreshPlateHistoryDropdown(plateHistoryCache);
            }
            catch
            {
                plateHistoryCache = new List<string>();
                RefreshPlateHistoryDropdown(plateHistoryCache);
            }
        }

        private void RefreshPlateHistoryDropdown(IEnumerable<string> plates)
        {
            isUpdatingPlateHistory = true;
            var currentText = comboBoxPlateHistory.Text;

            comboBoxPlateHistory.BeginUpdate();
            comboBoxPlateHistory.Items.Clear();
            foreach (var plate in plates)
            {
                comboBoxPlateHistory.Items.Add(plate);
            }
            comboBoxPlateHistory.EndUpdate();
            comboBoxPlateHistory.Text = currentText;
            comboBoxPlateHistory.SelectionStart = comboBoxPlateHistory.Text.Length;
            isUpdatingPlateHistory = false;
        }

        private void comboBoxPlateHistory_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingPlateHistory)
            {
                return;
            }

            var keyword = comboBoxPlateHistory.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                RefreshPlateHistoryDropdown(plateHistoryCache);
                return;
            }

            try
            {
                var dynamicPlates = DatabaseHelper.GetWeighRecordsByPlate(keyword)
                    .Select(r => r.PlateNumber)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                RefreshPlateHistoryDropdown(dynamicPlates);
                comboBoxPlateHistory.DroppedDown = true;
                comboBoxPlateHistory.SelectionStart = comboBoxPlateHistory.Text.Length;
            }
            catch
            {
                var fallbackPlates = plateHistoryCache
                    .Where(p => p.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                RefreshPlateHistoryDropdown(fallbackPlates);
            }
        }

        private void comboBoxPlateHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingPlateHistory)
            {
                return;
            }

            var selectedPlate = comboBoxPlateHistory.Text.Trim();
            if (string.IsNullOrWhiteSpace(selectedPlate))
            {
                return;
            }

            try
            {
                var records = DatabaseHelper.GetWeighRecordsByPlate(selectedPlate);
                var latestRecord = records
                    .FirstOrDefault(r => string.Equals(r.PlateNumber, selectedPlate, StringComparison.OrdinalIgnoreCase))
                    ?? records.FirstOrDefault();

                if (latestRecord == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(latestRecord.Province) && comboBox7.Items.Contains(latestRecord.Province))
                {
                    comboBox7.SelectedItem = latestRecord.Province;
                }
                else if (!string.IsNullOrWhiteSpace(latestRecord.PlateNumber))
                {
                    var province = latestRecord.PlateNumber.Substring(0, 1);
                    if (comboBox7.Items.Contains(province))
                    {
                        comboBox7.SelectedItem = province;
                    }
                }

                if (!string.IsNullOrWhiteSpace(latestRecord.PlateCode))
                {
                    textBox5.Text = latestRecord.PlateCode;
                }
                else if (!string.IsNullOrWhiteSpace(latestRecord.PlateNumber) && latestRecord.PlateNumber.Length > 1)
                {
                    textBox5.Text = latestRecord.PlateNumber.Substring(1);
                }

                // 仅回填基础业务信息，不覆盖当前实时重量
                if (latestRecord.BusinessType == BusinessType.SalesOut)
                {
                    radioButton4.Checked = true;
                }
                else
                {
                    radioButton3.Checked = true;
                }
            }
            catch
            {
                // 读取历史记录失败时忽略，避免影响称重流程
            }
        }

        private void EnsureCurrentPlateInHistory()
        {
            var currentPlate = string.Format("{0}{1}", comboBox7.Text, textBox5.Text).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(currentPlate))
            {
                return;
            }

            if (!plateHistoryCache.Any(p => string.Equals(p, currentPlate, StringComparison.OrdinalIgnoreCase)))
            {
                plateHistoryCache.Insert(0, currentPlate);
                RefreshPlateHistoryDropdown(plateHistoryCache);
            }
        }
    }
}
