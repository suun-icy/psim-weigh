using Newtonsoft.Json;
using pism_weigh.Database;
using pism_weigh.Models;
using pism_weigh.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace pism_weigh
{
    public partial class Form1 : Form
    {
        private enum WeighingMode
        {
            GrossFirst,
            TareFirst
        }

        private enum CapturedWeightType
        {
            Gross,
            Tare
        }
        
        private long receive_count = 0;
        private bool isUpdatingPlateHistory = false;
        private StringBuilder sb = new StringBuilder();
        string[] str = new string[50];
        int i = 0;
        private ScaleService scaleService = new ScaleService();
        private const int StableReadThreshold = 3;
        private const double StableDeltaThreshold = 0.005;
        private const string WeightUnit = " t";
        private const string ModeTagGrossFirst = "[MODE:GROSS_FIRST]";
        private const string ModeTagTareFirst = "[MODE:TARE_FIRST]";
        private double? _lastCurrentWeight;
        private double? _stableWeight;
        private int _stableReadCount;
        private int _printCount;
        private decimal? lastCapturedWeight;
        private CapturedWeightType? lastCapturedWeightType;
		private List<string> plateHistoryCache;
        private string _latestSourceUnit = "t";

		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyModernUiStyle();
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
			comboBox8.Items.Add("先毛后皮");
            comboBox8.Items.Add("先皮后毛");
            comboBox8.SelectedIndex = 0;
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
            if (string.IsNullOrWhiteSpace(textBox5.Text) || comboBox7.SelectedIndex == -1)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }

            // 手动模式：用户直接在textBox1中输入重量，按钮用于确认
            if (checkBoxManualMode.Checked)
            {
                if (textBox1.Text == "" || textBox1.Text == "0")
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("请手动输入重车重量");
                    return;
                }
                MessageBox.Show("重车重量已确认：" + textBox1.Text + " 吨");
                return;
            }

            // 联机模式：从串口实时重量复制
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
                if (TryParseWeightText(textBox1.Text, "重车重量", out double captured))
                {
                    lastCapturedWeight = Convert.ToDecimal(captured);
                    lastCapturedWeightType = CapturedWeightType.Gross;
                }
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

						if (ScaleService.TryParseWeightFrame(asciiFrame, out double currentWeight, out string sourceUnit))
						{
                            _latestSourceUnit = sourceUnit;
                            DatabaseHelper.SaveRawWeightLog(asciiFrame, currentWeight, sourceUnit);
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
            if (string.IsNullOrWhiteSpace(textBox5.Text) || comboBox7.SelectedIndex == -1) {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }

            // 手动模式：用户直接在textBox2中输入重量，按钮用于确认
            if (checkBoxManualMode.Checked)
            {
                if (textBox2.Text == "" || textBox2.Text == "0")
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("请手动输入空车重量");
                    return;
                }
                MessageBox.Show("空车重量已确认：" + textBox2.Text + " 吨");
                return;
            }

            // 联机模式：从串口实时重量复制
            if (textBox6.Text == "")
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("称重为空");
                return;
            }
            else {
                //填充数据
                textBox2.Text = textBox6.Text;
                if (TryParseWeightText(textBox2.Text, "空车重量", out double captured))
                {
                    lastCapturedWeight = Convert.ToDecimal(captured);
                    lastCapturedWeightType = CapturedWeightType.Tare;
                }
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
            if (string.IsNullOrWhiteSpace(textBox5.Text) || comboBox7.SelectedIndex == -1)
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("车牌为空");
                return;
            }

            if (lastCapturedWeight == null || lastCapturedWeightType == null)
            {
                MessageBox.Show("请先点击“称取重车”或“称取空车”采集本次重量。");
                return;
            }

            var currentWeight = lastCapturedWeight.Value;
            var currentType = lastCapturedWeightType.Value;
            var plate = comboBox7.Text + textBox5.Text.Trim().ToUpper();
            var openRecord = DatabaseHelper.GetLatestOpenRecordByPlate(plate);
            var selectedMode = GetSelectedMode();

            if (openRecord == null)
            {
                if ((selectedMode == WeighingMode.GrossFirst && currentType != CapturedWeightType.Gross) ||
                    (selectedMode == WeighingMode.TareFirst && currentType != CapturedWeightType.Tare))
                {
                    MessageBox.Show("当前称重模式与本次采集类型不一致，请先按模式进行首次称重。");
                    return;
                }

                var record = new WeighRecord
                {
                    PlateNumber = plate,
                    Province = comboBox7.Text,
                    PlateCode = textBox5.Text.Trim().ToUpper(),
                    GrossWeight = currentType == CapturedWeightType.Gross ? currentWeight : 0M,
                    TareWeight = currentType == CapturedWeightType.Tare ? currentWeight : 0M,
                    NetWeight = 0M,
                    BusinessType = radioButton3.Checked ? BusinessType.PurchaseIn : BusinessType.SalesOut,
                    Status = WeighStatus.FirstWeigh,
                    FirstWeighTime = DateTime.Now,
                    CompleteTime = DateTime.MinValue,
                    Remark = BuildModeRemark(selectedMode),
                    OperatorName = user.userName
                };

                if (!DatabaseHelper.SaveWeighRecord(record))
                {
                    MessageBox.Show("首次称重保存失败。");
                    return;
                }

                MessageBox.Show("首次称重已保存，请进行第二次称重。");
                textBox3.Text = "0";
                return;
            }

            var mode = GetModeFromRecord(openRecord, selectedMode);
            if ((mode == WeighingMode.GrossFirst && currentType != CapturedWeightType.Tare) ||
                (mode == WeighingMode.TareFirst && currentType != CapturedWeightType.Gross))
            {
                MessageBox.Show("第二次称重类型与未完成记录不匹配，已阻止保存。");
                return;
            }

            decimal grossWeight = openRecord.GrossWeight;
            decimal tareWeight = openRecord.TareWeight;
            if (currentType == CapturedWeightType.Gross)
            {
                grossWeight = currentWeight;
            }
            else
            {
                tareWeight = currentWeight;
            }

            if (mode == WeighingMode.GrossFirst && tareWeight > grossWeight)
            {
                var confirm = MessageBox.Show("第二次重量大于第一次，和“先毛后皮”模式不匹配，是否继续？", "异常确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }
            }

            if (mode == WeighingMode.TareFirst && grossWeight < tareWeight)
            {
                var confirm = MessageBox.Show("第二次重量小于第一次，和“先皮后毛”模式不匹配，是否继续？", "异常确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }
            }

            var netWeightValue = grossWeight - tareWeight;
            if (netWeightValue < 0)
            {
                MessageBox.Show("净重不得为负，已阻止保存。");
                return;
            }

            openRecord.GrossWeight = grossWeight;
            openRecord.TareWeight = tareWeight;
            openRecord.NetWeight = netWeightValue;
            openRecord.Status = WeighStatus.Completed;
            openRecord.SecondWeighTime = DateTime.Now;
            openRecord.CompleteTime = DateTime.Now;
            openRecord.UpdateTime = DateTime.Now;

            if (!DatabaseHelper.SaveWeighRecord(openRecord))
            {
                MessageBox.Show("第二次称重保存失败。");
                return;
            }

            textBox1.Text = grossWeight.ToString("0.###");
            textBox2.Text = tareWeight.ToString("0.###");
            textBox3.Text = netWeightValue.ToString("0.###");
            MessageBox.Show("称重完成并已保存。");
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
            
            var record = DatabaseHelper.GetLatestOpenRecordByPlate(cargoPlate);
            if (record == null)
            {
                var completed = DatabaseHelper.GetWeighRecordsByPlate(cargoPlate).FirstOrDefault(r => r.Status == WeighStatus.Completed);
                record = completed ?? new WeighRecord();
            }

            record.PlateNumber = cargoPlate;
            record.Province = comboBox7.Text;
            record.PlateCode = textBox5.Text?.Trim().ToUpperInvariant();
            record.GrossWeight = Convert.ToDecimal(roughWeight);
            record.TareWeight = Convert.ToDecimal(tareWeight);
            record.NetWeight = Convert.ToDecimal(netWeight);
            record.BusinessType = cargoComeOut ? BusinessType.SalesOut : BusinessType.PurchaseIn;
            record.Status = WeighStatus.Completed;
            record.FirstWeighTime = record.FirstWeighTime ?? DateTime.Now;
            record.SecondWeighTime = record.SecondWeighTime ?? DateTime.Now;
            record.CompleteTime = DateTime.Now;
            record.PrintCount = _printCount;
            record.OperatorName = user.userName;
            record.Remark = string.Format("LOCAL_ONLY|SRC_UNIT:{0}", _latestSourceUnit ?? "t");
            record.IsUploaded = false;
            record.UpdateTime = DateTime.Now;

            if (!DatabaseHelper.SaveWeighRecord(record))
            {
                MessageBox.Show("保存本地称重数据失败");
                return;
            }

            MessageBox.Show("称重单已保存到本地 SQLite。");
            EnsureCurrentPlateInHistory();

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
			//打开浏览器登录界面
			EnsureCurrentPlateInHistory();


		}

        private void ApplyModernUiStyle()
        {
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            BackColor = Color.FromArgb(245, 248, 252);

            var panels = new[] { panel1, panel2, panel3, panel4, panel5, panel6 };
            foreach (var panel in panels)
            {
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.FixedSingle;
            }

            var actionButtons = new[] { button1, button2, button4, button5, button6, button7, button8, button9, button10, button3 };
            foreach (var button in actionButtons)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 223);
                button.BackColor = Color.FromArgb(240, 244, 250);
            }

            textBoxCurrentWeight.Font = new Font("Consolas", 22F, FontStyle.Bold);
            textBox6.Font = new Font("Consolas", 22F, FontStyle.Bold);
            textBox_receive.BackColor = Color.FromArgb(250, 251, 253);

            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_receive.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxCurrentWeight.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            textBox6.Anchor = AnchorStyles.Top | AnchorStyles.Left;
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

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private WeighingMode GetSelectedMode()
        {
            return comboBox8.SelectedIndex == 1 ? WeighingMode.TareFirst : WeighingMode.GrossFirst;
        }

        private string BuildModeRemark(WeighingMode mode)
        {
            return mode == WeighingMode.TareFirst ? ModeTagTareFirst : ModeTagGrossFirst;
        }

        private WeighingMode GetModeFromRecord(WeighRecord record, WeighingMode fallback)
        {
            if (!string.IsNullOrEmpty(record.Remark))
            {
                if (record.Remark.Contains(ModeTagTareFirst))
                {
                    return WeighingMode.TareFirst;
                }

        }
    }
}
