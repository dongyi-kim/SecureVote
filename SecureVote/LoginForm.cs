using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Json;

namespace SecureVote
{
    public partial class LoginForm : Form
    {
        private bool isSuccess = false;
        public static bool DoLogin(Form form)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog(form);
            return loginForm.isSuccess;
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            isSuccess = SecureProtocol.Req_PublicKey(txtID.Text)
                    && SecureProtocol.Req_Auth(txtID.Text, txtPW.Text);
            this.Close();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Console.WriteLine("[!] 로그인이 필요합니다.");
        }
    }
}
