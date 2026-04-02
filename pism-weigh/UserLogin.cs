using System;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace pism_weigh
{
    public partial class UserLogin : Form
    {
        public UserLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // string url = "https://psim.sunhaijie.top:9999/ums/login";
            string url = "http://localhost:9999/ums/login";
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("账户或密码为空");
            }
            else
            {
                string username = textBox1.Text;
                string password = textBox2.Text;
                //发送https请求

                string loginInfo = "username=" + username + "&password=" + password;
                string s = HTTPS.HttppPost(url, loginInfo);
                if (s != "")
                {
               //     string json= JsonConvert.SerializeObject(s);
                    UserInfo user2 = JsonConvert.DeserializeObject<UserInfo>(s);
                    user.userId = user2.data.userId;
                    user.userName = user2.data.userName;
                    user.roleId = user2.data.roleId;
                    user.roleName = user2.data.roleName;
                    user.roleDescription = user2.data.roleDescription;
                    user.token = user2.data.token;
                    
                    MessageBox.Show(s);
                    this.Dispose();
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
