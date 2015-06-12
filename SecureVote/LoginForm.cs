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
        public static bool DoLogin(Form form, ref String user_id, ref String vote_id)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog(form);
            if (loginForm.isSuccess)
            {
                user_id =  loginForm.getUserID();
                vote_id = loginForm.getVoteID();
            }
            return loginForm.isSuccess;
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        public String getUserID() { return txtID.Text; }
        public String getVoteID() { return txtVoteID.Text; }
       
        private void btnLogin_Click(object sender, EventArgs e)
        {
            isSuccess = SecureProtocol.Req_PublicKey(txtID.Text)
                    && SecureProtocol.Req_Auth(txtID.Text, txtPW.Text);
            if (isSuccess)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("로그인에 실패하였습니다.");
            }
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Console.WriteLine("[!] 로그인이 필요합니다.");
        }
    }
}
