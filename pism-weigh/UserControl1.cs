using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pism_weigh
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://psim.sunhaijie.top:9999/ums/login";
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("账户或密码为空");
            }
            else {
                string username = textBox1.Text;
                string password = textBox2.Text;
                //发送https请求

              string loginInfo = "username=" + username + "& password =" + password;
              string s=  HTTPS.HttppPost(url, loginInfo);
                if (s != "") {
                    MessageBox.Show(s);
                }
            }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
