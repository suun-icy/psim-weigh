using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace pism_weigh
{
    public partial class Form1 : Form
    {
        
        private long receive_count = 0;
        private StringBuilder sb = new StringBuilder();
        string[] str = new string[50];
        int i = 0;
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
            }

            

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            int num = serialPort1.ReadBufferSize;      //获取接收缓冲区中的字节数

            byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据
           
            receive_count += num;                   //接收字节计数变量增加nun
         //   serialPort1.Read(received_buf, 0, num);   //读取接收缓冲区中num个字节到byte数组中

            sb.Clear();     //防止出错,首先清空字符串构造器
                            //遍历数组进行字符串转化及拼接
            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                this.Invoke((EventHandler)(delegate
                {
   
                    if (radioButton2.Checked)//选中HEX模式显示
                    {
                        foreach (byte b in received_buf)
                        {
                            sb.Append(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                        }
                    }
                    else
                    {
                        byte firstByte = Convert.ToByte(serialPort1.ReadByte());
                        if (firstByte == 0x02)
                        {
                            int bytesRead = serialPort1.ReadBufferSize;
                            //byte[] bytesData = new byte[bytesRead];
                            byte byteData;

                            for (int i = 0; i < bytesRead - 1; i++)
                            {
                                byteData = Convert.ToByte(serialPort1.ReadByte());
                                if (byteData == 0x03)//结束
                                {
                                    break;
                                }
                                received_buf[i] = byteData;
                            }
                            //strReceive = Encoding.Default.GetString(bytesData);
                        }
                        Array.Reverse(received_buf);
                        //选中ASCII模式显示
                        sb.Append(Encoding.ASCII.GetString(received_buf) + "\r\n");  //将整个数组解码为ASCII数组
                        string weigh = Encoding.ASCII.GetString(received_buf);
                        textBox6.Text = weigh.Substring(weigh.IndexOf("=") + 1).TrimStart('0');

                    }
                    textBox_receive.AppendText(sb.ToString());
                    label7.Text = "Rx:" + receive_count.ToString() + "Bytes";
                }
                   )
                );

            }

            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }
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
            print print = new print();
            print.ShowDialog();
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
                if (double.Parse(textBox1.Text) < double.Parse(textBox2.Text))
                {

                }
                double netWeight = (double.Parse(textBox1.Text) - double.Parse(textBox2.Text));
                textBox3.Text = Convert.ToString(netWeight);
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

            weightinfo.cargoPlate = textBox1.Text;
            weightinfo.roughWeight = double.Parse(textBox1.Text);
            weightinfo.tare = double.Parse(textBox1.Text);
            weightinfo.netWeight = double.Parse(textBox1.Text);
            weightinfo.psimType = textBox1.Text;
            weightinfo.cargoComeOut = cargoComeOut;
            weightinfo.printCount = int.Parse(textBox1.Text);
            weightinfo.printUser = user.userName;
            weightinfo.createDate = new DateTime();
            //开始上传数据至数据库
            NameValueCollection valueCollection = new NameValueCollection();
            valueCollection.Set("Authorization", user.token);
            HTTPS.HttppPost("http://10.102.84.200:9999/pms/weightinfo/addWeightinfo", JsonConvert.SerializeObject(weightinfo), "utf-8", "application/json", valueCollection);

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

        /// <summary>
        /// 手动模式切换事件
        /// </summary>
        private void checkBoxManualMode_CheckedChanged(object sender, EventArgs e)
        {
            bool isManual = checkBoxManualMode.Checked;
            if (isManual)
            {
                // 手动模式
                textBox6.Enabled = false;
                labelManualTip.Visible = true;
                label6.Text = "手动模式";
                label6.ForeColor = System.Drawing.Color.DarkOrange;
            }
            else
            {
                // 联机模式（恢复原有行为）
                textBox6.Enabled = true;
                labelManualTip.Visible = false;
                label6.Text = "串口已关闭";
                label6.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
